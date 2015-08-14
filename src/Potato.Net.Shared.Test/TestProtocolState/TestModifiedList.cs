using System;
using System.Collections.Concurrent;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestModifiedList {
        [Test]
        public void TestSourceContainsNullNotPassed() {
            var passed = false;

            var existing = new ConcurrentDictionary<string, PlayerModel>();
            existing.TryAdd("", null);

            var modified = new ConcurrentDictionary<string, PlayerModel>();
            modified.TryAdd("", new PlayerModel() {
                Name = ""
            });

            ProtocolState.ModifiedDictionary(existing, modified);

            Assert.IsFalse(passed);
        }

        [Test]
        public void TestOriginalContainsNull() {
            var passed = false;

            var existing = new ConcurrentDictionary<string, PlayerModel>();
            existing.TryAdd("", new PlayerModel() {
                Name = ""
            });

            var modified = new ConcurrentDictionary<string, PlayerModel>();
            modified.TryAdd("", null);

            ProtocolState.ModifiedDictionary(existing, modified);

            Assert.IsFalse(passed);
        }
    }
}
