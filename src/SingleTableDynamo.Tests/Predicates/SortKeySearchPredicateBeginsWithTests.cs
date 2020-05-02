using AutoFixture.Xunit2;
using SingleTableDynamo.Helpers;
using SingleTableDynamo.Predicates;
using FluentAssertions;
using Xunit;

namespace SingleTableDynamo.Tests.Predicates
{
    public class SortKeySearchPredicateBeginsWithTests
    {
        [Theory, AutoData]
        public void ToFilterExpression_WithStringAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, string attributeValue)
        {
            var sut = SortKeySearchPredicate.BeginsWith(attributeName, attributeValue);
            sut.ToFilterExpression().Should().Be($"begins_with ({attributeName.AliasFor()}, {attributeName.VariableFor()})");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithStringAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, string attributeValue)
        {
            var sut = SortKeySearchPredicate.BeginsWith(attributeName, attributeValue);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[attributeName.VariableFor()].S.Should().Be(attributeValue);
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithStringAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, string attributeValue)
        {
            var sut = SortKeySearchPredicate.BeginsWith(attributeName, attributeValue);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }
    }
}