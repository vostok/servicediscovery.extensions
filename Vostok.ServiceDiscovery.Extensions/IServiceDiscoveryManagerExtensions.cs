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
        /// <para><inheritdoc cref="AddReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> AddPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, tags);

        /// <summary>
        /// <para><inheritdoc cref="AddReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> AddEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral, tags);

        /// <summary>
        /// <para><inheritdoc cref="RemoveReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> RemovePersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, tagKeysToRemove);

        /// <summary>
        /// <para><inheritdoc cref="RemoveReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> RemoveEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral, tagKeysToRemove);

        /// <summary>
        /// <para><inheritdoc cref="ClearReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ClearPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent);

        /// <summary>
        /// <para><inheritdoc cref="ClearReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ClearEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral);
        
        /// <summary>
        /// <para><inheritdoc cref="ModifyReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Persistent"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ModifyPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, modifyTagsFunc);

        /// <summary>
        /// <para><inheritdoc cref="ModifyReplicaTags"/></para>
        /// <para>This method modifies <see cref="ReplicaTagKind.Ephemeral"/> tags.</para>
        /// <para>See <see cref="ReplicaTagKind"/> for more information about different tags kinds.</para>
        /// </summary>
        public static async Task<bool> ModifyEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral, modifyTagsFunc);

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
        private static async Task<bool> AddReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, TagCollection tags)
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
        private static async Task<bool> RemoveReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, IEnumerable<string> tagKeys)
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
        private static async Task<bool> ClearReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind)
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
        private static async Task<bool> ModifyReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, Func<TagCollection, TagCollection> modifyTagsFunc)
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
