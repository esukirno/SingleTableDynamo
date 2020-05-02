using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using SingleTableDynamo;
using SingleTableDynamo.Documents;
using SingleTableDynamo.Metrics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SingleTableDynamo.Tests
{
    public class SampleRepository : DynamoDBSingleTableRepository<SampleItem>
    {
        public SampleRepository(IAmazonDynamoDB DynamoDBClient, IDynamoDBContext DynamoDBContext, IMetricWriter metricWriter) : base(DynamoDBClient, DynamoDBContext, metricWriter, "Sample")
        {
        }
    }

    public class SampleItem : ISingleTableItem
    {
        public string HashKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SortKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}