using AutoFixture.Xunit2;
using SingleTableDynamo.Helpers;
using SingleTableDynamo.Predicates;
using FluentAssertions;
using Xunit;

namespace SingleTableDynamo.Tests.Predicates
{
    public class SortKeySearchPredicateBetweenTests
    {
        [Theory, AutoData]
        public void ToFilterExpression_WithStringAttributeValue_ShouldReturnCorrectFilterExpression(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = SortKeySearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            sut.ToFilterExpression().Should().Be($"{attributeName.AliasFor()} BETWEEN {(attributeName + "From").VariableFor()} AND {(attributeName + "To").VariableFor()}");
        }

        [Theory, AutoData]
        public void ToExpressionAttributeValues_WithStringAttributeValue_ShouldReturnCorrectAttributeValues(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = SortKeySearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeValues = sut.ToExpressionAttributeValues();
            attributeValues[(attributeName + "From").VariableFor()].S.Should().Be(attributeValueFrom);
            attributeValues[(attributeName + "To").VariableFor()].S.Should().Be(attributeValueTo);
        }

        [Theory, AutoData]
        public void ToExpressionAttributeNames_WithStringAttributeValue_ShouldReturnCorrectAttributeNames(string attributeName, string attributeValueFrom, string attributeValueTo)
        {
            var sut = SortKeySearchPredicate.Between(attributeName, attributeValueFrom, attributeValueTo);
            var attributeNames = sut.ToExpressionAttributeNames();
            attributeNames[attributeName.AliasFor()].Should().Be(attributeName);
        }
    }
}