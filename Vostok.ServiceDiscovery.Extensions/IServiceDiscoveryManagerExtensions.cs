using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Abstractions.Models;
using Vostok.ServiceDiscovery.Extensions.Helpers;

namespace Vostok.ServiceDiscovery.Extensions
{
    [PublicAPI]
    public static class IServiceDiscoveryManagerExtensions
    {
        /// <summary>
        /// <para><inheritdoc cref="AddReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> AddPersistentReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Persistent, tags).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="AddReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> AddEphemeralReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Ephemeral, tags).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="RemoveReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> RemovePersistentReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Persistent, tagKeysToRemove).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="RemoveReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> RemoveEphemeralReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Ephemeral, tagKeysToRemove).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="ClearReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ClearPersistentReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Persistent).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="ClearReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ClearEphemeralReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Ephemeral).ConfigureAwait(false);
        
        /// <summary>
        /// <para><inheritdoc cref="ModifyReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ModifyPersistentReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
            => await serviceDiscoveryManager.ModifyReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Persistent, modifyTagsFunc).ConfigureAwait(false);

        /// <summary>
        /// <para><inheritdoc cref="ModifyReplicaTagsAsync"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ModifyEphemeralReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
            => await serviceDiscoveryManager.ModifyReplicaTagsAsync(environment, application, replicaName, ReplicaTagKind.Ephemeral, modifyTagsFunc).ConfigureAwait(false);

        public static async Task<bool> AddToBlacklistAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] replicasToAdd)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.AddToBlacklist(replicasToAdd))
                .ConfigureAwait(false);
        }

        public static async Task<bool> RemoveFromBlacklistAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] replicasToRemove)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveFromBlacklist(replicasToRemove))
                .ConfigureAwait(false);
        }

        public static async Task<bool> SetExternalUrlAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, Uri externalUrl)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.SetExternalUrl(externalUrl))
                .ConfigureAwait(false);
        }
        
        /// <summary>
        /// Add given <paramref name="tags"/> by <paramref name="replicaName"/> to the <see cref="IApplicationInfoProperties"/> of the given <paramref name="application"/> and updates it in ServiceDiscovery.
        /// </summary>
        private static async Task<bool> AddReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, TagCollection tags)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.AddReplicaTags(replicaName, replicaTagKind, tags))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove given <paramref name="tagKeys"/> by <paramref name="replicaName"/> in the <see cref="IApplicationInfoProperties"/> of the given <paramref name="application"/> and updates it in ServiceDiscovery.
        /// </summary>
        private static async Task<bool> RemoveReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, IEnumerable<string> tagKeys)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveReplicaTags(replicaName, replicaTagKind, tagKeys))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Clear all tags by given <paramref name="replicaName"/> in the <see cref="IApplicationInfoProperties"/> of the given <paramref name="application"/> and updates it in ServiceDiscovery.
        /// </summary>
        private static async Task<bool> ClearReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.ClearReplicaTags(replicaName, replicaTagKind))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Modifies tags by given <paramref name="replicaName"/> in the <see cref="IApplicationInfoProperties"/> of the given <paramref name="application"/> using <paramref name="modifyTagsFunc"/> and updates it in ServiceDiscovery.
        /// </summary>
        private static async Task<bool> ModifyReplicaTagsAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, Func<TagCollection, TagCollection> modifyTagsFunc)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.ModifyReplicaTags(replicaName, replicaTagKind, modifyTagsFunc))
                .ConfigureAwait(false);
        }
		
		public static async Task<bool> RemoveExternalUrlAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveExternalUrl())
                .ConfigureAwait(false);
        }
    }
}
