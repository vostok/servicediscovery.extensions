using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions
{
    [PublicAPI]
    public static class IServiceDiscoveryManagerExtensions
    {
        public static async Task<bool> AddToBlacklistAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] addReplicas)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => ApplicationInfoUpdater.AddToBlacklist(addReplicas, properties))
                .ConfigureAwait(false);
        }

        public static async Task<bool> RemoveFromBlacklistAsync(this IServiceDiscoveryManager serviceDiscoveryManager, string environment, string application, params Uri[] removeReplicas)
        {
            return await serviceDiscoveryManager.TryUpdateApplicationPropertiesAsync(
                    environment,
                    application,
                    properties => ApplicationInfoUpdater.RemoveFromBlacklist(removeReplicas, properties))
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
    }
}