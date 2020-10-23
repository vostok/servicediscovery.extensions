using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Abstractions.Models;
using Vostok.ServiceDiscovery.Extensions.Helpers;

namespace Vostok.ServiceDiscovery.Extensions
{
    [PublicAPI]
    public static class IApplicationInfoPropertiesExtensions
    {
        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetExternalUrl(properties);

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetBlacklist(properties);

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetReplicaWeights(properties);

        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName)
            => PropertiesHelper.GetReplicaTags(properties, replicaName);

        [NotNull]
        public static IReadOnlyDictionary<string, TagCollection> GetTags([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetTags(properties);

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetExternalUrl([NotNull] this IApplicationInfoProperties properties, [NotNull] Uri externalUrl)
            => properties.Set(PropertyConstants.ExternalUrlProperty, externalUrl.ToString());

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetBlacklist([NotNull] this IApplicationInfoProperties properties, [NotNull] IEnumerable<Uri> blacklist)
            => properties.Set(PropertyConstants.BlacklistProperty, string.Join(PropertyConstants.BlacklistItemSeparator, blacklist));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, TagCollection tags)
            => tags.Count > 0
                ? properties.Set(GetPersistentReplicaTagsPropertyKey(replicaName), tags.ToString())
                : properties.Remove(GetPersistentReplicaTagsPropertyKey(replicaName));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveExternalUrl([NotNull] this IApplicationInfoProperties properties)
            => properties.Remove(PropertyConstants.ExternalUrlProperty);

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveBlacklist([NotNull] this IApplicationInfoProperties properties)
            => properties.Remove(PropertyConstants.BlacklistProperty);

        [NotNull]
        private static string GetPersistentReplicaTagsPropertyKey(string replicaName)
            => TagPropertyHelpers.FormatName(replicaName, "persistent");
    }
}