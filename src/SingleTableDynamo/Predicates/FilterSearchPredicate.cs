using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Operations;
using System.Collections.Generic;

namespace SingleTableDynamo.Predicates
{
    public class FilterSearchPredicate : ISearchPredicate
    {
        public SearchPredicateType PredicateType => SearchPredicateType.Filter;

        private readonly ISearchPredicateOperation _attributeValue;

        protected FilterSearchPredicate(ISearchPredicateOperation attributeValue)
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

        public static FilterSearchPredicate EqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.EQ, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate EqualTo(string attributeName, long attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.EQ, new AttributeValue { N = attributeValue.ToString() });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate LessThan(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LT, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate LessThan(string attributeName, long attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LT, new AttributeValue { N = attributeValue.ToString() });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate LessThanOrEqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LE, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate LessThanOrEqualTo(string attributeName, long attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.LE, new AttributeValue { N = attributeValue.ToString() });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate GreaterThan(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GT, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate GreaterThan(string attributeName, long attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GT, new AttributeValue { N = attributeValue.ToString() });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate GreaterThanOrEqualTo(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GE, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate GreaterThanOrEqualTo(string attributeName, long attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.GE, new AttributeValue { N = attributeValue.ToString() });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate BeginsWith(string attributeName, string attributeValue)
        {
            var value = new SimpleSearchPredicateOperation(attributeName, ComparisonOperator.BEGINS_WITH, new AttributeValue { S = attributeValue });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate Between(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var value = new BetweenSearchPredicateOperation(attributeName, new AttributeValue { S = attributeValueFrom }, new AttributeValue { S = attributeValueTo });
            return new FilterSearchPredicate(value);
        }

        public static FilterSearchPredicate Between(string attributeName, long attributeValueFrom, long attributeValueTo)
        {
            var value = new BetweenSearchPredicateOperation(attributeName, new AttributeValue { N = attributeValueFrom.ToString() }, new AttributeValue { N = attributeValueTo.ToString() });
            return new FilterSearchPredicate(value);
        }
    }
}