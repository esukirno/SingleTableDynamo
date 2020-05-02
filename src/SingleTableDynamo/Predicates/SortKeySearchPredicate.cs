using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Operations;
using System.Collections.Generic;

namespace SingleTableDynamo.Predicates
{
    public class SortKeySearchPredicate : ISearchPredicate
    {
        public SearchPredicateType PredicateType => SearchPredicateType.SortKey;

        private readonly ISearchPredicateOperation _attributeValue;

        protected SortKeySearchPredicate(ISearchPredicateOperation attributeValue)
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

        public static SortKeySearchPredicate EqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.EQ, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate LessThan(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LT, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate LessThanOrEqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LE, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate GreaterThan(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GT, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate GreaterThanOrEqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GE, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate BeginsWith(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.BEGINS_WITH, new AttributeValue { S = attributeValue });
            return new SortKeySearchPredicate(value);
        }

        public static SortKeySearchPredicate Between(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var value = new BetweenSearchPredicateOperation(attributeName, new AttributeValue { S = attributeValueFrom }, new AttributeValue { S = attributeValueTo });
            return new SortKeySearchPredicate(value);
        }
    }
}