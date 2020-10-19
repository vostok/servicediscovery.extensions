using System;
using System.Collections.Generic;
using System.Linq;
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
        public static async Task<bool> ModifyReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, Func<ITag[], ITag[]> modifyTagsFunc)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.ModifyReplicaTags(replicaName, modifyTagsFunc))
                .ConfigureAwait(false);
        }

        public static async Task<bool> AddReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, params ITag[] tags)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, t => Add(t, tags));

        public static async Task<bool> RemoveReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, params string[] tags)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, t => Remove(t, tags));
        
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
        
        private static ITag[] Add(ITag[] existTags, ITag[] newTags)
        {
            var hashSet = new HashSet<string>(newTags.Select(x => x.Key));
            return existTags
                .Where(x => !hashSet.Contains(x.Key))
                .Concat(ReplicaTagsHelpers.Distinct(newTags))
                .ToArray();
        }

        private static ITag[] Remove(ITag[] existTags, string[] tagsToRemove)
        {
            var hashSet = new HashSet<string>(tagsToRemove);
            return existTags
                .Where(x => !hashSet.Contains(x.Key))
                .ToArray();
        }
    }
}