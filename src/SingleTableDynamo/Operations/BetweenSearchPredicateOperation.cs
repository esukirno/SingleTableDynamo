using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Helpers;
using System.Collections.Generic;

namespace SingleTableDynamo.Operations
{
    public class BetweenSearchPredicateOperation : ISearchPredicateOperation
    {
        public string AttributeName { get; private set; }

        public ComparisonOperator ComparisonOperator => ComparisonOperator.BETWEEN;

        public AttributeValue AttributeValueFrom { get; private set; }

        public AttributeValue AttributeValueTo { get; private set; }

        public BetweenSearchPredicateOperation(string attributeName, AttributeValue attributeValueFrom, AttributeValue attributeValueTo)
        {
            AttributeName = attributeName;
            AttributeValueFrom = attributeValueFrom;
            AttributeValueTo = attributeValueTo;
        }

        public Dictionary<string, AttributeValue> ToExpressionAttributeValues()
        {
            return new Dictionary<string, AttributeValue> {
                { $"{(AttributeName + "From").VariableFor()}", AttributeValueFrom },
                { $"{(AttributeName + "To").VariableFor()}", AttributeValueTo }
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
            var attributeValueFrom = (AttributeName + "From").VariableFor();
            var attributeValueTo = (AttributeName + "To").VariableFor();

            string attributeNameExpression = $"{attributeName} BETWEEN {attributeValueFrom} AND {attributeValueTo}";
            return checkAttributeNotExists ? $"(attribute_not_exists({attributeName}) OR {attributeNameExpression})" : attributeNameExpression;
        }
    }
}