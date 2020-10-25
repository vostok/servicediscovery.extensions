using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.ServiceDiscovery.Abstractions;
using Vostok.ServiceDiscovery.Abstractions.Models;
using Vostok.ServiceDiscovery.Extensions.Helpers;

namespace Vostok.ServiceDiscovery.Extensions.Tests.Helpers
{
    [TestFixture]
    public class PropertiesHelper_Tests
    {
        [Test]
        public void AddToBlacklist_should_add_blacklist()
        {
            var properties = new TestApplicationInfoProperties();
            var addReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };

            var updated = properties.AddToBlacklist(addReplicas);

            updated.GetBlacklist().Should().BeEquivalentTo(addReplicas);
        }

        [Test]
        public void AddToBlacklist_should_not_add_duplicates()
        {
            var properties = new TestApplicationInfoProperties();
            var addReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };

            var updated = properties
                .AddToBlacklist(addReplicas.Take(2).ToArray())
                .AddToBlacklist(addReplicas);

            updated.GetBlacklist().Should().BeEquivalentTo(addReplicas);
        }

        [Test]
        public void AddToBlacklist_should_add_new_replicas_to_existent_blacklist()
        {
            var properties = new TestApplicationInfoProperties();
            var initReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };
            var withBlacklist = properties.AddToBlacklist(initReplicas);
            var updateReplicas = new[]
            {
                new Uri("http://replica:4/vostok"),
                new Uri("http://replica:5/vostok"),
                new Uri("http://replica:6/vostok")
            };

            var updated = withBlacklist.AddToBlacklist(updateReplicas);

            updated.GetBlacklist().Should().BeEquivalentTo(initReplicas.Concat(updateReplicas));
        }

        [Test]
        public void RemoveFromBlacklist_should_not_fail_with_empty_blacklist()
        {
            var properties = new TestApplicationInfoProperties();
            var removeReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };

            var updated = properties.RemoveFromBlacklist(removeReplicas);

            updated.GetBlacklist().Should().BeEmpty();
        }

        [Test]
        public void RemoveFromBlacklist_should_remove_replicas()
        {
            var properties = new TestApplicationInfoProperties();
            var initReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok"),
                new Uri("http://replica:4/vostok")
            };
            var withBlacklist = properties.AddToBlacklist(initReplicas);

            var removeReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };

            var updated = withBlacklist.RemoveFromBlacklist(removeReplicas);

            updated.GetBlacklist().Should().BeEquivalentTo(new Uri("http://replica:4/vostok"));
        }

        [Test]
        public void RemoveFromBlacklist_should_not_change_blacklist_if_replicas_already_not_present()
        {
            var properties = new TestApplicationInfoProperties();
            var initReplicas = new[]
            {
                new Uri("http://replica:4/vostok")
            };
            var withBlacklist = properties.AddToBlacklist(initReplicas);

            var removeReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok")
            };

            var updated = withBlacklist.RemoveFromBlacklist(removeReplicas);

            updated.GetBlacklist().Should().BeEquivalentTo(new Uri("http://replica:4/vostok"));
        }

        [Test]
        public void AddTags_for_given_replica_should_update_property_tags()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica1 = "replica1";
            var replica2 = "replica2";
            var replica1Tags = new TagCollection
            {
                {"tag1", "value1"},
                {"tag2", "value1"},
                "tag3"
            };
            var replica2Tags = new TagCollection
            {
                "tag4",
                {"tag5", "value3"},
                "tag6"
            };
            properties = properties.AddReplicaTags(replica1, replica1Tags);
            properties.GetReplicaTags(replica1).Should().BeEquivalentTo(replica1Tags);
            properties.GetReplicaTags(replica2).Should().BeEquivalentTo(new TagCollection());

            properties = properties.AddReplicaTags(replica2, replica2Tags);
            properties.GetReplicaTags(replica1).Should().BeEquivalentTo(replica1Tags);
            properties.GetReplicaTags(replica2).Should().BeEquivalentTo(replica2Tags);

            properties.GetTags()
                .Should()
                .BeEquivalentTo(
                    new Dictionary<string, TagCollection>
                    {
                        {replica1, replica1Tags},
                        {replica2, replica2Tags}
                    });
        }

        [Test]
        public void AddTags_for_given_service_replica_should_correctly_parse_service_replicas()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica1 = "http://localhost:1234/";
            var replica2 = "replica2";
            var replica1Tags = new TagCollection
            {
                {"tag1", "value1"},
                {"tag2", "value1"},
                "tag3"
            };
            var replica2Tags = new TagCollection
            {
                "tag4",
                {"tag5", "value3"},
                "tag6"
            };
            
            properties = properties.AddReplicaTags(replica1, replica1Tags);
            properties.GetReplicaTags(replica1).Should().BeEquivalentTo(replica1Tags);
            properties.GetReplicaTags(replica2).Should().BeEquivalentTo(new TagCollection());

            properties = properties.AddReplicaTags(replica2, replica2Tags);
            properties.GetReplicaTags(replica1).Should().BeEquivalentTo(replica1Tags);
            properties.GetReplicaTags(replica2).Should().BeEquivalentTo(replica2Tags);

            properties.GetTags()
                .Should()
                .BeEquivalentTo(
                    new Dictionary<string, TagCollection>
                    {
                        {replica1, replica1Tags},
                        {replica2, replica2Tags},
                    });

            properties.GetServiceTags()
                .Should()
                .BeEquivalentTo(
                    new Dictionary<Uri, TagCollection>
                    {
                        {new Uri(replica1), replica1Tags}
                    });
        }

        [Test]
        public void AddTags_should_update_duplicate_key_property_tags()
        {
            var properties = new TestApplicationInfoProperties();
            var replica1 = "replica1";
            var tagsToAdd = new TagCollection
            {
                {"tag1", "value1"},
                {"tag2", "value1"},
                "tag3"
            };
            var tagsToUpdate = new TagCollection
            {
                {"tag1", "updatedValue"}
            };
            var added = properties.AddReplicaTags(replica1, tagsToAdd);
            var updated = added.AddReplicaTags(replica1, tagsToUpdate);
            updated.GetReplicaTags(replica1).Should().BeEquivalentTo(new TagCollection {{"tag1", "updatedValue"}, {"tag2", "value1"}, "tag3"});
        }

        [Test]
        public void RemoveTags_should_remove_given_replica_tags()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica1 = "replica1";
            var replica2 = "replica2";
            var replica1Tags = new TagCollection
            {
                {"tag1", "value1"},
                {"tag2", "value1"},
                "tag3"
            };
            var replica2Tags = new TagCollection
            {
                "tag4",
                {"tag5", "value3"},
                "tag6"
            };
            properties = properties.AddReplicaTags(replica1, replica1Tags);
            properties = properties.AddReplicaTags(replica2, replica2Tags);
            properties = properties.RemoveReplicaTags(replica1, new List<string> {"tag3", "tag1"});
            properties.GetReplicaTags(replica1).Should().BeEquivalentTo(new TagCollection {{"tag2", "value1"}});
            properties.GetReplicaTags(replica2).Should().BeEquivalentTo(replica2Tags);
        }

        [Test]
        public void RemoveTags_then_remove_all_tags_should_remove_replica_property_key()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica1 = "replica1";
            var replica1Tags = new TagCollection
            {
                {"tag1", "value1"},
                {"tag2", "value1"},
                "tag3"
            };
            properties = properties.AddReplicaTags(replica1, replica1Tags);
            properties = properties.RemoveReplicaTags(replica1, new List<string> {"tag3", "tag1"});
            properties.GetTags().Should().BeEquivalentTo(new Dictionary<string, TagCollection> {{replica1, new TagCollection {{"tag2", "value1"}}}});

            properties = properties.RemoveReplicaTags(replica1, new List<string> {"tag2"});
            properties.GetTags().Should().BeEmpty();
        }

        [Test]
        public void RemoveTags_should_not_fail_with_empty_property_tags()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica1 = "replica1";
            properties = properties.RemoveReplicaTags(replica1, new List<string> {"tag1", "tag2"});
            properties.GetReplicaTags(replica1).Should().BeEmpty();
            properties.GetTags().Should().BeEmpty();
        }

        [Test]
        public void ModifyTags_should_update_properties_with_empty_tags_if_func_is_null()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica = "replica";
            var replicaTags = new TagCollection {"tag4"};
            properties = properties.AddReplicaTags(replica, replicaTags);
            properties = properties.ModifyReplicaTags(replica, null);
            properties.GetReplicaTags(replica).Should().BeEmpty();
            properties.GetTags().Should().BeEmpty();
        }

        [Test]
        public void ModifyTags_should_update_properties_with_empty_tags_if_func_returns_null()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica = "replica";
            var replicaTags = new TagCollection {"tag4"};
            properties = properties.AddReplicaTags(replica, replicaTags);
            properties = properties.ModifyReplicaTags(replica, tags => null);
            properties.GetReplicaTags(replica).Should().BeEmpty();
            properties.GetTags().Should().BeEmpty();
        }

        [Test]
        public void GetTags_should_return_persistent_tags_in_first_priority()
        {
            IApplicationInfoProperties properties = new TestApplicationInfoProperties();
            var replica = "replica";
            var persistentTags = new TagCollection {{"tag4", "v1"}};
            properties = properties.AddReplicaTags(replica, persistentTags);
            var otherTags = new TagCollection {{"tag4", "v2"}, {"tag1", "v1"}};
            properties = properties.Set(new TagPropertyKey(replica, "other").ToString(), otherTags.ToString());
            properties.GetTags()
                .Should()
                .BeEquivalentTo(
                    new Dictionary<string, TagCollection>
                    {
                        {replica, new TagCollection {{"tag4", "v1"}, {"tag1", "v1"}}}
                    });
            
            properties = properties.RemoveReplicaTags(replica, new List<string> {"tag4"});
            properties.GetTags()
                .Should()
                .BeEquivalentTo(
                    new Dictionary<string, TagCollection>
                    {
                        {replica, otherTags}
                    });
        }

        public class TestApplicationInfoProperties : Dictionary<string, string>, IApplicationInfoProperties
        {
            public IApplicationInfoProperties Set(string key, string value)
            {
                this[key ?? throw new ArgumentNullException(nameof(key))] = value;
                return this;
            }

            IApplicationInfoProperties IApplicationInfoProperties.Remove(string key)
            {
                base.Remove(key ?? throw new ArgumentNullException(nameof(key)));
                return this;
            }
        }
    }
}