#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections.TextCommands;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyComplexObjectMatching : TestTextCommandParserBase {
        [Test]
        public void TestComplexChangeMapToAllMaps() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "change map to all maps", TextCommandChangeMap, new List<PlayerModel>(), new List<MapModel>() {
                MapPortValdez,
                MapValparaiso,
                MapPanamaCanal
            });
        }

        [Test]
        public void TestComplexKickAllPlayers() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick all players", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values, new List<MapModel>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptMe() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except me", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values.Where(player => player != PlayerPhogue).ToList(), new List<MapModel>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptPhogue() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except phogue", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values.Where(player => player != PlayerPhogue).ToList(), new List<MapModel>());
        }

        [Test]
        public void TestComplexKickEveryoneExceptPhogueOnAllMapsButPortValdez() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone except phogue on all maps but port valdez", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values.Where(player => player != PlayerPhogue).ToList(), new List<MapModel>() {
                MapValparaiso,
                MapPanamaCanal
            });
        }

        [Test]
        public void TestComplexKickEveryoneNotUsingC4() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone not using C4", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values.Where(player => player != PlayerImisnew2).ToList(), new List<MapModel>());
        }

        [Test]
        public void TestComplexKickEveryoneUsingSniperRifles() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone using sniper rifles", TextCommandKick, new List<PlayerModel>() {
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        [Test]
        public void TestComplexKickPhogueOnPortValdez() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue on port valdez", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>() {
                MapPortValdez
            });
        }

        /// <summary>
        ///     Kick everyone with a ping equal to a specific number
        /// </summary>
        [Test]
        public void TestComplexKickPlayersEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping equal to 50", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Test kicking everyone from australia only, excluding all others.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersFromAustralia() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick everyone from australia", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Test kicking everyone from australia only, excluding all others and only players with
        ///     a score less than 1000
        /// </summary>
        [Test]
        public void TestComplexKickPlayersFromAustraliaAndScoreLessThan1000() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick everyone from australia and score less than 1000", TextCommandKick, new List<PlayerModel>() {
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kick everyone with a ping greater than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersGreaterThanPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping greater than 300", TextCommandKick, new List<PlayerModel>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        [Test]
        public void TestComplexKickPlayersGreaterThanPingOnPortValdez() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping over 320 on port valdez", TextCommandKick, new List<PlayerModel>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }, new List<MapModel>() {
                MapPortValdez
            });
        }

        /// <summary>
        ///     Kick everyone with a ping greater than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersGreaterThanorEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 300", TextCommandKick, new List<PlayerModel>() {
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly,
                PlayerPapaCharlie9
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kick everything with a ping less than or equal to a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersLessThanOrEqualToPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping lteq 100", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerSayaNishino,
                PlayerMrDiacritic,
                PlayerImisnew2
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kick everything with a ping less than a specific number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersLessThanPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping less than 100", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerSayaNishino,
                PlayerMrDiacritic
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Test kicking everyone that is not in australia.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersNotFromAustralia() {
            var textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone not from australia", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Values.Except(new List<PlayerModel>() {
                PlayerPhogue,
                PlayerZaeed,
                PlayerPhogueIsAButterfly
            }).ToList(), new List<MapModel>());
        }

        /// <summary>
        ///     Kick everyone within a range of pings
        /// </summary>
        [Test]
        public void TestComplexKickPlayersWithinRangeOfPing() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 50 and ping lteq 100", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerImisnew2
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kick everyone within a range of pings and a score equal to a set number.
        /// </summary>
        [Test]
        public void TestComplexKickPlayersWithinRangeOfPingAndScoreEqualTo1000() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick players with ping gteq 50 and ping lteq 100 and score = 1000", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }
    }
}