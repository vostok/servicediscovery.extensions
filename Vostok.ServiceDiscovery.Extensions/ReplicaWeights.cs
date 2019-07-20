using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.ServiceDiscovery.Extensions
{
    /// <summary>
    /// A collection of replica weights for a topology.
    /// </summary>
    [PublicAPI]
    public class ReplicaWeights : Dictionary<Uri, ReplicaWeight>
    {
        public ReplicaWeights(int capacity = 4)
            : base(capacity, ReplicaComparer.Instance)
        {
        }
    }
}