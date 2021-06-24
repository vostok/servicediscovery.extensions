using JetBrains.Annotations;
using Vostok.ServiceDiscovery.Abstractions.Models;

namespace Vostok.ServiceDiscovery.Extensions.TagFilters
{
    /// <summary>
    /// Represents a tag filter which will be used to configure service discovery tags filtering rules.
    /// </summary>
    [PublicAPI]
    public interface ITagFilter
    {
        /// <summary>
        /// <para> Returns matching result based on given <paramref name="collection" /> and other implementation parameters. </para>
        /// <para> Implementations need to be careful about modifying <paramref name="collection" />. </para>
        /// </summary>
        bool Matches(TagCollection collection);
    }
}