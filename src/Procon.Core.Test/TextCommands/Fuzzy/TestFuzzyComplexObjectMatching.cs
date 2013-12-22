#region

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyComplexObjectMatching : TestFuzzyBase {
        [Test]
        public void TestComplexChangeMapToAllMaps() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "change map to all maps", TextCommandChangeMap, new List<Player>(), new List<Map>() {
                MapPortValdez,
                MapValparaiso,
                MapPanamaCanal
            });
        }

        [Test]
        public void TestComplexKickAllPlayers() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick all players", TextCommandKick, textCommandController.Connection.GameState.Players, new List<Map>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptMe() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except me", TextCommandKick, textCommandController.Connection.GameState.Players.Where(player => player != PlayerPhogue).ToList(), new List<Map>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptPhogue() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except phogue", TextCommandKick, textCommandController.Connection.GameState.Players.Where(player => player != PlayerPhogue).ToList(), new List<Map>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptPhogueOnAllMapsButPortValdez() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except phogue on all maps but port valdez", TextCommandKick, textCommandController.Connection.GameState.Players.Where(player => player != PlayerPhogue).ToList(), new List<Map>() {
                MapValparaiso,
                MapPanamaCanal
            });
        }

        [Test]
        public void TestComplexKickEveryoneNotUsingC4() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone not using C4", TextCommandKick, textCommandController.Connection.GameState.Players.Where(player => player != PlayerImisnew2).ToList(), new List<Map>());
        }

        [Test]
        public void TestComplexKickEveryoneUsingSniperRifles() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone using sniper rifles", TextCommandKick, new List<Player>() {
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<Map>());
        }

        [Test]
        public void TestComplexKickPhogueOnPortValdez() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue on port valdez", TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>() {
                MapPortValdez
            });
        }

        /// <summary>
        ///     Kick everyone with a ping equal to a specific number
        /// </summary>
        [Test]
        public void TestComplexKickPlayersEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping equal to 50", TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>());
        }

        /// <summary>
        ///     Test kicking everyone from australia only, excluding all others.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersFromAustralia() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick everyone from australia", TextCommandKick, new List<Player>() {
                PlayerPhogue,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<Map>());
        }

        /// <summary>
        ///     Test kicking everyone from australia only, excluding all others and only players with
        ///     a score less than 1000
        /// </summary>
        [Test]
        public void TestComplexKickPlayersFromAustraliaAndScoreLessThan1000() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick everyone from australia and score less than 1000", TextCommandKick, new List<Player>() {
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<Map>());
        }

        /// <summary>
        ///     Kick everyone with a ping greater than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersGreaterThanPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping greater than 300", TextCommandKick, new List<Player>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<Map>());
        }

        [Test]
        public void TestComplexKickPlayersGreaterThanPingOnPortValdez() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping over 320 on port valdez", TextCommandKick, new List<Player>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<Map>() {
                MapPortValdez
            });
        }

        /// <summary>
        ///     Kick everyone with a ping greater than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersGreaterThanorEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 300", TextCommandKick, new List<Player>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly,
                PlayerPapaCharlie9
            }, new List<Map>());
        }

        /// <summary>
        ///     Kick everything with a ping less than or equal to a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersLessThanOrEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping lteq 100", TextCommandKick, new List<Player>() {
                PlayerPhogue,
                PlayerSayaNishino,
                PlayerMrDiacritic,
                PlayerImisnew2
            }, new List<Map>());
        }

        /// <summary>
        ///     Kick everything with a ping less than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersLessThanPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping less than 100", TextCommandKick, new List<Player>() {
                PlayerPhogue,
                PlayerSayaNishino,
                PlayerMrDiacritic
            }, new List<Map>());
        }

        /// <summary>
        ///     Test kicking everyone that is not in australia.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersNotFromAustralia() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone not from australia", TextCommandKick, textCommandController.Connection.GameState.Players.Except(new List<Player>() {
                PlayerPhogue,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }).ToList(), new List<Map>());
        }

        /// <summary>
        ///     Kick everyone within a range of pings
        /// </summary>
        [Test]
        public void TestComplexKickPlayersWithinRangeOfPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 50 and ping lteq 100", TextCommandKick, new List<Player>() {
                PlayerPhogue,
                PlayerImisnew2
            }, new List<Map>());
        }

        /// <summary>
        ///     Kick everyone within a range of pings and a score equal to a set number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersWithinRangeOfPingAndScoreEqualTo1000() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 50 and ping lteq 100 and score = 1000", TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>());
        }
    }
}