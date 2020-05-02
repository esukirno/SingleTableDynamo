using Amazon;
using Amazon.CloudWatch;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using SingleTableDynamo.Metrics;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SingleTableDynamo.Tests
{
    public class SampleRepositoryChecks
    {
        [Fact]
        public async void UpsertChecks()
        {
            var dynamoDbclientConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast2
            };
            var dynamoDbClient = new AmazonDynamoDBClient(dynamoDbclientConfig);
            var dynamoDbContext = new DynamoDBContext(dynamoDbClient);
            var cloudWatchConfig = new AmazonCloudWatchConfig
            {
                RegionEndpoint = RegionEndpoint.APSoutheast2
            };
            var cloudWatchClient = new AmazonCloudWatchClient(cloudWatchConfig);
            var metricWriter = new CloudWatchMetricWriter(cloudWatchClient, new TestMetricWriterSettings());

            var repo = new SampleRepository(dynamoDbClient, dynamoDbContext, metricWriter);

            var item = new SampleItem("1", new DateTime(2020, 12, 30));
            await repo.UpsertAsync(item);
        }
    }

    public class TestMetricWriterSettings : IOptions<MetricWriterSettings>
    {
        public MetricWriterSettings Value => new MetricWriterSettings
        {
            CloudWatchNamespace = "SingleTableDynamoDemo"
        };
    }
}
