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
        public static IReadOnlyDictionary<string, TagCollection> GetTags([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties.GetTagsInternal(
                key => TagPropertyKey.TryParse(key, out var tagPropertyKey)
                    ? (tagPropertyKey.ReplicaName, tagPropertyKey.TagKind)
                    : (null, null));

        [NotNull]
        public static IReadOnlyDictionary<Uri, TagCollection> GetServiceTags([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties.GetTagsInternal(
                key => TagPropertyKey.TryParse(key, out var tagPropertyKey)
                    ? (UrlParser.Parse(tagPropertyKey.ReplicaName), tagPropertyKey.TagKind)
                    : (null, null));

        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, [NotNull] string replicaName)
        {
            var tagList = new Dictionary<string, TagCollection>();
            foreach (var property in properties)
            {
                if (!TagPropertyKey.TryParse(property.Key, out var tagPropertyKey))
                    continue;
                if (tagPropertyKey.ReplicaName != replicaName)
                    continue;
                if (!TagCollection.TryParse(property.Value, out var tagCollection))
                    continue;
                tagList[tagPropertyKey.TagKind] = tagCollection;
            }

            return MergeTagCollections(tagList);
        }

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties AddReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, TagCollection tags)
            => ModifyReplicaTags(properties, replicaName, tc => AddTags(tc, tags));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, IEnumerable<string> tagKeysToRemove)
            => ModifyReplicaTags(properties, replicaName, tc => RemoveTags(tc, tagKeysToRemove));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties ClearReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName)
            => properties.SetReplicaTags(replicaName, new TagCollection());

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties ModifyReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, Func<TagCollection, TagCollection> modifyTagsFunc)
        {
            var tags = properties.GetPersistentReplicaTags(replicaName);
            var newTags = modifyTagsFunc?.Invoke(tags) ?? new TagCollection();
            return properties.SetReplicaTags(replicaName, newTags);
        }

        [NotNull]
        private static TagCollection GetPersistentReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, [NotNull] string replicaName)
            => properties.TryGetValue(GetPersistentReplicaTagsPropertyKey(replicaName), out var value)
               && TagCollection.TryParse(value, out var tagCollection)
                ? tagCollection
                : new TagCollection();

        [NotNull]
        private static IReadOnlyDictionary<T, TagCollection> GetTagsInternal<T>([NotNull] this IReadOnlyDictionary<string, string> properties, Func<string, (T, string)> parseReplicaFunc)
        {
            var tagKindDictionary = new Dictionary<T, Dictionary<string, TagCollection>>();
            foreach (var property in properties)
            {
                var (replica, kind) = parseReplicaFunc(property.Key);
                if (replica == null || kind == null)
                    continue;
                if (!TagCollection.TryParse(property.Value, out var tagCollection))
                    continue;

                if (!tagKindDictionary.ContainsKey(replica))
                    tagKindDictionary.Add(replica, new Dictionary<string, TagCollection>());

                tagKindDictionary[replica][kind] = tagCollection;
            }

            return tagKindDictionary.ToDictionary(x => x.Key, x => MergeTagCollections(x.Value));
        }

        [Pure]
        [NotNull]
        private static TagCollection MergeTagCollections([NotNull] Dictionary<string, TagCollection> kindCollectionPairs)
        {
            var tagCollection = new TagCollection();
            var notPersistentTags = kindCollectionPairs
                .Where(kindCollectionPair => kindCollectionPair.Key != PropertyConstants.PersistentTagKindKey)
                .SelectMany(kindCollectionPair => kindCollectionPair.Value);

            foreach (var tag in notPersistentTags)
                tagCollection[tag.Key] = tag.Value;

            if (kindCollectionPairs.ContainsKey(PropertyConstants.PersistentTagKindKey))
            {
                foreach (var tag in kindCollectionPairs[PropertyConstants.PersistentTagKindKey])
                    tagCollection[tag.Key] = tag.Value;
            }

            return tagCollection;
        }

        private static TagCollection AddTags(TagCollection existingTags, TagCollection newTags)
        {
            if (newTags == null)
                return existingTags;
            foreach (var newTag in newTags)
                existingTags[newTag.Key] = newTag.Value;
            return existingTags;
        }

        private static TagCollection RemoveTags(TagCollection existingTags, IEnumerable<string> tagKeysToRemove)
        {
            if (tagKeysToRemove == null)
                return existingTags;
            foreach (var tagToRemove in tagKeysToRemove)
                existingTags.Remove(tagToRemove);
            return existingTags;
        }

        [NotNull]
        private static string GetPersistentReplicaTagsPropertyKey(string replicaName)
            => new TagPropertyKey(replicaName, PropertyConstants.PersistentTagKindKey).ToString();
    }
}