#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestSet {

        // Players

        [Test]
        public void TestSinglePlayersSet() {
            var state = new ProtocolState();

            state.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 1
            });
            state.Players.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Score = 2
            });

            var segment = new ProtocolStateSegment() {
                Players = new ConcurrentDictionary<string, PlayerModel>()
            };
            segment.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Score = 1
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Players.Count);
            Assert.AreEqual(1, state.Players.First().Value.Score);
        }

        // Maps

        [Test]
        public void TestSingleMapsSet() {
            var state = new ProtocolState();

            state.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });
            state.Maps.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Map 2"
            });

            var segment = new ProtocolStateSegment() {
                Maps = new ConcurrentDictionary<string, MapModel>()
            };
            segment.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Maps.Count);
            Assert.AreEqual("Fun Map 1", state.Maps.First().Value.FriendlyName);
        }

        [Test]
        public void TestTwoMapsIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState();

            state.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "First Map"
            });
            state.Maps.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Second Map"
            });

            var segment = new ProtocolStateSegment() {
                Maps = new ConcurrentDictionary<string, MapModel>()
            };
            segment.Maps.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun First Map"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Maps.Count);
            Assert.AreEqual("Fun First Map", state.Maps.First().Value.FriendlyName);
        }

        // Bans

        [Test]
        public void TestSingleUidBansSet() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Time/Uid/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Time
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<string, BanModel>()
            };
            segment.Bans.TryAdd("Round/Uid/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Round
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Uid = "1",
                            Score = 1
                        }
                    }
                }
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Value.Scope.Times.First().Context);
        }

        [Test]
        public void TestSingleIpBansSet() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Time/Ip/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Time
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<string, BanModel>()
            };
            segment.Bans.TryAdd("Permanent/Ip/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Time
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Ip = "1",
                            Score = 1
                        }
                    }
                }
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.First().Value.Scope.Times.First().Context);
        }

        [Test]
        public void TestSingleNameBansSet() {
            var state = new ProtocolState();

            state.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 1
                        }
                    }
                }
            });
            state.Bans.TryAdd("Time/Name/2", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Time
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "2",
                            Score = 2
                        }
                    }
                }
            });

            var segment = new ProtocolStateSegment() {
                Bans = new ConcurrentDictionary<string, BanModel>()
            };
            // Technically it would have an incorrect key, so next sync it would be
            // removed because it does not exist..
            segment.Bans.TryAdd("Permanent/Name/1", new BanModel() {
                Scope = {
                    Times = {
                        new TimeSubsetModel() {
                            Context = TimeSubsetContext.Time
                        }
                    },
                    Players = new List<PlayerModel>() {
                        new PlayerModel() {
                            Name = "1",
                            Score = 1
                        }
                    }
                }
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.First().Value.Scope.Times.First().Context);
        }

        // MapPool

        [Test]
        public void TestSingleMapPoolSet() {
            var state = new ProtocolState();

            state.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Boring Map 1"
            });
            state.MapPool.TryAdd("gamemode2/map2", new MapModel() {
                Name = "map2",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Boring Map 2"
            });

            var segment = new ProtocolStateSegment() {
                MapPool = new ConcurrentDictionary<string, MapModel>()
            };
            segment.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun Map 1"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.MapPool.Count);
            Assert.AreEqual("Fun Map 1", state.MapPool.First().Value.FriendlyName);
        }

        [Test]
        public void TestMapPoolIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState();

            state.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "First Map"
            });
            state.MapPool.TryAdd("gamemode2/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode2"
                },
                FriendlyName = "Second Map"
            });

            var segment = new ProtocolStateSegment() {
                MapPool = new ConcurrentDictionary<string, MapModel>()
            };
            segment.MapPool.TryAdd("gamemode1/map1", new MapModel() {
                Name = "map1",
                GameMode = new GameModeModel() {
                    Name = "gamemode1"
                },
                FriendlyName = "Fun First Map"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.MapPool.Count);
            Assert.AreEqual("Fun First Map", state.MapPool.First().Value.FriendlyName);
        }

        // GameModePool

        [Test]
        public void TestSingleGameModePoolSet() {
            var state = new ProtocolState();

            state.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Boring GameMode 1"
            });
            state.GameModePool.TryAdd("gamemode2", new GameModeModel() {
                Name = "gamemode 2",
                FriendlyName = "Boring GameMode 2"
            });

            var segment = new ProtocolStateSegment() {
                GameModePool = new ConcurrentDictionary<string, GameModeModel>()
            };
            segment.GameModePool.TryAdd("gamemode1", new GameModeModel() {
                Name = "gamemode 1",
                FriendlyName = "Fun GameMode 1"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.GameModePool.Count);
            Assert.AreEqual("Fun GameMode 1", state.GameModePool.First().Value.FriendlyName);
        }

        // Groups

        [Test]
        public void TestSingleGroupPoolSet() {
            var state = new ProtocolState();

            state.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Boring Group 1"
            });
            state.Groups.TryAdd("Team/2", new GroupModel() {
                Uid = "2",
                Type = GroupModel.Team,
                FriendlyName = "Boring Group 2"
            });

            var segment = new ProtocolStateSegment() {
                Groups = new ConcurrentDictionary<string, GroupModel>()
            };
            segment.Groups.TryAdd("Team/1", new GroupModel() {
                Uid = "1",
                Type = GroupModel.Team,
                FriendlyName = "Fun Group 1"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Groups.Count);
            Assert.AreEqual("Fun Group 1", state.Groups.First().Value.FriendlyName);
        }

        // Items

        [Test]
        public void TestSingleItemPoolSet() {
            var state = new ProtocolState();

            state.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Boring Item 1"
            });
            state.Items.TryAdd("2", new ItemModel() {
                Name = "2",
                FriendlyName = "Boring Item 2"
            });

            var segment = new ProtocolStateSegment() {
                Items = new ConcurrentDictionary<string, ItemModel>()
            };
            segment.Items.TryAdd("1", new ItemModel() {
                Name = "1",
                FriendlyName = "Fun Item 1"
            });

            state.Set(segment);

            Assert.AreEqual(1, state.Items.Count);
            Assert.AreEqual("Fun Item 1", state.Items.First().Value.FriendlyName);
        }
    }
}
