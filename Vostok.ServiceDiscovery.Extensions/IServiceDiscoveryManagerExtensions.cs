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
        /// <inheritdoc cref="AddReplicaTags"/>
        /// todo
        public static async Task<bool> AddPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, tags);

        /// <inheritdoc cref="AddReplicaTags"/>
        /// todo
        public static async Task<bool> AddEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
            => await serviceDiscoveryManager.AddReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral, tags);

        /// <inheritdoc cref="RemoveReplicaTags"/>
        /// todo
        public static async Task<bool> RemovePersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, tagKeysToRemove);

        /// <inheritdoc cref="RemoveReplicaTags"/>
        /// todo
        public static async Task<bool> RemoveEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.RemoveReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral, tagKeysToRemove);

        /// <inheritdoc cref="ClearReplicaTags"/>
        /// todo
        public static async Task<bool> ClearPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent);

        /// <inheritdoc cref="ClearReplicaTags"/>
        /// todo
        public static async Task<bool> ClearEphemeralReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
            => await serviceDiscoveryManager.ClearReplicaTags(environment, application, replicaName, ReplicaTagKind.Ephemeral);
        
        /// <inheritdoc cref="ModifyReplicaTags"/>
        /// todo
        public static async Task<bool> ModifyPersistentReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, ReplicaTagKind.Persistent, modifyTagsFunc);

        /// <inheritdoc cref="ModifyReplicaTags"/>
        /// todo
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
        /// todo
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
        /// todo
        /// </summary>
        private static async Task<bool> RemoveReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, ReplicaTagKind replicaTagKind, IEnumerable<string> tagKeysToRemove)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveReplicaTags(replicaName, replicaTagKind, tagKeysToRemove))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// todo
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
        /// todo
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
