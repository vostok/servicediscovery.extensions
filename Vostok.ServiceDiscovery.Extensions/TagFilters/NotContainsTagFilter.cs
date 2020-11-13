using System;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions.Models;

namespace Vostok.ServiceDiscovery.Extensions.TagFilters
{
    /// <summary>
    /// An implementation of <see cref="ITagFilter" /> that implements simple "not contains" expression.
    /// </summary>
    [PublicAPI]
    public class NotContainsTagFilter : ITagFilter
    {
        public NotContainsTagFilter([NotNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key));

            Key = key.Trim();
        }

        [NotNull]
        public string Key { get; }

        /// <summary>
        /// Returns true if <see cref="NotContainsTagFilter.Key" /> is not present in <paramref name="collection" /> or <paramref name="collection" /> is null.
        /// </summary>
        public bool Matches(TagCollection collection)
            => collection == null
               || !collection.ContainsKey(Key);

        public override string ToString() => "!" + Key;
    }
}