using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions
{
    internal static class ApplicationInfoUpdater
    {
        public static IApplicationInfoProperties AddToBlacklist(Uri[] add, IApplicationInfoProperties properties)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (blacklist.IsSupersetOf(add))
                return properties;

            var newBlackList = blacklist.Concat(add);
            return properties.SetBlacklist(newBlackList);
        }

        public static IApplicationInfoProperties RemoveFromBlacklist(Uri[] remove, IApplicationInfoProperties properties)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (!remove.Intersect(blacklist).Any())
                return properties;

            blacklist.ExceptWith(remove);
            return properties.SetBlacklist(blacklist);
        }
    }
}