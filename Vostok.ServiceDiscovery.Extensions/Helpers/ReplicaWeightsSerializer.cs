using System;
using JetBrains.Annotations;
using Vostok.Commons.Binary;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    internal static class ReplicaWeightsSerializer
    {
        [NotNull]
        public static ReplicaWeights Deserialize([NotNull] byte[] data)
        {
            var reader = new BinaryBufferReader(data, 0);

            var count = reader.ReadInt32();

            var weights = new ReplicaWeights(count);

            for (var i = 0; i < count; i++)
            {
                var replica = ReadReplica(reader);
                var weight = ReadWeight(reader);

                weights[replica] = weight;
            }

            return weights;
        }

        [NotNull]
        private static Uri ReadReplica([NotNull] BinaryBufferReader reader)
        {
            var host = reader.ReadString();
            var port = reader.ReadInt32();

            return new Uri($"http://{host}:{port}");
        }

        [NotNull]
        private static ReplicaWeight ReadWeight([NotNull] BinaryBufferReader reader)
        {
            var value = reader.ReadDouble();
            var timestamp = reader.ReadDateTime();

            return new ReplicaWeight(value, timestamp);
        }
    }
}