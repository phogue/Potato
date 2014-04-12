using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestRemoveList {
        [Test]
        public void TestSourceContainsNullNotPassed() {
            var passed = false;

            ProtocolState.RemoveList(
                new List<PlayerModel>() {
                    null
                }, new List<PlayerModel>() {
                    new PlayerModel() {
                        Name = ""
                    }
                },
                (model, playerModel) => passed = true
            );

            Assert.IsFalse(passed);
        }

        [Test]
        public void TestOriginalContainsNull() {
            var passed = false;

            ProtocolState.RemoveList(
                new List<PlayerModel>() {
                    new PlayerModel() {
                        Name = ""
                    }
                }, new List<PlayerModel>() {
                    null
                },
                (model, playerModel) => passed = true
            );

            Assert.IsFalse(passed);
        }
    }
}
