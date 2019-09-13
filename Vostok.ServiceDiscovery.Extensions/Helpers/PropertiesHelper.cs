using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Url;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    internal static class PropertiesHelper
    {
        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] IReadOnlyDictionary<string, string> properties)
            => properties.TryGetValue(PropertyConstants.ExternalUrlProperty, out var value)
                ? UrlParser.Parse(value)
                : null;

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties.TryGetValue(PropertyConstants.BlacklistProperty, out var value)
                ? UrlParser.Parse(value.Split(PropertyConstants.BlacklistItemSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                : Array.Empty<Uri>();

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IReadOnlyDictionary<string, string> properties)
        {
            if (!properties.TryGetValue(PropertyConstants.WeightsVersionProperty, out var weightsVersion) ||
                !properties.TryGetValue(PropertyConstants.WeightsProperty, out var weightsData) ||
                !int.TryParse(weightsVersion, out var numericVersion) || numericVersion != 0)
                return null;

            return ReplicaWeightsSerializer.Deserialize(Convert.FromBase64String(weightsData));
        }

        [Pure]
        public static IApplicationInfoProperties AddToBlacklist(this IApplicationInfoProperties properties, Uri[] replicasToAdd)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (blacklist.IsSupersetOf(replicasToAdd))
                return properties;

            var newBlackList = blacklist.Concat(replicasToAdd);
            return properties.SetBlacklist(newBlackList);
        }

        [Pure]
        public static IApplicationInfoProperties RemoveFromBlacklist(this IApplicationInfoProperties properties, Uri[] replicasToRemove)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (!replicasToRemove.Intersect(blacklist).Any())
                return properties;

            blacklist.ExceptWith(replicasToRemove);
            return properties.SetBlacklist(blacklist);
        }
    }
}