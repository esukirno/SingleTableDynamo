using Amazon;
using Amazon.CloudWatch;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using SingleTableDynamo.Metrics;
using SingleTableDynamo.Predicates;
using System;
using Xunit;

namespace SingleTableDynamo.Tests
{
    public class SampleRepositoryChecks
    {
        private readonly SampleRepository _sut;

        public SampleRepositoryChecks()
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

            _sut = new SampleRepository(dynamoDbClient, dynamoDbContext, metricWriter);
        }


        //[Theory, AutoData]
        public async void UpsertCheck(int number, DateTime dateTime)
        {
            var newItem = new SampleItem(number.ToString(), dateTime);
            await _sut.UpsertAsync(newItem);

            var queryResults = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Single(queryResults);

            await _sut.DeleteAsync(newItem);

            var queryResultsAfterCleanup = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Empty(queryResultsAfterCleanup);
        }

        //[Theory, AutoData]
        public async void ConditionalUpsertCheck(int number, DateTime dateTime)
        {
            var newItem = new SampleItem(number.ToString(), dateTime)
            {
                TimeStamp = 1000
            };
            await _sut.UpsertAsync(newItem, new[] { FilterSearchPredicate.LessThan("TimeStamp", newItem.TimeStamp) });

            var queryResults = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Single(queryResults);
            Assert.Equal(newItem.TimeStamp, queryResults[0].TimeStamp);

            var updatedItem = new SampleItem(number.ToString(), dateTime)
            {
                TimeStamp = 500
            };

            await _sut.UpsertAsync(updatedItem, new[] { FilterSearchPredicate.LessThan("TimeStamp", newItem.TimeStamp) });

            var queryResultsAfterUpdate = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Single(queryResultsAfterUpdate);
            Assert.Equal(newItem.TimeStamp, queryResults[0].TimeStamp);

            await _sut.DeleteAsync(newItem);

            var queryResultsAfterCleanup = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Empty(queryResultsAfterCleanup);
        }

        //[Theory, AutoData]
        public async void ConditionalDeleteCheck(int number, DateTime dateTime)
        {
            var newItem = new SampleItem(number.ToString(), dateTime)
            {
                TimeStamp = 1000
            };
            await _sut.UpsertAsync(newItem, new[] { FilterSearchPredicate.LessThan("TimeStamp", newItem.TimeStamp) });

            var queryResults = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Single(queryResults);
            Assert.Equal(newItem.TimeStamp, queryResults[0].TimeStamp);

            var updatedItem = new SampleItem(number.ToString(), dateTime)
            {
                TimeStamp = 500
            };

            await _sut.DeleteAsync(updatedItem, new[] { FilterSearchPredicate.LessThan("TimeStamp", newItem.TimeStamp) });

            var queryResultsAfterDelete = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Single(queryResultsAfterDelete);
            Assert.Equal(newItem.TimeStamp, queryResults[0].TimeStamp);

            await _sut.DeleteAsync(newItem);

            var queryResultsAfterCleanup = await _sut.QueryAsync(HashKeySearchPredicate.EqualTo(_sut.HashKeyName, newItem.HashKey), SortKeySearchPredicate.EqualTo(_sut.SortKeyName, newItem.SortKey));
            Assert.Empty(queryResultsAfterCleanup);
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
