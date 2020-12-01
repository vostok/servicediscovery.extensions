namespace Vostok.ServiceDiscovery.Extensions
{
    /// <summary>
    /// Represents the type of tag in ServiceDiscovery
    /// </summary>
    public enum ReplicaTagKind
    {
        /// <summary>
        /// Tags that exist during the life of beacon and are completely overwritten or deleted during the beacon start or stop.
        /// </summary>
        Ephemeral,
        
        /// <summary>
        /// <para>Tags that can change at any time and are not tied to the beacon lifetime.</para>
        /// <para>This type of tags are controlled only by the user and can be written at any time, even if beacon does not exist.</para>
        /// </summary>
        Persistent
    }
}