﻿using Amazon.DynamoDBv2;
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

        public readonly IDynamoDBSingleTableRepository<Staff> Repository;

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
            Repository = new DynamoDBSingleTableRepository<Staff>(_dynamoDbClient, _dynamoDbContext, new DoNothingMetricWriter(), _tableName);

            fixture.InitOnce(() => new DockerFixtureOptions
            {
                DockerComposeFiles = new[] { "../../../docker/docker-compose.yml" },
                CustomUpTest = output => output.Any(o => o.Contains("App is ready")),
                DockerComposeUpArgs = "--build",
                StartupTimeoutSecs = 500
            });
        }

        [Fact]
        public async void QueryAsync_UsingHashKeyAndSortKey_ReturnsCorrectItem()
        {
            var actual = new Staff
            {
                Id = "1",
                DOB = "2020-01-01"
            };
            await Repository.UpsertAsync(actual);

            var result = await Repository.QueryAsync(HashKeySearchPredicate.EqualTo(Repository.HashKeyName, "Staff#1"), SortKeySearchPredicate.EqualTo(Repository.SortKeyName, "DOB#2020-01-01"));
            Assert.NotEmpty(result);

            var expected = result.FirstOrDefault();
            Assert.NotNull(expected);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.DOB, actual.DOB);
            Assert.Equal(expected.HashKey, actual.HashKey);
            Assert.Equal(expected.SortKey, actual.SortKey);
        }
    }
}