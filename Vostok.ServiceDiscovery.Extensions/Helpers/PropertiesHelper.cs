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

            blacklist.UnionWith(replicasToAdd);
            return properties.SetBlacklist(blacklist);
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
        public static IApplicationInfoProperties ModifyReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
        {
            var tags = properties.GetPersistentReplicaTags(replicaName);
            var newTags = modifyTagsFunc(tags);
            return properties.SetReplicaTags(replicaName, newTags);
        }

        [NotNull]
        public static IReadOnlyDictionary<string, TagCollection> GetTags([NotNull] this IReadOnlyDictionary<string, string> properties)
        {
            return properties
                .Where(x => TagPropertyHelpers.IsTagsPropertyKey(x.Key))
                .GroupBy(x => TagPropertyHelpers.ExtractReplicaName(x.Key), x => TagCollection.TryParse(x.Value, out var tags) ? tags : null)
                .ToDictionary(x => x.Key, MergeTagCollections);
        }

        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, string replicaName)
        {
            var tagCollections = properties
                .Where(x => TagPropertyHelpers.IsTagsPropertyKey(x.Key))
                .Select(x => TagCollection.TryParse(x.Value, out var tags) ? tags : null);
            
            return MergeTagCollections(tagCollections);
        }

        [NotNull]
        public static IApplicationInfoProperties SetReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, TagCollection tagCollection)
        {
            var propertyName = GetPersistentReplicaTagsPropertyKey(replicaName);
            return tagCollection.Any() 
                ? properties.Set(propertyName, tagCollection.ToString()) 
                : properties.Remove(propertyName);
        }

        [CanBeNull]
        private static TagCollection GetPersistentReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, string replicaName) 
            => properties.TryGetValue(GetPersistentReplicaTagsPropertyKey(replicaName), out var value) 
                && TagCollection.TryParse(value, out var tagCollection)
                ? tagCollection
                : null;

        [NotNull]
        private static string GetPersistentReplicaTagsPropertyKey(string replicaName)
            => TagPropertyHelpers.FormatName(replicaName, "persistent");

        [Pure]
        [NotNull]
        private static TagCollection MergeTagCollections([NotNull] IEnumerable<TagCollection> collections)
        {
            return new TagCollection(
                collections
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .ToDictionary(x => x.Key, x => x.Value)
                );
        }
        
        private const string TagsParameterPrefix = "Tags:";
        private const string TagsParameterValuesSeparator = ":";
    }
}