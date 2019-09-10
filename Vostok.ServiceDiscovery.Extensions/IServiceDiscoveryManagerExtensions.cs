using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions
{
    public static class IServiceDiscoveryManagerExtensions
    {
        public static async Task<bool> AddToBlacklist(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] replicaUri)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties =>
                    {
                        var blacklist = new HashSet<Uri>(properties.GetBlacklist());
                        if (blacklist.IsSupersetOf(replicaUri))
                            return properties;

                        var newBlackList = blacklist.Concat(replicaUri);
                        return properties.SetBlacklist(newBlackList);
                    })
                .ConfigureAwait(false);
        }

        public static async Task<bool> RemoveFromBlacklist(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] replicaUri)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties =>
                    {
                        var blacklist = new HashSet<Uri>(properties.GetBlacklist());
                        if (!replicaUri.Intersect(blacklist).Any())
                            return properties;

                        blacklist.ExceptWith(replicaUri);
                        return properties.SetBlacklist(blacklist);
                    })
                .ConfigureAwait(false);
        }

        public static async Task<bool> SetExternalUrl(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, Uri externalUrl)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => properties.SetExternalUrl(externalUrl))
                .ConfigureAwait(false);
        }
    }
}