using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using SingleTableDynamo.Documents;
using SingleTableDynamo.Metrics;
using System;

namespace SingleTableDynamo.Tests
{
    public class SampleRepository : DynamoDBSingleTableRepository<SampleItem>
    {
        public SampleRepository(IAmazonDynamoDB DynamoDBClient, IDynamoDBContext DynamoDBContext, IMetricWriter metricWriter) : base(DynamoDBClient, DynamoDBContext, metricWriter, "SingleTableDemo")
        {
        }
    }

    public class SampleItem : ISingleTableItem
    {
        public string HashKey => $"SampleItem#{SampleItemNumber}";
        public string SortKey => $"SortKey#{CreatedDate:yyyy-MM-dd}";
        public string SampleItemNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public SampleItem() { }

        public SampleItem(string sampleItemNumber, DateTime createdDate)
        {
            SampleItemNumber = sampleItemNumber;
            CreatedDate = createdDate;
        }
    }
}