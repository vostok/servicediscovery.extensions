using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.ServiceDiscovery.Abstractions;

namespace Vostok.ServiceDiscovery.Extensions.Tests
{
    [TestFixture]
    public class ApplicationInfoUpdater_Tests
    {
        [Test]
        public void AddToBlacklist_should_add_blacklist()
        {
            var properties = new TestApplicationInfoProperties();
            var addReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok"),
            };

            var updated = ApplicationInfoUpdater.AddToBlacklist(addReplicas, properties);

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
                new Uri("http://replica:3/vostok"),
            };
            var withBlacklist = ApplicationInfoUpdater.AddToBlacklist(addReplicas, properties);

            var updated = ApplicationInfoUpdater.AddToBlacklist(addReplicas, withBlacklist);

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
                new Uri("http://replica:3/vostok"),
            };
            var withBlacklist = ApplicationInfoUpdater.AddToBlacklist(initReplicas, properties);
            var updateReplicas = new[]
            {
                new Uri("http://replica:4/vostok"),
                new Uri("http://replica:5/vostok"),
                new Uri("http://replica:6/vostok"),
            };

            var updated = ApplicationInfoUpdater.AddToBlacklist(updateReplicas, withBlacklist);

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
                new Uri("http://replica:3/vostok"),
            };

            var updated = ApplicationInfoUpdater.RemoveFromBlacklist(removeReplicas, properties);

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
                new Uri("http://replica:4/vostok"),
            };
            var withBlacklist = ApplicationInfoUpdater.AddToBlacklist(initReplicas, properties);

            var removeReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok"),
            };

            var updated = ApplicationInfoUpdater.RemoveFromBlacklist(removeReplicas, withBlacklist);

            updated.GetBlacklist().Should().BeEquivalentTo(new Uri("http://replica:4/vostok"));
        }

        [Test]
        public void RemoveFromBlacklist_should_not_change_blacklist_if_replicas_already_not_present()
        {
            var properties = new TestApplicationInfoProperties();
            var initReplicas = new[]
            {
                new Uri("http://replica:4/vostok"),
            };
            var withBlacklist = ApplicationInfoUpdater.AddToBlacklist(initReplicas, properties);

            var removeReplicas = new[]
            {
                new Uri("http://replica:1/vostok"),
                new Uri("http://replica:2/vostok"),
                new Uri("http://replica:3/vostok"),
            };

            var updated = ApplicationInfoUpdater.RemoveFromBlacklist(removeReplicas, withBlacklist);

            updated.GetBlacklist().Should().BeEquivalentTo(new Uri("http://replica:4/vostok"));
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