using System;
using System.Collections.Generic;

namespace Vostok.ServiceDiscovery.Extensions
{
    public static class TopologyDataExtensions
    {
        public static void SetExternalUrl(this Dictionary<string, string> topologyData, Uri externalUrl)
        {
            if (externalUrl == null)
            {
                topologyData.Remove(TopologyDataConstants.ExternalUrlField);
                return;
            }
            topologyData[TopologyDataConstants.ExternalUrlField] = externalUrl.ToString();
        }

        public static void SetBlacklist(this Dictionary<string, string> topologyData, IEnumerable<Uri> blacklist)
        {
            topologyData[TopologyDataConstants.BlacklistField] = string.Join(TopologyDataConstants.BlacklistItemSeparator, blacklist);
        }
    }
}