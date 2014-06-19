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

using System;
using System.Collections.Generic;
using System.Linq;
using Myrcon.Protocols.Frostbite.Battlefield;
using NUnit.Framework;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Test.TestBattlefield {
    [TestFixture]
    public class TestPlayerOnKillDispatchHandler {

        /// <summary>
        /// Tests that passing through no words results in an empty response not an exception.
        /// </summary>
        [Test]
        public void TestInsufficentWordsResultsInEmptyEvent() {
            var called = false;

            var protocol = new BattlefieldGame();

            protocol.ProtocolEvent += (protocol1, args) => { called = true; };

            var request = new FrostbitePacket();

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsFalse(called);
        }

        /// <summary>
        /// Tests that passing through a value not convertable to a boolean for words[4]
        /// will result in an empty response, not an exception.
        /// </summary>
        [Test]
        public void TestWords4NotABooleanResultsInEmptyEvent() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.ProtocolEvent += (protocol1, args) => { called = true; };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "a",
                        "b",
                        "c",
                        "d",
                        "e"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsFalse(called);
        }

        /// <summary>
        /// Tests that passing through a correctly formatted event will result in
        /// no error, even if none of the data exists.
        /// </summary>
        [Test]
        public void TestPassingThroughCorrectFormatResultsInEvent() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => { called = true; };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "Fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests a known item will be looked up and added to the killers inventory
        /// </summary>
        [Test]
        public void TestAKnownItemResultsInThatItemInTheKillerInventory() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Items.TryAdd("fairyfloss", new ItemModel() {
                Name = "fairyfloss",
                FriendlyName = "Fairy Floss, the perfect sniper rifle"
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("fairyfloss", args.Now.Kills.First().Now.Players.First().Inventory.Now.Items.First().Name);
                Assert.AreEqual("Fairy Floss, the perfect sniper rifle", args.Now.Kills.First().Now.Players.First().Inventory.Now.Items.First().FriendlyName);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests the inventory item is simply coalesced with a new item and data in the packet
        /// </summary>
        [Test]
        public void TestAnUnknownItemResultsInThatItemInTheKillerInventory() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("fairyfloss", args.Now.Kills.First().Now.Players.First().Inventory.Now.Items.First().Name);
                Assert.IsEmpty(args.Now.Kills.First().Now.Players.First().Inventory.Now.Items.First().FriendlyName);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests that a killer will be looked up by their name and that player object used
        /// as the killer
        /// </summary>
        [Test]
        public void TestAKnownPlayerWillBeUsedAsTheKiller() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("EA_1", args.Now.Kills.First().Now.Players.First().Uid);
                Assert.AreEqual("Phogue", args.Now.Kills.First().Now.Players.First().Name);
                Assert.AreEqual(100, args.Now.Kills.First().Now.Players.First().Score);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests that if a player cannot be found in the state with a given name then no event will be raised.
        /// </summary>
        [Test]
        public void TestAnUnknownPlayerKillerWillResultInNoEvent() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsFalse(called);
        }

        /// <summary>
        /// Tests that a known killer will have their kills incremented by 1 for the kill
        /// </summary>
        [Test]
        public void TestAKnownKillerWillHaveTheirKillsIncremented() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("EA_1", args.Now.Kills.First().Now.Players.First().Uid);
                Assert.AreEqual(6, args.Now.Kills.First().Now.Players.First().Kills);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests that a victim will be looked up by their name and that player object used
        /// as the victim
        /// </summary>
        [Test]
        public void TestAKnownPlayerWillBeUsedAsTheVictim() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("EA_2", args.Now.Kills.First().Scope.Players.First().Uid);
                Assert.AreEqual("Zaeed", args.Now.Kills.First().Scope.Players.First().Name);
                Assert.AreEqual(100, args.Now.Kills.First().Scope.Players.First().Score);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests that if a player cannot be found in the state with a given name then no event will be raised.
        /// </summary>
        [Test]
        public void TestAnUnknownPlayerVictimWillResultInNoEvent() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsFalse(called);
        }

        /// <summary>
        /// Tests that a known victim will have their kills incremented by 1 for the kill
        /// </summary>
        [Test]
        public void TestAKnownVictimWillHaveTheirDeathsIncremented() {
            var called = false;
            var protocol = new BattlefieldGame();

            protocol.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 100,
                Kills = 5,
                Deaths = 5
            });

            protocol.ProtocolEvent += (protocol1, args) => {
                called = true;

                Assert.AreEqual("EA_2", args.Now.Kills.First().Scope.Players.First().Uid);
                Assert.AreEqual(6, args.Now.Kills.First().Scope.Players.First().Deaths);
            };

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);

            Assert.IsTrue(called);
        }







        /// <summary>
        /// Tests a headshot true will come through as a FrostbiteGame.Headshot
        /// </summary>
        [Test]
        public void TestHeadshotTrueYieldsCorrectFlags() {
            var protocol = new BattlefieldGame();

            protocol.ProtocolEvent += (protocol1, args) => Assert.AreEqual(FrostbiteGame.Headshot, args.Now.Kills.First().Scope.HumanHitLocations.First());

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "Fairyfloss",
                        "true"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);
        }


        /// <summary>
        /// Tests a headshot false will come through as a FrostbiteGame.Bodyshot
        /// </summary>
        [Test]
        public void TestHeadshotFalseYieldsCorrectFlags() {
            var protocol = new BattlefieldGame();

            protocol.ProtocolEvent += (protocol1, args) => Assert.AreEqual(FrostbiteGame.Bodyshot, args.Now.Kills.First().Scope.HumanHitLocations.First());

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "player.onKill",
                        "Phogue",
                        "Zaeed",
                        "Fairyfloss",
                        "false"
                    }
                }
            };

            var response = new FrostbitePacket();

            protocol.PlayerOnKillDispatchHandler(request, response);
        }
    }
}
