using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Url;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions
{
    [PublicAPI]
    public static class IServiceTopologyPropertiesExtensions
    {
        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] this IServiceTopologyProperties properties)
            => properties.TryGetValue(PropertyConstants.ExternalUrlProperty, out var value)
                ? UrlParser.Parse(value)
                : null;

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IServiceTopologyProperties properties)
            => properties.TryGetValue(PropertyConstants.BlacklistProperty, out var value)
                ? UrlParser.Parse(value.Split(PropertyConstants.BlacklistItemSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                : Array.Empty<Uri>();

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IServiceTopologyProperties properties)
        {
            if (!properties.TryGetValue(PropertyConstants.WeightsVersionProperty, out var weightsVersion) ||
                !properties.TryGetValue(PropertyConstants.WeightsProperty, out var weightsData) || 
                !int.TryParse(weightsVersion, out var numericVersion) || numericVersion != 0)
                return null;

            return ReplicaWeightsSerializer.Deserialize(Convert.FromBase64String(weightsData));
        }

        [NotNull]
        public static IServiceTopologyProperties SetExternalUrl([NotNull] this IServiceTopologyProperties properties, [NotNull] Uri externalUrl)
            => properties.Set(PropertyConstants.ExternalUrlProperty, externalUrl.ToString());

        [NotNull]
        public static IServiceTopologyProperties SetBlacklist([NotNull] this IServiceTopologyProperties properties, [NotNull] IEnumerable<Uri> blacklist)
            => properties.Set(PropertyConstants.BlacklistProperty, string.Join(PropertyConstants.BlacklistItemSeparator, blacklist));

        [NotNull]
        public static IServiceTopologyProperties RemoveExternalUrl([NotNull] this IServiceTopologyProperties properties)
            => properties.Remove(PropertyConstants.ExternalUrlProperty);

        [NotNull]
        public static IServiceTopologyProperties RemoveBlacklist([NotNull] this IServiceTopologyProperties properties)
            => properties.Remove(PropertyConstants.BlacklistProperty);
    }
}
