using System;
using System.Collections.Generic;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    internal static class StringEnumerableExtensions
    {
        public static List<Uri> ToParsedUriList(this IEnumerable<string> uriList)
        {
            var result = new List<Uri>();
            foreach (var uri in uriList)
            {
                if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
                    continue;
                result.Add(parsedUri);
            }
            return result;
        }
    }
}