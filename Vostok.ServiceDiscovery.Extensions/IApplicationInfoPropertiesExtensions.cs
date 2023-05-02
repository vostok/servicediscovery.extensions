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

        [NotNull]
        public static Uri[] GetHostingTopology([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetHostingTopology(properties);

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IApplicationInfoProperties properties)
            => PropertiesHelper.GetReplicaWeights(properties);

        /// <summary>
        /// Returns <see cref="TagCollection"/> with all tag kinds by given <paramref name="replicaName"/>.
        /// </summary>
        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName)
            => PropertiesHelper.GetReplicaTags(properties, replicaName);

        /// <summary>
        /// Returns dictionary of <see cref="TagCollection"/> by application replicas with all tag kinds.
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
            => properties.Set(PropertyConstants.BlacklistProperty, string.Join(PropertyConstants.UriListSeparator, blacklist));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetHostingTopology([NotNull] this IApplicationInfoProperties properties, [NotNull] IEnumerable<Uri> hostingTopology)
            => properties.Set(PropertyConstants.HostingTopologyProperty, string.Join(PropertyConstants.UriListSeparator, hostingTopology));

        /// <summary>
        /// <para>Sets given <paramref name="tags"/> for given <paramref name="replicaName"/> to <see cref="IApplicationInfoProperties"/> and gives them <see cref="ReplicaTagKind.Persistent"/> kind.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static IApplicationInfoProperties SetPersistentReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName, TagCollection tags)
            => properties.SetReplicaTags(replicaName, ReplicaTagKind.Persistent, tags);
        
        /// <summary>
        /// <para>Sets given <paramref name="tags"/> for given <paramref name="replicaName"/> to <see cref="IApplicationInfoProperties"/> and gives them <see cref="ReplicaTagKind.Ephemeral"/> kind.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
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
        
        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveHostingTopology([NotNull] this IApplicationInfoProperties properties)
            => properties.Remove(PropertyConstants.HostingTopologyProperty);
    }
}