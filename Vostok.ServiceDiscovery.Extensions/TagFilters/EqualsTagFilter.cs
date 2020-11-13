using System;
using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions.Models;

namespace Vostok.ServiceDiscovery.Extensions.TagFilters
{
    /// <summary>
    /// An implementation of <see cref="ITagFilter" /> that implements "equals" expression.
    /// </summary>
    [PublicAPI]
    public class EqualsTagFilter : ITagFilter
    {
        public EqualsTagFilter([NotNull] string key, [NotNull] string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key));

            Key = key.Trim();
            Value = value?.Trim() ?? throw new ArgumentNullException();
        }

        [NotNull]
        public string Value { get; }

        [NotNull]
        public string Key { get; }

        /// <summary>
        /// Returns true if <see cref="EqualsTagFilter.Key" /> is present in <paramref name="collection" /> and <see cref="EqualsTagFilter.Value" /> equals to <paramref name="collection" /> value by this <see cref="EqualsTagFilter.Key" />.
        /// </summary>
        public bool Matches(TagCollection collection)
            => collection != null
               && collection.TryGetValue(Key, out var value)
               && value == Value;

        public override string ToString() => Key + "==" + Value;
    }
}