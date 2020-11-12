using FluentAssertions;
using NUnit.Framework;
using Vostok.ServiceDiscovery.Extensions.TagFilters;

namespace Vostok.ServiceDiscovery.Extensions.Tests.TagFilters
{
    [TestFixture]
    internal class TagFilterExpressionHelpers_Tests
    {
        [Test]
        [TestCase("tag==value", "tag", "value")]
        [TestCase("tag == value", "tag", "value")]
        [TestCase("!tag==value", "!tag", "value")]
        [TestCase("  tag  ==  value q  ", "tag", "value q")]
        [TestCase("tag==      ", "tag", "")]
        [TestCase("tag==", "tag", "")]
        [TestCase("tag==value!=value1", "tag", "value!=value1")]
        [TestCase("tag==value==value1", "tag", "value==value1")]
        [TestCase("tag!=value==value1", "tag!=value", "value1")]
        public void Parse_should_correctly_parse_equals_filter_expression(string expression, string expectedKey, string expectedValue)
        {
            var filter = TagFilterExpressionHelpers.Parse(expression);
            filter.Should().BeOfType<EqualsTagFilter>();
            filter.Should().BeEquivalentTo(new EqualsTagFilter(expectedKey, expectedValue));
        }

        [Test]
        [TestCase("tag!=value", "tag", "value")]
        [TestCase("tag != value", "tag", "value")]
        [TestCase("!tag!=value", "!tag", "value")]
        [TestCase("  tag  !=  value q  ", "tag", "value q")]
        [TestCase("tag!=      ", "tag", "")]
        [TestCase("tag!=", "tag", "")]
        [TestCase("tag!=value!=value1", "tag", "value!=value1")]
        public void Parse_should_correctly_parse_not_equals_filter_expression(string expression, string expectedKey, string expectedValue)
        {
            var filter = TagFilterExpressionHelpers.Parse(expression);
            filter.Should().BeOfType<NotEqualsTagFilter>();
            filter.Should().BeEquivalentTo(new NotEqualsTagFilter(expectedKey, expectedValue));
        }

        [Test]
        [TestCase("tag", "tag")]
        [TestCase("tag=", "tag=")]
        [TestCase("tag=value", "tag=value")]
        [TestCase("tag!value", "tag!value")]
        [TestCase("  ==value", "==value")]
        [TestCase("  tag  ", "tag")]
        [TestCase("tag or tag2", "tag or tag2")]
        [TestCase("tag || tag2         ", "tag || tag2")]
        [TestCase("  tag && !tag2", "tag && !tag2")]
        [TestCase("  t!ag && tag2", "t!ag && tag2")]
        public void Parse_should_correctly_parse_contains_filter_expression(string expression, string expectedKey)
        {
            var filter = TagFilterExpressionHelpers.Parse(expression);
            filter.Should().BeOfType<ContainsTagFilter>();
            filter.Should().BeEquivalentTo(new ContainsTagFilter(expectedKey));
        }

        [Test]
        [TestCase("!tag", "tag")]
        [TestCase("  !tag  ", "tag")]
        [TestCase("  ! tag  ", "tag")]
        [TestCase("  !=value", "=value")]
        [TestCase("!tag or tag2", "tag or tag2")]
        [TestCase("!tag || tag2         ", "tag || tag2")]
        [TestCase(" ! tag && tag2", "tag && tag2")]
        [TestCase(" !tag && tag2", "tag && tag2")]
        public void Parse_should_correctly_parse_not_contains_filter_expression(string expression, string expectedKey)
        {
            var filter = TagFilterExpressionHelpers.Parse(expression);
            filter.Should().BeOfType<NotContainsTagFilter>();
            filter.Should().BeEquivalentTo(new NotContainsTagFilter(expectedKey));
        }
    }
}