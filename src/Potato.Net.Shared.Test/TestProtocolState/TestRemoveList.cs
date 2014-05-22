using System;
using System.Collections.Concurrent;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestRemoveList {
        [Test]
        public void TestSourceContainsNullNotPassed() {
            var passed = false;

            var existing = new ConcurrentDictionary<String, PlayerModel>();
            existing.TryAdd("", null);

            var modified = new ConcurrentDictionary<String, PlayerModel>();
            modified.TryAdd("", new PlayerModel() {
                Name = ""
            });

            ProtocolState.RemoveDictionary(existing, modified);

            Assert.IsFalse(passed);
        }

        [Test]
        public void TestOriginalContainsNull() {
            var passed = false;

            var existing = new ConcurrentDictionary<String, PlayerModel>();
            existing.TryAdd("", new PlayerModel() {
                Name = ""
            });

            var modified = new ConcurrentDictionary<String, PlayerModel>();
            modified.TryAdd("", null);

            ProtocolState.RemoveDictionary(existing, modified);

            Assert.IsFalse(passed);
        }
    }
}
