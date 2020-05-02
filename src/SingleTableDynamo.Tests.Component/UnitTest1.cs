using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DockerComposeFixture;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Polly;
using Xunit;
using Xunit.Abstractions;
using DynamoDbDocument = Amazon.DynamoDBv2.DocumentModel.Document;

namespace RacingFormAggregator.Tests.Component
{
    [CollectionDefinition("ComponentTests")]
    public class ComponentTestsCollection : ICollectionFixture<ComponentTestsFixture>
    {
    }

    public class ComponentTestsFixture : DockerFixture
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IDynamoDBContext _dynamoDbContext;
        private const string SqsServiceUrl = "http://localhost:4576";
        private const string DynamoDbServiceUrl = "http://localhost:4569";
        private readonly DynamoDbRacingFormRepository _racingFormRepository;
        private const string _tableName = "test_db";

        public DynamoDbRacingFormRepository RacingFormRepository => _racingFormRepository;

        public ComponentTestsFixture(IMessageSink messageSink) : base(messageSink)
        {
            var credentials = new BasicAWSCredentials("dummyaccess", "dummysecret");

            var sqsConfig = new AmazonSQSConfig
            {
                ServiceURL = SqsServiceUrl,
                UseHttp = true
            };
            _amazonSqsClient = new AmazonSQSClient(credentials, sqsConfig);
            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = DynamoDbServiceUrl,
                UseHttp = true
            };

            var metricsFactory = new MetricWriterFactory(new Mock<IAmazonCloudWatch>().Object, new MockMetricWriterSettingsOptions());
            _dynamoDbClient = new AmazonDynamoDBClient(credentials, dynamoDbConfig);
            _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
            _racingFormRepository = new DynamoDbRacingFormRepository(metricsFactory, _dynamoDbClient, _dynamoDbContext, _tableName);

            InitOnce(() => new DockerFixtureOptions
            {
                DockerComposeFiles = new[] { "../../../../../docker-compose.tests.yml" },
                CustomUpTest = output => output.Any(o => o.Contains("App is ready")),
                DockerComposeUpArgs = "--build",
                StartupTimeoutSecs = 600
            });
        }

        public async Task<SendMessageResponse> SendMessagesToQueueAsync<T>(IEnumerable<T> messages)
        {
            return await RetryAsync(r => r.HttpStatusCode != HttpStatusCode.OK, async () =>
            {
                var request = new SendMessageRequest
                {
                    QueueUrl = $"{SqsServiceUrl}/queue/test_queue",
                    MessageBody = JsonConvert.SerializeObject(messages.First())
                };

                return await _amazonSqsClient.SendMessageAsync(request);
            });
        }

        public async Task<IEnumerable<T>> QueryDynamoDbAsync<T>(string hashKey)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                Limit = 100,
                KeyConditionExpression = "HashKey = :v_hashkey",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_hashkey", new AttributeValue(hashKey) }
                }
            };

            var response = await RetryAsync(r => r.HttpStatusCode != HttpStatusCode.OK, async () => await _dynamoDbClient.QueryAsync(request));

            return response.Items.Select(item =>
                _dynamoDbContext.FromDocument<T>(DynamoDbDocument.FromAttributeMap(item)));
        }

        public async Task UpsertDynamoDbAsync<T>(IEnumerable<T> items) where T : ISingleTableItem
        {
            var itemsWithSearchPredicates = items.Select(i => new ItemWithSearchPredicates<T>() { Item = i, FilterSearchPredicates = null }).ToList();
            await _racingFormRepository.UpsertAsync("Item", itemsWithSearchPredicates);
        }

        public async Task UpsertDynamoDbAsync<T>(IEnumerable<ItemWithSearchPredicates<T>> items) where T : ISingleTableItem
        {
            await _racingFormRepository.UpsertAsync("Item", items.ToList());
        }

        public async Task<T> RetryAsync<T>(Func<T, bool> check, Func<Task<T>> action)
        {
            return await Policy
                .HandleResult(check)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(500))
                .ExecuteAsync(action);
        }
    }

    public class MockMetricWriterSettingsOptions : IOptions<MetricWriterSettings>
    {
        public MetricWriterSettings Value => new MetricWriterSettings();
    }
}