using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpSetsObjectMatching : TestNlpBase {
        
        /// <summary>
        /// Kicks Phogue and Philk with no joining character, implying a join into a single list.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoin() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueOrPhilk() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue or phil",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kicks Phogue and Philk using a logical join between the two things.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkLogicalAndJoin() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue and phil",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK
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
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil, zaeed ike",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK,
                    TestNlpBase.PlayerIke,
                    TestNlpBase.PlayerZaeed
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining two sets of information using a logical and join.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIkeZaeedImpliedJoin() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil and zaeed ike",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK,
                    TestNlpBase.PlayerIke,
                    TestNlpBase.PlayerZaeed
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
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil garbage ike",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK,
                    TestNlpBase.PlayerIke
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests combining a set with a single thing of a matching type with a logical and join
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIke() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(),
                "kick phogue phil and ike",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK,
                    TestNlpBase.PlayerIke
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [TestMethod]
        public void TestKickPhoguePhilkImpliedJoinWithImpliedJoinPortValdez() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue phil port valdez",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );
        }

        /// <summary>
        /// Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [TestMethod]
        public void TestKickEveryoneWithExclusionOnPhoguePhilImpliedJoin() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            this.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone but not phogue phil",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Except(new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerPhilK
                }).ToList(),
                new List<Map>()
            );
        }
    }
}
