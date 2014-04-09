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
using Procon.Core.Connections.TextCommands;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzySetsObjectMatching : TestTextCommandParserBase {
        /// <summary>
        ///     Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [Test]
        public void TestKickEveryoneWithExclusionOnPhoguePhilImpliedJoin() {
            TextCommandController textCommandController = CreateTextCommandController();

            AssertCommandPlayerListMapList(textCommandController, "kick everyone but not phogue phil", TextCommandKick, textCommandController.Connection.ProtocolState.Players.Except(new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK
            }).ToList(), new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueOrPhilk() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue or phil", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kicks Phogue and Philk with no joining character, implying a join into a single list.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoin() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Tests combining a set with a single thing of a matching type with a garbage separator
        ///     that will be thrown out during reduction.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoinGarbageIke() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil garbage ike", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK,
                PlayerIke
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Tests combining two sets of information by placing a garbage string between two sets
        ///     which will be thrown out during reduction.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoinGarbageIkeZaeedImpliedJoin() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil, zaeed ike", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK,
                PlayerIke,
                PlayerZaeed
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Tests that sets won't be combined if not all of the types match.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoinWithImpliedJoinPortValdez() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil port valdez", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK
            }, new List<MapModel>() {
                MapPortValdez
            });
        }

        /// <summary>
        ///     Tests combining a set with a single thing of a matching type with a logical and join
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIke() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil and ike", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK,
                PlayerIke
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Tests combining two sets of information using a logical and join.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkImpliedJoinWithLogicalAndJoinIkeZaeedImpliedJoin() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue phil and zaeed ike", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK,
                PlayerIke,
                PlayerZaeed
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kicks Phogue and Philk using a logical join between the two things.
        /// </summary>
        [Test]
        public void TestKickPhoguePhilkLogicalAndJoin() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue and phil", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerPhilK
            }, new List<MapModel>());
        }
    }
}