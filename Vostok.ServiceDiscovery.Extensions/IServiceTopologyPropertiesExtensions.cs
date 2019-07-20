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
        public const string ExternalUrlProperty = "ExternalUrl";
        public const string BlacklistProperty = "Blacklist";
        public const string WeightsProperty = "weights";
        public const string WeightsVersionProperty = "weightsVersion";

        public const string BlacklistItemSeparator = "|";

        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] this IServiceTopologyProperties properties)
            => properties.TryGetValue(ExternalUrlProperty, out var value)
                ? UrlParser.Parse(value)
                : null;

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IServiceTopologyProperties properties)
            => properties.TryGetValue(BlacklistProperty, out var value)
                ? UrlParser.Parse(value.Split(BlacklistItemSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                : Array.Empty<Uri>();

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IServiceTopologyProperties properties)
        {
            if (!properties.TryGetValue(WeightsVersionProperty, out var weightsVersion) ||
                !properties.TryGetValue(WeightsProperty, out var weightsData) || 
                !int.TryParse(weightsVersion, out var numericVersion) || numericVersion != 0)
                return null;

            return ReplicaWeightsSerializer.Deserialize(Convert.FromBase64String(weightsData));
        }

        [NotNull]
        public static IServiceTopologyProperties SetExternalUrl([NotNull] this IServiceTopologyProperties properties, [NotNull] Uri externalUrl)
            => properties.Set(ExternalUrlProperty, externalUrl.ToString());

        [NotNull]
        public static IServiceTopologyProperties SetBlacklist([NotNull] this IServiceTopologyProperties properties, [NotNull] IEnumerable<Uri> blacklist)
            => properties.Set(BlacklistProperty, string.Join(BlacklistItemSeparator, blacklist));

        [NotNull]
        public static IServiceTopologyProperties RemoveExternalUrl([NotNull] this IServiceTopologyProperties properties)
            => properties.Remove(ExternalUrlProperty);

        [NotNull]
        public static IServiceTopologyProperties RemoveBlacklist([NotNull] this IServiceTopologyProperties properties)
            => properties.Remove(BlacklistProperty);
    }
}
