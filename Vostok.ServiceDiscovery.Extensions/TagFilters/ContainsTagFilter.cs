using System;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions.Models;

namespace Vostok.ServiceDiscovery.Extensions.TagFilters
{
    /// <summary>
    /// An implementation of <see cref="ITagFilter" /> that implements simple "contains" expression.
    /// </summary>
    [PublicAPI]
    public class ContainsTagFilter : ITagFilter
    {
        public ContainsTagFilter([NotNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key));

            Key = key.Trim();
        }

        [NotNull]
        public string Key { get; }

        /// <summary>
        /// <para> Returns true if <see cref="ContainsTagFilter.Key" /> is present in <paramref name="collection" />. </para>
        /// </summary>
        public bool Matches(TagCollection collection)
            => collection != null
               && collection.ContainsKey(Key);

        public override string ToString() => Key;
    }
}