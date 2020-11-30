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

        /// <summary>
        /// todo
        /// </summary>
        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName)
            => PropertiesHelper.GetReplicaTags(properties, replicaName);

        /// <summary>
        /// todo
        /// </summary>
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

        /// <summary>
        /// todo
        /// </summary>
        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetPersistentReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName, TagCollection tags)
            => properties.SetReplicaTags(replicaName, ReplicaTagKind.Persistent, tags);

        /// <summary>
        /// todo
        /// </summary>
        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetEphemeralReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName, TagCollection tags)
            => properties.SetReplicaTags(replicaName, ReplicaTagKind.Ephemeral, tags);

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveExternalUrl([NotNull] this IApplicationInfoProperties properties)
            => properties.Remove(PropertyConstants.ExternalUrlProperty);

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveBlacklist([NotNull] this IApplicationInfoProperties properties)
            => properties.Remove(PropertyConstants.BlacklistProperty);
    }
}