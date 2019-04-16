using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.ServiceDiscovery.Extensions.Tests
{
    [TestFixture]
    internal class Extensions_FunctionalTests : TopologyDataTestBase
    {
        [Test]
        public void Should_set_and_get_externalUrl_when_not_null()
        {
            var url = new Uri("http://example.com/");
            
            TopologyData.SetExternalUrl(url);
            Topology.GetExternalUrl().Should().Be(url);
        }

        [Test]
        public void Should_set_and_get_externalUrl_when_null()
        {
            TopologyData.SetExternalUrl(new Uri("http://example.com/"));
            
            TopologyData.SetExternalUrl(null);
            Topology.GetExternalUrl().Should().Be(null);
        }

        [Test]
        public void Should_set_and_get_blacklist()
        {
            var urls = new[] {new Uri("http://example.com/"), new Uri("http://example2.com/")};
            
            TopologyData.SetBlacklist(urls);
            Topology.GetBlacklist().Should().Equal(urls);
        }
    }
}