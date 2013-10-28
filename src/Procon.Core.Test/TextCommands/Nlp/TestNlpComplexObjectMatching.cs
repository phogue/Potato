using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpComplexObjectMatching : TestNlpBase {

        [TestMethod]
        public void TestComplexKickPhogueOnPortValdez() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue on port valdez",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );
        }

        [TestMethod]
        public void TestComplexKickPlayersGreaterThanPingOnPortValdez() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping over 320 on port valdez",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerEBassie,
                    TestNlpBase.PlayerZaeed,
                    TestNlpBase.PlayerPhogueIsAButterfly
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );
        }

        /// <summary>
        /// Kick everyone with a ping greater than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersGreaterThanPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping greater than 300",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerEBassie,
                    TestNlpBase.PlayerZaeed,
                    TestNlpBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone with a ping greater than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersGreaterThanorEqualToPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping gteq 300",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerEBassie,
                    TestNlpBase.PlayerZaeed,
                    TestNlpBase.PlayerPhogueIsAButterfly,
                    TestNlpBase.PlayerPapaCharlie9
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everything with a ping less than a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersLessThanPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping less than 100",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerSayaNishino,
                    TestNlpBase.PlayerMrDiacritic
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everything with a ping less than or equal to a specific number.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersLessThanOrEqualToPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping lteq 100",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerSayaNishino,
                    TestNlpBase.PlayerMrDiacritic,
                    TestNlpBase.PlayerImisnew2
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone with a ping equal to a specific number
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersEqualToPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping equal to 50",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Kick everyone within a range of pings
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersWithinRangeOfPing() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick players with ping gteq 50 and ping lteq 100",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerImisnew2
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Test kicking everyone from australia only, excluding all others.
        /// </summary>
        [TestMethod]
        public void TestComplexKickPlayersFromAustralia() {
            this.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick everyone from australia",
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerZaeed,
                    TestNlpBase.PlayerPhogueIsAButterfly
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

            this.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone not from australia",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Except(new List<Player>() {
                    TestNlpBase.PlayerPhogue,
                    TestNlpBase.PlayerZaeed,
                    TestNlpBase.PlayerPhogueIsAButterfly
                }).ToList(),
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexKickAllPlayers() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            this.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick all players",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList,
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexChangeMapToAllMaps() {
            this.AssertCommandPlayerListMapList(this.CreateTextCommandController(), 
                "change map to all maps",
                TestNlpBase.TextCommandChangeMap,
                new List<Player>(),
                new List<Map>() {
                    TestNlpBase.MapPortValdez,
                    TestNlpBase.MapValparaiso,
                    TestNlpBase.MapPanamaCanal
                }
            );
        }

        [TestMethod]
        public void TestComplexKickEveryoneExceptPhogue() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            this.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone except phogue",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Where(player => player != TestNlpBase.PlayerPhogue).ToList(),
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestComplexKickEveryoneExceptPhogueOnAllMapsButPortValdez() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            this.AssertCommandPlayerListMapList(
                textCommandController, 
                "kick everyone except phogue on all maps but port valdez",
                TestNlpBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList.Where(player => player != TestNlpBase.PlayerPhogue).ToList(),
                new List<Map>() {
                    TestNlpBase.MapValparaiso,
                    TestNlpBase.MapPanamaCanal
                }
            );
        }
    }
}
