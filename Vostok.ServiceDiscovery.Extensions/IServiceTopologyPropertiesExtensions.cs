using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Extensions.Helpers;

namespace Vostok.ServiceDiscovery.Extensions
{
    [PublicAPI]
    public static class IServiceTopologyPropertiesExtensions
    {
        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] this IServiceTopologyProperties properties)
            => PropertiesHelper.GetExternalUrl(properties);

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IServiceTopologyProperties properties)
            => PropertiesHelper.GetBlacklist(properties);

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IServiceTopologyProperties properties)
            => PropertiesHelper.GetReplicaWeights(properties);

        [NotNull]
        public static ITag[] GetReplicaTags([NotNull] this IServiceTopologyProperties properties, string replicaName)
            => PropertiesHelper.GetReplicaTags(properties, replicaName);

        [NotNull]
        public static IReadOnlyDictionary<string, ITag[]> GetTags([NotNull] this IServiceTopologyProperties properties)
            => PropertiesHelper.GetTags(properties);
    }
}