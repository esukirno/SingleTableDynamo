using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using DockerComposeFixture;
using SingleTableDynamo.Predicates;
using SingleTableDynamo.Tests.Components.Models;
using SingleTableDynamo.Tests.Components.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SingleTableDynamo.Tests.Components
{
    [Collection("ComponentTests")]
    public class QueryItemTests : IClassFixture<DockerFixture>
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IDynamoDBContext _dynamoDbContext;
        private const string _tableName = "test_db";
        private const string DynamoDbServiceUrl = "http://localhost:4569";

        public readonly IDynamoDBSingleTableRepository<Item> Repository;

        private List<Item> _result;
        private List<Item> _items;

        public QueryItemTests(DockerFixture fixture)
        {
            var credentials = new BasicAWSCredentials("dummyaccess", "dummysecret");

            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = DynamoDbServiceUrl,
                UseHttp = true
            };

            _dynamoDbClient = new AmazonDynamoDBClient(credentials, dynamoDbConfig);
            _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
            Repository = new DynamoDBSingleTableRepository<Item>(_dynamoDbClient, _dynamoDbContext, new DoNothingMetricWriter(), _tableName);

            fixture.InitOnce(() => new DockerFixtureOptions
            {
                DockerComposeFiles = new[] { "../../../docker/docker-compose.yml" },
                CustomUpTest = output => output.Any(o => o.Contains("App is ready")),
                DockerComposeUpArgs = "--build",
                StartupTimeoutSecs = 120
            });
        }

        [Fact]
        public async void Test()
        {
            var result = await Repository.QueryAsync(HashKeySearchPredicate.EqualTo(Repository.HashKeyName, "Item#1"), SortKeySearchPredicate.EqualTo(Repository.SortKeyName, "Date#2020-01-01"));
            Assert.Empty(result);
        }
    }
}