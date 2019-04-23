using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions.Tests
{
    internal class TopologyDataTestBase
    {
        protected IServiceTopology Topology;
        protected Dictionary<string,string> TopologyData;

        [SetUp]
        public void SetUp()
        {
            Topology = Substitute.For<IServiceTopology>();
            TopologyData = new Dictionary<string, string>();
            Topology.Properties.Returns(TopologyData);
        }
    }
}