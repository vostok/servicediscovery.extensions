using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Url;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Abstractions.Models;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    internal static class PropertiesHelper
    {
        [CanBeNull]
        public static Uri GetExternalUrl([NotNull] IReadOnlyDictionary<string, string> properties)
            => properties.TryGetValue(PropertyConstants.ExternalUrlProperty, out var value)
                ? UrlParser.Parse(value)
                : null;

        [NotNull]
        public static Uri[] GetBlacklist([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties.TryGetValue(PropertyConstants.BlacklistProperty, out var value)
                ? UrlParser.Parse(value.Split(PropertyConstants.BlacklistItemSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                : Array.Empty<Uri>();

        [CanBeNull]
        public static ReplicaWeights GetReplicaWeights([NotNull] this IReadOnlyDictionary<string, string> properties)
        {
            if (!properties.TryGetValue(PropertyConstants.WeightsVersionProperty, out var weightsVersion) ||
                !properties.TryGetValue(PropertyConstants.WeightsProperty, out var weightsData) ||
                !int.TryParse(weightsVersion, out var numericVersion) || numericVersion != 0)
                return null;

            return ReplicaWeightsSerializer.Deserialize(Convert.FromBase64String(weightsData));
        }

        [Pure]
        public static IApplicationInfoProperties AddToBlacklist(this IApplicationInfoProperties properties, Uri[] replicasToAdd)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (blacklist.IsSupersetOf(replicasToAdd))
                return properties;

            var newBlackList = blacklist.Concat(replicasToAdd);
            return properties.SetBlacklist(newBlackList);
        }

        [Pure]
        public static IApplicationInfoProperties RemoveFromBlacklist(this IApplicationInfoProperties properties, Uri[] replicasToRemove)
        {
            var blacklist = new HashSet<Uri>(properties.GetBlacklist());
            if (!replicasToRemove.Intersect(blacklist).Any())
                return properties;

            blacklist.ExceptWith(replicasToRemove);
            return properties.SetBlacklist(blacklist);
        }
        
        [NotNull]
        public static IApplicationInfoProperties ModifyReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, Func<ITag[], ITag[]> modifyTagsFunc)
        {
            var tags = properties.GetPersistentReplicaTags(replicaName);
            var newTags = modifyTagsFunc(tags);
            return properties.SetReplicaTags(replicaName, newTags);
        }
        
        [NotNull]
        public static IReadOnlyDictionary<string, ITag[]> GetTags([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties
                .Where(x => x.Key.StartsWith(TagsParameterPrefix))
                .ToDictionary(x => x.Key, x => ReplicaTagsHelpers.Deserialize(x.Value));

        [NotNull]
        public static ITag[] GetReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, string replicaName)
        {
            return properties
                .Where(x => x.Key.StartsWith(TagsParameterPrefix + replicaName + ":"))
                .SelectMany(x => ReplicaTagsHelpers.Deserialize(x.Value))
                .ToArray();
        }

        [NotNull]
        public static IApplicationInfoProperties SetReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, ITag[] tags)
        {
            var propertyName = GetPersistentReplicaTagsPropertyKey(replicaName);
            return tags.Length == 0 
                ? properties.Remove(propertyName) 
                : properties.Set(propertyName, ReplicaTagsHelpers.Serialize(tags));
        }

        [NotNull]
        private static ITag[] GetPersistentReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, string replicaName) 
            => properties.TryGetValue(GetPersistentReplicaTagsPropertyKey(replicaName), out var value) 
                ? ReplicaTagsHelpers.Deserialize(value) 
                : Array.Empty<ITag>();

        [NotNull]
        private static string GetPersistentReplicaTagsPropertyKey(string replicaName)
            => ReplicaTagsHelpers.GetReplicaTagsPropertyKey(replicaName + ":" + "persistent");
        
        private const string TagsParameterPrefix = "Tags:";
    }
}