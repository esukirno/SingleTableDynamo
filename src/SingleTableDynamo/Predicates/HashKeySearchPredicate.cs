using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Operations;
using System.Collections.Generic;

namespace SingleTableDynamo.Predicates
{
    public class HashKeySearchPredicate : ISearchPredicate
    {
        public SearchPredicateType PredicateType => SearchPredicateType.HashKey;

        private readonly ISearchPredicateOperation _attributeValue;

        protected HashKeySearchPredicate(ISearchPredicateOperation attributeValue)
        {
            _attributeValue = attributeValue;
        }

        public Dictionary<string, AttributeValue> ToExpressionAttributeValues()
        {
            return _attributeValue.ToExpressionAttributeValues();
        }

        public Dictionary<string, string> ToExpressionAttributeNames()
        {
            return _attributeValue.ToExpressionAttributeNames();
        }

        public string ToFilterExpression(bool checkAttributeNotExists = false)
        {
            return _attributeValue.ToFilterExpression(checkAttributeNotExists);
        }

        public static HashKeySearchPredicate EqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.EQ, new AttributeValue { S = attributeValue });
            return new HashKeySearchPredicate(value);
        }
    }
}