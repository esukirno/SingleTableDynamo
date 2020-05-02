using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;

namespace SingleTableDynamo.Operations
{
    public interface ISearchPredicateOperation
    {
        Dictionary<string, AttributeValue> ToExpressionAttributeValues();

        Dictionary<string, string> ToExpressionAttributeNames();

        string ToFilterExpression(bool checkAttributeNotExists = false);
    }
}