using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzySetsObjectMatching : TestFuzzyBase {
        
        /// <summary>
        /// Kicks Phogue and Philk with no joining character, implying a join into a single list.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoin() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueOrPhilk() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue or phil",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kicks Phogue and Philk using a logical join between the two things.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkLogicalAndJoin() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue and phil",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining two sets of information by placing a garbage string between two sets
        /// which will be thrown out during reduction.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinGarbageIkeZaeedImpliedJoin() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil, zaeed ike",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK,
                    TestFuzzyBase.PlayerIke,
                    TestFuzzyBase.PlayerZaeed
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining two sets of information using a logical and join.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIkeZaeedImpliedJoin() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil and zaeed ike",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK,
                    TestFuzzyBase.PlayerIke,
                    TestFuzzyBase.PlayerZaeed
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining a set with a single thing of a matching type with a garbage separator
        /// that will be thrown out during reduction.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinGarbageIke() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil garbage ike",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK,
                    TestFuzzyBase.PlayerIke
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining a set with a single thing of a matching type with a logical and join
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIke() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil and ike",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK,
                    TestFuzzyBase.PlayerIke
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithImpliedJoinPortValdez() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil port valdez",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );
        }

        /// <summary>
        /// Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [TestMethod]
        public void TestKickEveryoneWithExclusionOnPhoguePhilImpliedJoin() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone but not phogue phil",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Except(new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerPhilK
                }).ToList(),
                new List<Map>()
            );
        }
    }
}
