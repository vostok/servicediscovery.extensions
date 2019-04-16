using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.ServiceDiscovery.Extensions.Tests
{
    internal class IServiceTopologyExtensions_Tests : TopologyDataTestBase
    {
        [Test]
        public void Should_return_null_when_no_external_url()
        {
            Topology.GetExternalUrl().Should().BeNull();
        }
        
        [Test]
        public void Should_return_null_when_external_url_is_invalid()
        {
            TopologyData[TopologyDataConstants.ExternalUrlField] = "!@#$%";
            Topology.GetExternalUrl().Should().BeNull();
        }

        [Test]
        public void Should_return_empty_list_when_no_blacklist()
        {
            Topology.GetBlacklist().Should().BeEmpty();
        }

        [Test]
        public void Should_ignore_invalid_urls_in_blacklist()
        {
            TopologyData[TopologyDataConstants.BlacklistField] = $"!@$%{TopologyDataConstants.BlacklistItemSeparator}http://example.com";
            Topology.GetBlacklist().Should().Equal(new Uri("http://example.com"));
        }
    }
}