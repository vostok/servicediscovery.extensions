namespace Vostok.ServiceDiscovery.Extensions
{
    internal static class PropertyConstants
    {
        public const string ExternalUrlProperty = "ExternalUrl";
        public const string BlacklistProperty = "Blacklist";
        public const string DesiredTopologyProperty = "DesiredTopology";
        public const string WeightsProperty = "weights";
        public const string WeightsVersionProperty = "weightsVersion";
        public const string BlacklistItemSeparator = "|";
        public const string DesiredTopologyItemSeparator = BlacklistItemSeparator;
    }
}