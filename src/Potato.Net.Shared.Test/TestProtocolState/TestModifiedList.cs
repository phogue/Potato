using System.Collections.Generic;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestModifiedList {
        [Test]
        public void TestSourceContainsNullNotPassed() {
            var passed = false;

            ProtocolState.ModifiedList(
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

            ProtocolState.ModifiedList(
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
