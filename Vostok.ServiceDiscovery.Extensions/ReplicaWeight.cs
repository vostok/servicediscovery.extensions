using System;
using JetBrains.Annotations;

namespace Vostok.ServiceDiscovery.Extensions
{
    /// <summary>
    /// Represents a relative weight of a single replica in a cluster.
    /// </summary>
    [PublicAPI]
    public class ReplicaWeight
    {
        public ReplicaWeight(double value, DateTime timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Weight value. Must be in the [0; 1] range.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// A UTC timestamp representing the moment when this weight was computed.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}