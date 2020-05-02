using AutoFixture.Xunit2;
using SingleTableDynamo.Helpers;
using SingleTableDynamo.Predicates;
using FluentAssertions;
using Xunit;

namespace SingleTableDynamo.Tests.Predicates
{
    public partial class FilterSearchPredicateGreaterThanOrEqualToTests
    {
        [Theory, AutoData]
        public void ToFilterExpression_WithStringAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, string attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            sut.ToFilterExpression().Should().Be($"{attributeName.AliasFor()} >= {attributeName.VariableFor()}");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithStringAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, string attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[attributeName.VariableFor()].S.Should().Be(attributeValue);
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithStringAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, string attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }

        [Theory, AutoData]
        public void ToFilterExpression_WithLongAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, long attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            sut.ToFilterExpression().Should().Be($"{attributeName.AliasFor()} >= {attributeName.VariableFor()}");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithLongAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, long attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[attributeName.VariableFor()].N.Should().Be(attributeValue.ToString());
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithLongAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, long attributeValue)
        {
            var sut = FilterSearchPredicate.GreaterThanOrEqualTo(attributeName, attributeValue);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }
    }
}