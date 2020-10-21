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

        public static async Task<bool> AddReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<Tag> tags)
            => await serviceDiscoveryManager.ModifyReplicaTags(environment, application, replicaName, t => Add(t, tags));

        public static async Task<bool> RemoveReplicaTags(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, string replicaName, IEnumerable<string> tags)
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

        public static async Task<bool> RemoveExternalUrlAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.RemoveExternalUrl())
                .ConfigureAwait(false);
        }        
        
        private static TagCollection Add(TagCollection existTags, IEnumerable<Tag> newTags)
        {
            foreach (var newTag in newTags)
                existTags[newTag.Key] = newTag.Value;
            return existTags;
        }

        private static TagCollection Remove(TagCollection existTags, IEnumerable<string> tagsToRemove)
        {
            foreach (var tagToRemove in tagsToRemove)
                existTags.Remove(tagToRemove);
            return existTags;
        }
    }
}
