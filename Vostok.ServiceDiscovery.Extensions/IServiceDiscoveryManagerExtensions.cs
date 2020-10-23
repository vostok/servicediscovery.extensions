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
        public static async Task<bool> ModifyReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.ModifyReplicaTags(replicaName, modifyTagsFunc))
                .ConfigureAwait(false);
        }

        public static async Task<bool> AddReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, TagCollection tags)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.AddReplicaTags(replicaName, tags))
                .ConfigureAwait(false);
        }

        public static async Task<bool> RemoveReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tagKeysToRemove)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveReplicaTags(replicaName, tagKeysToRemove))
                .ConfigureAwait(false);
        }

        public static async Task<bool> ClearReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.ClearReplicaTags(replicaName))
                .ConfigureAwait(false);
        }
        
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
