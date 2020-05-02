using AutoFixture.Xunit2;
using SingleTableDynamo.Helpers;
using SingleTableDynamo.Predicates;
using FluentAssertions;
using Xunit;

namespace SingleTableDynamo.Tests.Predicates
{
    public class FilterSearchPredicateBetweenTests
    {
        [Theory, AutoData]
        public void ToFilterExpression_WithStringAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            sut.ToFilterExpression().Should().Be($"{attributeName.AliasFor()} BETWEEN {(attributeName + "From").VariableFor()} AND {(attributeName + "To").VariableFor()}");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithStringAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[(attributeName + "From").VariableFor()].S.Should().Be(attributeValueFrom);
            attributeValues[(attributeName + "To").VariableFor()].S.Should().Be(attributeValueTo);
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithStringAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }

        [Theory, AutoData]
        public void ToFilterExpression_WithLongAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, long attributeValueFrom, long attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            sut.ToFilterExpression().Should().Be($"{attributeName.AliasFor()} BETWEEN {(attributeName + "From").VariableFor()} AND {(attributeName + "To").VariableFor()}");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithLongAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, long attributeValueFrom, long attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[(attributeName + "From").VariableFor()].N.Should().Be(attributeValueFrom.ToString());
            attributeValues[(attributeName + "To").VariableFor()].N.Should().Be(attributeValueTo.ToString());
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithLongAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, long attributeValueFrom, long attributeValueTo)
        {
            var sut = FilterSearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }
    }
}