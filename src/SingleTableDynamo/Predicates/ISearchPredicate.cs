using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;

namespace SingleTableDynamo.Predicates
{
    public interface ISearchPredicate
    {
        SearchPredicateType PredicateType { get; }

        Dictionary<string, AttributeValue> ToExpressionAttributeValues();

        Dictionary<string, string> ToExpressionAttributeNames();

        string ToFilterExpression(bool checkAttributeNotExists = false);
    }
}