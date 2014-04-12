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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestUpdate {

        // Players

        [Test]
        public void TestMultiplePlayersModified() {
            var state = new ProtocolState() {
                Players = {
                    new PlayerModel() {
                        Uid = "1",
                        Score = 1
                    },
                    new PlayerModel() {
                        Uid = "2",
                        Score = 2
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Players = new List<PlayerModel>() {
                    new PlayerModel() {
                        Uid = "1",
                        Score = 100
                    },
                    new PlayerModel() {
                        Uid = "2",
                        Score = 200
                    }
                }
            });

            Assert.AreEqual(2, state.Players.Count);
            Assert.AreEqual(100, state.Players.First(item => item.Uid == "1").Score);
            Assert.AreEqual(200, state.Players.First(item => item.Uid == "2").Score);
        }

        [Test]
        public void TestPlayerModifiedAndPlayerInserted() {
            var state = new ProtocolState() {
                Players = {
                    new PlayerModel() {
                        Uid = "1",
                        Score = 1
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Players = new List<PlayerModel>() {
                    new PlayerModel() {
                        Uid = "1",
                        Score = 100
                    },
                    // Does not currently exist, will be inserted.
                    new PlayerModel() {
                        Uid = "2",
                        Score = 200
                    }
                }
            });

            Assert.AreEqual(2, state.Players.Count);
            Assert.AreEqual(100, state.Players.First(item => item.Uid == "1").Score);
            Assert.AreEqual(200, state.Players.First(item => item.Uid == "2").Score);
        }
        
        // Maps

        [Test]
        public void TestMultipleMapsModified() {
            var state = new ProtocolState() {
                Maps = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Boring Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Boring Map 2"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Maps = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Map 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun Map 1", state.Maps.First(item => item.Name == "map1").FriendlyName);
            Assert.AreEqual("Fun Map 2", state.Maps.First(item => item.Name == "map2").FriendlyName);
        }

        [Test]
        public void TestMapModifiedAndMapInserted() {
            var state = new ProtocolState() {
                Maps = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Boring Map 1"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Maps = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Map 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun Map 1", state.Maps.First(item => item.Name == "map1").FriendlyName);
            Assert.AreEqual("Fun Map 2", state.Maps.First(item => item.Name == "map2").FriendlyName);
        }

        [Test]
        public void TestTwoMapsIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState() {
                Maps = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "First Map"
                    },
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Second Map"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Maps = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun First Map"
                    },
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Second Map"
                    }
                }
            });

            Assert.AreEqual(2, state.Maps.Count);
            Assert.AreEqual("Fun First Map", state.Maps.First(item => item.Name == "map1" && item.GameMode.Name == "gamemode1").FriendlyName);
            Assert.AreEqual("Fun Second Map", state.Maps.First(item => item.Name == "map1" && item.GameMode.Name == "gamemode2").FriendlyName);
        }

        // Bans

        [Test]
        public void TestMultipleUidBansModified() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    },
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Permanent
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Uid = "2",
                                    Score = 2
                                }
                            }
                        }
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
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
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }

        [Test]
        public void TestUidBanModifiedAndUidBanInserted() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
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
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }


        [Test]
        public void TestMultipleIpBansModified() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    },
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Permanent
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Ip = "2",
                                    Score = 2
                                }
                            }
                        }
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Round
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Ip = "1",
                                    Score = 1
                                }
                            }
                        }
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }

        [Test]
        public void TestIpBanModifiedAndIpBanInserted() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Round
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Ip = "1",
                                    Score = 1
                                }
                            }
                        }
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }


        [Test]
        public void TestMultNameleNameBansModified() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    },
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Permanent
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Name = "2",
                                    Score = 2
                                }
                            }
                        }
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Round
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Name = "1",
                                    Score = 1
                                }
                            }
                        }
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }

        [Test]
        public void TestNameBanModifiedAndNameBanInserted() {
            var state = new ProtocolState() {
                Bans = {
                    new BanModel() {
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
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Bans = new List<BanModel>() {
                    new BanModel() {
                        Scope = {
                            Times = {
                                new TimeSubsetModel() {
                                    Context = TimeSubsetContext.Round
                                }
                            },
                            Players = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Name = "1",
                                    Score = 1
                                }
                            }
                        }
                    },
                    new BanModel() {
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
                    }
                }
            });

            Assert.AreEqual(2, state.Bans.Count);
            Assert.AreEqual(TimeSubsetContext.Round, state.Bans.First().Scope.Times.First().Context);
            Assert.AreEqual(TimeSubsetContext.Time, state.Bans.Last().Scope.Times.First().Context);
        }

        // MapPool

        [Test]
        public void TestMultipleMapPoolModified() {
            var state = new ProtocolState() {
                MapPool = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Boring Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Boring Map 2"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                MapPool = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Map 2"
                    }
                }
            });

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun Map 1", state.MapPool.First(item => item.Name == "map1").FriendlyName);
            Assert.AreEqual("Fun Map 2", state.MapPool.First(item => item.Name == "map2").FriendlyName);
        }

        [Test]
        public void TestMapPoolModifiedAndMapPoolInserted() {
            var state = new ProtocolState() {
                MapPool = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Boring Map 1"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                MapPool = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun Map 1"
                    },
                    new MapModel() {
                        Name = "map2",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Map 2"
                    }
                }
            });

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun Map 1", state.MapPool.First(item => item.Name == "map1").FriendlyName);
            Assert.AreEqual("Fun Map 2", state.MapPool.First(item => item.Name == "map2").FriendlyName);
        }

        [Test]
        public void TestTwoMapPoolIdenticalNamesDifferentGameModes() {
            var state = new ProtocolState() {
                MapPool = {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "First Map"
                    },
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Second Map"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                MapPool = new List<MapModel>() {
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode1"
                        },
                        FriendlyName = "Fun First Map"
                    },
                    new MapModel() {
                        Name = "map1",
                        GameMode = new GameModeModel() {
                            Name = "gamemode2"
                        },
                        FriendlyName = "Fun Second Map"
                    }
                }
            });

            Assert.AreEqual(2, state.MapPool.Count);
            Assert.AreEqual("Fun First Map", state.MapPool.First(item => item.Name == "map1" && item.GameMode.Name == "gamemode1").FriendlyName);
            Assert.AreEqual("Fun Second Map", state.MapPool.First(item => item.Name == "map1" && item.GameMode.Name == "gamemode2").FriendlyName);
        }

        // GameModePool

        [Test]
        public void TestMultipleGameModePoolModified() {
            var state = new ProtocolState() {
                GameModePool = {
                    new GameModeModel() {
                        Name = "gamemode 1",
                        FriendlyName = "Boring GameMode 1"
                    },
                    new GameModeModel() {
                        Name = "gamemode 2",
                        FriendlyName = "Boring GameMode 2"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                GameModePool = new List<GameModeModel>() {
                    new GameModeModel() {
                        Name = "gamemode 1",
                        FriendlyName = "Fun GameMode 1"
                    },
                    new GameModeModel() {
                        Name = "gamemode 2",
                        FriendlyName = "Fun GameMode 2"
                    }
                }
            });

            Assert.AreEqual(2, state.GameModePool.Count);
            Assert.AreEqual("Fun GameMode 1", state.GameModePool.First().FriendlyName);
            Assert.AreEqual("Fun GameMode 2", state.GameModePool.Last().FriendlyName);
        }

        [Test]
        public void TestGameModePoolModifiedAndGameModePoolInserted() {
            var state = new ProtocolState() {
                GameModePool = {
                    new GameModeModel() {
                        Name = "gamemode 1",
                        FriendlyName = "Boring GameMode 1"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                GameModePool = new List<GameModeModel>() {
                    new GameModeModel() {
                        Name = "gamemode 1",
                        FriendlyName = "Fun GameMode 1"
                    },
                    new GameModeModel() {
                        Name = "gamemode 2",
                        FriendlyName = "Fun GameMode 2"
                    }
                }
            });

            Assert.AreEqual(2, state.GameModePool.Count);
            Assert.AreEqual("Fun GameMode 1", state.GameModePool.First().FriendlyName);
            Assert.AreEqual("Fun GameMode 2", state.GameModePool.Last().FriendlyName);
        }

        // Groups

        [Test]
        public void TestMultipleGroupPoolModified() {
            var state = new ProtocolState() {
                Groups = {
                    new GroupModel() {
                        Uid = "1",
                        Type = GroupModel.Team,
                        FriendlyName = "Boring Group 1"
                    },
                    new GroupModel() {
                        Uid = "2",
                        Type = GroupModel.Team,
                        FriendlyName = "Boring Group 2"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Groups = new List<GroupModel>() {
                    new GroupModel() {
                        Uid = "1",
                        Type = GroupModel.Team,
                        FriendlyName = "Fun Group 1"
                    },
                    new GroupModel() {
                        Uid = "2",
                        Type = GroupModel.Team,
                        FriendlyName = "Fun Group 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Groups.Count);
            Assert.AreEqual("Fun Group 1", state.Groups.First().FriendlyName);
            Assert.AreEqual("Fun Group 2", state.Groups.Last().FriendlyName);
        }

        [Test]
        public void TestGroupPoolModifiedAndGroupPoolInserted() {
            var state = new ProtocolState() {
                Groups = {
                    new GroupModel() {
                        Uid = "1",
                        Type = GroupModel.Team,
                        FriendlyName = "Boring Group 1"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Groups = new List<GroupModel>() {
                    new GroupModel() {
                        Uid = "1",
                        Type = GroupModel.Team,
                        FriendlyName = "Fun Group 1"
                    },
                    new GroupModel() {
                        Uid = "2",
                        Type = GroupModel.Team,
                        FriendlyName = "Fun Group 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Groups.Count);
            Assert.AreEqual("Fun Group 1", state.Groups.First().FriendlyName);
            Assert.AreEqual("Fun Group 2", state.Groups.Last().FriendlyName);
        }

        // Items

        [Test]
        public void TestMultipleItemPoolModified() {
            var state = new ProtocolState() {
                Items = {
                    new ItemModel() {
                        Name = "1",
                        FriendlyName = "Boring Item 1"
                    },
                    new ItemModel() {
                        Name = "2",
                        FriendlyName = "Boring Item 2"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Items = new List<ItemModel>() {
                    new ItemModel() {
                        Name = "1",
                        FriendlyName = "Fun Item 1"
                    },
                    new ItemModel() {
                        Name = "2",
                        FriendlyName = "Fun Item 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual("Fun Item 1", state.Items.First().FriendlyName);
            Assert.AreEqual("Fun Item 2", state.Items.Last().FriendlyName);
        }

        [Test]
        public void TestItemPoolModifiedAndItemPoolInserted() {
            var state = new ProtocolState() {
                Items = {
                    new ItemModel() {
                        Name = "1",
                        FriendlyName = "Boring Item 1"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Items = new List<ItemModel>() {
                    new ItemModel() {
                        Name = "1",
                        FriendlyName = "Fun Item 1"
                    },
                    new ItemModel() {
                        Name = "2",
                        FriendlyName = "Fun Item 2"
                    }
                }
            });

            Assert.AreEqual(2, state.Items.Count);
            Assert.AreEqual("Fun Item 1", state.Items.First().FriendlyName);
            Assert.AreEqual("Fun Item 2", state.Items.Last().FriendlyName);
        }

        // Settings

        [Test]
        public void TestSettingsModified() {
            var state = new ProtocolState() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Boring Name"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Fun Name"
                    }
                }
            });

            Assert.AreEqual("Fun Name", state.Settings.Current.ServerNameText);
        }

        [Test]
        public void TestSettingsMaintainedWhenNotModified() {
            var state = new ProtocolState() {
                Settings = new Settings() {
                    Current = {
                        ServerNameText = "Boring Name"
                    }
                }
            };

            state.Modified(new ProtocolStateSegment());

            Assert.AreEqual("Boring Name", state.Settings.Current.ServerNameText);
        }
    }
}
