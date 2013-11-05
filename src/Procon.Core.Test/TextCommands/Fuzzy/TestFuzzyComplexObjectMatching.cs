using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzyComplexObjectMatching : TestFuzzyBase {

        [TestMethod]
        public void TestComplexKickPhogueOnPortValdez() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue on port valdez",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );
        }

        [TestMethod]
        public void TestComplexKickPlayersGreaterThanPingOnPortValdez() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping over 320 on port valdez",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerEBassie,
                    TestFuzzyBase.PlayerZaeed,
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );
        }

        /// <summary>
        /// Kick everyone with a ping greater than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersGreaterThanPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping greater than 300",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerEBassie,
                    TestFuzzyBase.PlayerZaeed,
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone with a ping greater than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersGreaterThanorEqualToPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping gteq 300",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerEBassie,
                    TestFuzzyBase.PlayerZaeed,
                    TestFuzzyBase.PlayerPhogueIsAButterfly,
                    TestFuzzyBase.PlayerPapaCharlie9
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everything with a ping less than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersLessThanPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping less than 100",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerSayaNishino,
                    TestFuzzyBase.PlayerMrDiacritic
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everything with a ping less than or equal to a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersLessThanOrEqualToPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping lteq 100",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerSayaNishino,
                    TestFuzzyBase.PlayerMrDiacritic,
                    TestFuzzyBase.PlayerImisnew2
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone with a ping equal to a specific number
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersEqualToPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping equal to 50",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone within a range of pings
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersWithinRangeOfPing() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping gteq 50 and ping lteq 100",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerImisnew2
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Test kicking everyone from australia only, excluding all others.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersFromAustralia() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick everyone from australia",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerZaeed,
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Test kicking everyone that is not in australia.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersNotFromAustralia() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone not from australia",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Except(new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerZaeed,
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                }).ToList(),
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexKickAllPlayers() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick all players",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList,
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexChangeMapToAllMaps() {
            TestFuzzyBase.AssertCommandPlayerListMapList(this.CreateTextCommandController(), 
                "change map to all maps",
                TestFuzzyBase.TextCommandChangeMap,
                new List<Player>(),
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez,
                    TestFuzzyBase.MapValparaiso,
                    TestFuzzyBase.MapPanamaCanal
                }
            );
        }

        [TestMethod]
        public void TestComplexKickEveryoneExceptPhogue() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone except phogue",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Where(player => player != TestFuzzyBase.PlayerPhogue).ToList(),
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexKickEveryoneExceptPhogueOnAllMapsButPortValdez() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone except phogue on all maps but port valdez",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Where(player => player != TestFuzzyBase.PlayerPhogue).ToList(),
                new List<Map>() {
                    TestFuzzyBase.MapValparaiso,
                    TestFuzzyBase.MapPanamaCanal
                }
            );
        }
    }
}
