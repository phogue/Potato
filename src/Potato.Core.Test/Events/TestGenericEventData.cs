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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

#endregion

namespace Potato.Core.Test.Events {
    [TestFixture]
    public class TestGenericEventData {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        [Test]
        public void TestGameEventObjectConversion() {
            var gameEventData = new ProtocolEventData() {
                Bans = new List<BanModel>() {
                    new BanModel() {
                        Now = {
                            Content = new List<String>() {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Chats = new List<ChatModel>() {
                    new ChatModel() {
                        Now = {
                            Content = new List<String>() {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Kicks = new List<KickModel>() {
                    new KickModel() {
                        Scope = {
                            Content = new List<String>() {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Kills = new List<KillModel>() {
                    new KillModel() {
                        Scope = {
                            Content = new List<String>() {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Players = new List<PlayerModel>() {
                    new PlayerModel() {
                        Name = "Phogue",
                        Ping = 100
                    }
                },
                Spawns = new List<SpawnModel>() {
                    new SpawnModel() {
                        Player = new PlayerModel() {
                            Name = "Phogue",
                            Ping = 100
                        }
                    }
                }
            };

            CommandData genericEventData = GenericEventData.Parse(gameEventData);

            Assert.AreEqual("I should remove this property", genericEventData.Bans.First().Now.Content.First());

            Assert.AreEqual("I should remove this property", genericEventData.Chats.First().Now.Content.First());

            Assert.AreEqual("I should remove this property", genericEventData.Kicks.First().Scope.Content.First());

            Assert.AreEqual("I should remove this property", genericEventData.Kills.First().Scope.Content.First());

            Assert.AreEqual("Phogue", genericEventData.Players.First().Name);
            Assert.AreEqual(100, genericEventData.Players.First().Ping);

            Assert.AreEqual("Phogue", genericEventData.Spawns.First().Player.Name);
            Assert.AreEqual(100, genericEventData.Spawns.First().Player.Ping);
        }
    }
}