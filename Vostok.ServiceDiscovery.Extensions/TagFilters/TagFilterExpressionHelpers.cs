using System;
using JetBrains.Annotations;

namespace Vostok.ServiceDiscovery.Extensions.TagFilters
{
    [PublicAPI]
    public static class TagFilterExpressionHelpers
    {
        public static ITagFilter Parse(string input)
        {
            var equalsIndex = input.IndexOf("==", StringComparison.Ordinal);
            if (equalsIndex >= 0)
            {
                var key = input.Substring(0, equalsIndex);
                if (!string.IsNullOrWhiteSpace(key))
                    return new EqualsTagFilter(key, input.Substring(equalsIndex + 2));
            }

            var notEqualsIndex = input.IndexOf("!=", StringComparison.Ordinal);
            if (notEqualsIndex >= 0)
            {
                var key = input.Substring(0, notEqualsIndex);
                if (!string.IsNullOrWhiteSpace(key))
                    return new NotEqualsTagFilter(key, input.Substring(notEqualsIndex + 2));
            }

            input = input.Trim();
            if (input.StartsWith("!"))
                return new NotContainsTagFilter(input.Substring(1));
            return new ContainsTagFilter(input);
        }
    }
}