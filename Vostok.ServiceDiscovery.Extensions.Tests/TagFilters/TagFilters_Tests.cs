using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.ServiceDiscovery.Abstractions.Models;
using Vostok.ServiceDiscovery.Extensions.TagFilters;

namespace Vostok.ServiceDiscovery.Extensions.Tests.TagFilters
{
    [TestFixture]
    internal class TagFilters_Tests
    {
        [Test]
        [TestCase("", "value")]
        [TestCase("  ", "value")]
        [TestCase(null, "value")]
        public void EqualsFilter_ctor_should_throw_argument_out_of_range_when_arguments_not_valid(string key, string value)
        {
            var action = (Action)(() => new EqualsTagFilter(key, value));
            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        [TestCase(" tag ", " value ")]
        [TestCase("tag", "  ")]
        public void EqualsFilter_ctor_should_trim_valid_arguments(string key, string value)
            => new EqualsTagFilter(key, value).Should().BeEquivalentTo(new EqualsTagFilter(key.Trim(), value.Trim()));

        [Test]
        [TestCase("", "value")]
        [TestCase("  ", "value")]
        [TestCase(null, "value")]
        public void NotEqualsFilter_ctor_should_throw_argument_out_of_range_when_arguments_not_valid(string key, string value)
        {
            var action = (Action)(() => new NotEqualsTagFilter(key, value));
            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        [TestCase(" tag ", " value ")]
        [TestCase("tag", "  ")]
        public void NotEqualsFilter_ctor_should_trim_valid_arguments(string key, string value)
            => new NotEqualsTagFilter(key, value).Should().BeEquivalentTo(new NotEqualsTagFilter(key.Trim(), value.Trim()));

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void ContainsFilter_ctor_should_throw_argument_out_of_range_when_arguments_not_valid(string key)
        {
            var action = (Action)(() => new ContainsTagFilter(key));
            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ContainsFilter_ctor_should_trim_valid_arguments()
            => new ContainsTagFilter(" tag ").Should().BeEquivalentTo(new ContainsTagFilter("tag"));

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void NotContainsFilter_should_throw_argument_out_of_range_when_arguments_not_valid(string key)
        {
            var action = (Action)(() => new NotContainsTagFilter(key));
            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        public void NotContainsFilter_ctor_should_trim_valid_arguments()
            => new NotContainsTagFilter(" tag ").Should().BeEquivalentTo(new NotContainsTagFilter("tag"));

        [Test]
        [TestCase("tag", "")]
        [TestCase("tag", "value")]
        public void EqualsFilter_matches_should_return_true(string key, string value)
        {
            var filter = new EqualsTagFilter(key, value);
            filter.Matches(new TagCollection {{key, value}}).Should().BeTrue();
        }

        [Test]
        public void EqualsFilter_matches_should_return_false_when_collection_is_null()
        {
            var filter = new EqualsTagFilter("tag", "value");
            filter.Matches(null).Should().BeFalse();
        }

        [Test]
        public void EqualsFilter_matches_should_return_false_when_collection_has_no_key()
        {
            var filter = new EqualsTagFilter("tag", "value");
            filter.Matches(new TagCollection {"tag1", {"tag2", "value2"}}).Should().BeFalse();
        }

        [Test]
        [TestCase("tag", "")]
        [TestCase("tag", "value")]
        public void EqualsFilter_matches_should_return_false_when_value_is_not_equals(string key, string value)
        {
            var filter = new EqualsTagFilter(key, value);
            filter.Matches(new TagCollection {{key, Guid.NewGuid().ToString()}}).Should().BeFalse();
        }

        [Test]
        [TestCase("tag", "", "tag==")]
        [TestCase("tag", "value", "tag==value")]
        public void EqualsFilter_ToString_should_return_correct_string(string key, string value, string expected)
            => new EqualsTagFilter(key, value).ToString().Should().Be(expected);

        [Test]
        [TestCase("tag", "")]
        [TestCase("tag", "value")]
        public void NotEqualsFilter_matches_should_return_false(string key, string value)
        {
            var filter = new NotEqualsTagFilter(key, value);
            filter.Matches(new TagCollection {{key, value}}).Should().BeFalse();
        }

        [Test]
        public void NotEqualsFilter_matches_should_return_false_when_collection_is_null()
        {
            var filter = new NotEqualsTagFilter("tag", "value");
            filter.Matches(null).Should().BeFalse();
        }

        [Test]
        public void NotEqualsFilter_matches_should_return_false_when_collection_has_no_key()
        {
            var filter = new NotEqualsTagFilter("tag", "value");
            filter.Matches(new TagCollection {"tag1", {"tag2", "value2"}}).Should().BeFalse();
        }

        [Test]
        [TestCase("tag", "")]
        [TestCase("tag", "value")]
        public void NotEqualsFilter_matches_should_return_true_when_value_is_not_equals(string key, string value)
        {
            var filter = new NotEqualsTagFilter(key, value);
            filter.Matches(new TagCollection {{key, Guid.NewGuid().ToString()}}).Should().BeTrue();
        }

        [Test]
        [TestCase("tag", "", "tag!=")]
        [TestCase("tag", "value", "tag!=value")]
        public void NotEqualsFilter_ToString_should_return_correct_string(string key, string value, string expected)
            => new NotEqualsTagFilter(key, value).ToString().Should().Be(expected);

        [Test]
        public void ContainsFilter_matches_should_return_true_when_collection_has_key()
        {
            var filter = new ContainsTagFilter("tag");
            filter.Matches(new TagCollection {"tag"}).Should().BeTrue();
        }

        [Test]
        public void ContainsFilter_matches_should_return_true_when_collection_has_key_with_some_value()
        {
            var filter = new ContainsTagFilter("tag");
            filter.Matches(new TagCollection {{"tag", Guid.NewGuid().ToString()}}).Should().BeTrue();
        }

        [Test]
        public void ContainsFilter_matches_should_return_false_when_collection_is_null_or_empty()
        {
            var filter = new ContainsTagFilter("tag");
            filter.Matches(null).Should().BeFalse();
            filter.Matches(new TagCollection()).Should().BeFalse();
        }

        [Test]
        public void ContainsTagFilter_matches_should_return_false_when_collection_has_no_key()
        {
            var filter = new ContainsTagFilter("tag");
            filter.Matches(new TagCollection {"tag1", {"tag2", "value2"}}).Should().BeFalse();
        }

        [Test]
        public void ContainsTagFilter_ToString_should_return_correct_string()
            => new ContainsTagFilter("tag").ToString().Should().Be("tag");

        [Test]
        public void NotContainsFilter_matches_should_return_false_when_collection_has_key()
        {
            var filter = new NotContainsTagFilter("tag");
            filter.Matches(new TagCollection {"tag"}).Should().BeFalse();
        }

        [Test]
        public void NotContainsFilter_matches_should_return_false_when_collection_has_key_with_some_value()
        {
            var filter = new NotContainsTagFilter("tag");
            filter.Matches(new TagCollection {{"tag", Guid.NewGuid().ToString()}}).Should().BeFalse();
        }

        [Test]
        public void NotContainsFilter_matches_should_return_true_when_collection_is_null_or_empty()
        {
            var filter = new NotContainsTagFilter("tag");
            filter.Matches(null).Should().BeTrue();
            filter.Matches(new TagCollection()).Should().BeTrue();
        }

        [Test]
        public void NotContainsTagFilter_matches_should_return_true_when_collection_has_no_key()
        {
            var filter = new NotContainsTagFilter("tag");
            filter.Matches(new TagCollection {"tag1", {"tag2", "value2"}}).Should().BeTrue();
        }

        [Test]
        public void NotContainsTagFilter_ToString_should_return_correct_string()
            => new NotContainsTagFilter("tag").ToString().Should().Be("!tag");
    }
}