using System;
using System.Collections.Generic;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Extensions.Helpers;

namespace Vostok.ServiceDiscovery.Extensions
{
    public static class IServiceTopologyExtensions
    {
        public static Uri GetExternalUrl(this IServiceTopology topology)
        {
            return topology.Data.TryGetValue(TopologyDataConstants.ExternalUrlField, out var value) && Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
        }

        public static IList<Uri> GetBlacklist(this IServiceTopology topology)
        {
            if (!topology.Data.TryGetValue(TopologyDataConstants.BlacklistField, out var blacklist))
                return new Uri[] {};
            return blacklist.Split(TopologyDataConstants.BlacklistItemSeparator[0]).ToParsedUriList();
        }
    }
}