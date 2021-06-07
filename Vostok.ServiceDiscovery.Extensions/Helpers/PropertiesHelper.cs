using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Topology;
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
        public static IApplicationInfoProperties SetReplicaTags([NotNull] this IApplicationInfoProperties properties, [NotNull] string replicaName, ReplicaTagKind replicaTagKind, TagCollection tags)
        {
            var propertyKey = new TagsPropertyKey(replicaName, replicaTagKind.ToString());
            if (properties.GetReplicaKindTags(propertyKey).Equals(tags))
                return properties;

            return tags?.Count > 0
                ? properties.Set(propertyKey.ToString(), tags.ToString())
                : properties.Remove(propertyKey.ToString());
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
                key => TagsPropertyKey.TryParse(key, out var tagPropertyKey)
                    ? (tagPropertyKey.ReplicaName, tagPropertyKey.TagKind)
                    : (null, null));

        [NotNull]
        public static IReadOnlyDictionary<Uri, TagCollection> GetServiceTags([NotNull] this IReadOnlyDictionary<string, string> properties)
            => properties.GetTagsInternal(
                key => TagsPropertyKey.TryParse(key, out var tagPropertyKey)
                    ? (UrlParser.Parse(tagPropertyKey.ReplicaName), tagPropertyKey.TagKind)
                    : (null, null),
                ReplicaComparer.Instance);

        [NotNull]
        public static TagCollection GetReplicaTags([NotNull] this IReadOnlyDictionary<string, string> properties, [NotNull] string replicaName)
        {
            var persistentTags = properties.GetReplicaKindTags(new TagsPropertyKey(replicaName, ReplicaTagKind.Persistent.ToString()));
            var ephemeralTags = properties.GetReplicaKindTags(new TagsPropertyKey(replicaName, ReplicaTagKind.Ephemeral.ToString()));
            return MergeTagCollections(ephemeralTags, persistentTags);
        }

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties AddReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, ReplicaTagKind replicaTagKind, TagCollection tags)
            => ModifyReplicaTags(properties, replicaName, replicaTagKind, tc => AddTags(tc, tags));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties RemoveReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, ReplicaTagKind replicaTagKind, IEnumerable<string> tagKeys)
            => ModifyReplicaTags(properties, replicaName, replicaTagKind, tc => RemoveTags(tc, tagKeys));

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties ClearReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, ReplicaTagKind replicaTagKind)
            => properties.SetReplicaTags(replicaName, replicaTagKind, new TagCollection());

        [Pure]
        [NotNull]
        public static IApplicationInfoProperties ModifyReplicaTags([NotNull] this IApplicationInfoProperties properties, string replicaName, ReplicaTagKind replicaTagKind, Func<TagCollection, TagCollection> modifyTagsFunc)
        {
            var tags = properties.GetReplicaKindTags(replicaName, replicaTagKind);
            var newTags = modifyTagsFunc?.Invoke(tags) ?? new TagCollection();
            return properties.SetReplicaTags(replicaName, replicaTagKind, newTags);
        }

        [NotNull]
        private static TagCollection GetReplicaKindTags([NotNull] this IReadOnlyDictionary<string, string> properties, [NotNull] TagsPropertyKey tagsPropertyKey)
            => properties.TryGetValue(tagsPropertyKey.ToString(), out var collectionString)
                ? TagCollection.TryParse(collectionString, out var collection)
                    ? collection
                    : new TagCollection()
                : new TagCollection();

        [NotNull]
        private static TagCollection GetReplicaKindTags([NotNull] this IReadOnlyDictionary<string, string> properties, [NotNull] string replicaName, ReplicaTagKind replicaTagKind)
            => properties.GetReplicaKindTags(new TagsPropertyKey(replicaName, replicaTagKind.ToString()));

        [NotNull]
        private static IReadOnlyDictionary<T, TagCollection> GetTagsInternal<T>([NotNull] this IReadOnlyDictionary<string, string> properties, Func<string, (T, string)> parseReplicaFunc, IEqualityComparer<T> comparer = null)
        {
            var tagKindDictionary = new Dictionary<T, Dictionary<ReplicaTagKind, TagCollection>>(comparer);
            foreach (var property in properties)
            {
                var (replica, kindString) = parseReplicaFunc(property.Key);
                if (replica == null || kindString == null)
                    continue;
                if (!Enum.TryParse<ReplicaTagKind>(kindString, out var kind))
                    continue;
                if (!TagCollection.TryParse(property.Value, out var tagCollection))
                    continue;

                if (!tagKindDictionary.TryGetValue(replica, out var tags))
                    tagKindDictionary[replica] = tags = new Dictionary<ReplicaTagKind, TagCollection>();
                tags[kind] = tagCollection;
            }

            return tagKindDictionary
                .ToDictionary(
                    x => x.Key,
                    x => MergeTagCollections(
                        x.Value.TryGetValue(ReplicaTagKind.Ephemeral, out var ephemeralTags) ? ephemeralTags : null,
                        x.Value.TryGetValue(ReplicaTagKind.Persistent, out var persistentTags) ? persistentTags : null),
                    comparer);
        }

        [Pure]
        [NotNull]
        private static TagCollection MergeTagCollections([CanBeNull] TagCollection ephemeralTags, [CanBeNull] TagCollection persistentTags)
        {
            if (ephemeralTags == null)
                return persistentTags ?? new TagCollection();
            if (persistentTags == null)
                return ephemeralTags;
            var tagCollection = new TagCollection();
            foreach (var pair in ephemeralTags.Concat(persistentTags))
                tagCollection[pair.Key] = pair.Value;
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

        private static TagCollection RemoveTags(TagCollection existingTags, IEnumerable<string> tagKeys)
        {
            if (tagKeys == null)
                return existingTags;
            foreach (var tagToRemove in tagKeys)
                existingTags.Remove(tagToRemove);
            return existingTags;
        }
    }
}