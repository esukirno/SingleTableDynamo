using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Helpers;
using System;
using System.Collections.Generic;

namespace SingleTableDynamo.Operations
{
    public class SimpleSearchPredicateOperation : ISearchPredicateOperation
    {
        public string AttributeName { get; private set; }

        public ComparisonOperator ComparisonOperator { get; private set; }

        public AttributeValue AttributeValue { get; private set; }

        public SimpleSearchPredicateOperation(string attributeName, ComparisonOperator comparisonOperator, AttributeValue attributeValue)
        {
            AttributeName = attributeName;
            ComparisonOperator = comparisonOperator;
            AttributeValue = attributeValue;
        }

        public Dictionary<string, AttributeValue> ToExpressionAttributeValues()
        {
            return new Dictionary<string, AttributeValue> {
                { $"{AttributeName.VariableFor()}", AttributeValue }
            };
        }

        public Dictionary<string, string> ToExpressionAttributeNames()
        {
            return new Dictionary<string, string>
            {
                { AttributeName.AliasFor(), AttributeName }
            };
        }

        public string ToFilterExpression(bool checkAttributeNotExists = false)
        {
            var attributeName = AttributeName.AliasFor();
            var attributeValue = AttributeName.VariableFor();

            string attributeNameExpression;

            if (ComparisonOperator == ComparisonOperator.BEGINS_WITH)
            {
                attributeNameExpression = $"begins_with ({attributeName}, {attributeValue})";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else if (ComparisonOperator == ComparisonOperator.EQ)
            {
                attributeNameExpression = $"{attributeName} = {attributeValue}";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else if (ComparisonOperator == ComparisonOperator.LT)
            {
                attributeNameExpression = $"{attributeName} < {attributeValue}";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else if (ComparisonOperator == ComparisonOperator.LE)
            {
                attributeNameExpression = $"{attributeName} <= {attributeValue}";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else if (ComparisonOperator == ComparisonOperator.GT)
            {
                attributeNameExpression = $"{attributeName} > {attributeValue}";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else if (ComparisonOperator == ComparisonOperator.GE)
            {
                attributeNameExpression = $"{attributeName} >= {attributeValue}";
                return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
            }
            else
            {
                throw new ApplicationException($"ComparisonOperation {ComparisonOperator} is not supported");
            }
        }
    }
}