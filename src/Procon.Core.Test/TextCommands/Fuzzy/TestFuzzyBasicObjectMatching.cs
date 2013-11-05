using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzyBasicObjectMatching : TestFuzzyBase {

        [TestMethod]
        public void TestBasicKickPhogue() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }


        [TestMethod]
        public void TestBasicKickDiacritic() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick MrDiacritic",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerMrDiacritic
                },
                new List<Map>()
            ); 
        }

        [TestMethod]
        public void TestBasicKickSelf() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick me",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicKickPhogueNameTypo() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phouge",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicKickPhogueCommandSmallTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                { 
                    "text", 
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "kcik phogue"
                            }
                        }
                    }
                }
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [TestMethod]
        public void TestBasicKickPhogueCommandSevereTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                { 
                    "text", 
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "kik phogue"
                            }
                        }
                    }
                }
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [TestMethod]
        public void TestBasicAlternateKickPhogueCommandSmallTypo() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "get rdi of phogue",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestBasicAlternateKickPhogueCommandSevereTypo() {
            CommandResultArgs result = this.CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                { 
                    "text", 
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "getch rids of phogue"
                            }
                        }
                    }
                }
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        /// <summary>
        /// Kicks Phogue and morpheus using a comma to seperate the two items.
        /// </summary>
        [TestMethod]
        public void TestKickPhogueCommaMorpheus() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue, morpheus(aut)",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueMorpheusTruncated() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick pho, morph",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueMorpheusSevereTypo() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phage, marphius aut",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue,
                    TestFuzzyBase.PlayerMorpheus
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueIsAButterfly() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is a butterfly",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueButNotPhogueIsAButterflyWithHighSimilarity() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is perhaps not a butterfly",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickPhogueIsAButterflySmallTypo() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick phogue is a buttrfly",
                TestFuzzyBase.TextCommandKick, 
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogueIsAButterfly
                },
                new List<Map>()
            );
        }

        [TestMethod]
        public void TestKickSplitNameDoubleSubsetMatch() {
            TestFuzzyBase.AssertCommandPlayerListMapList(
                this.CreateTextCommandController(), 
                "kick say nish",
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerSayaNishino
                },
                new List<Map>()
            );
        }

        /// <summary>
        /// Tests that everything (maps, players, all things) are included in the return
        /// when a named inclusive reduction token isn't included.
        /// </summary>
        [TestMethod]
        public void TestKickAll() {
            TextCommandController textCommandController = this.CreateTextCommandController();

            TestFuzzyBase.AssertCommandPlayerListMapList(
                textCommandController,
                "kick all",
                TestFuzzyBase.TextCommandKick,
                textCommandController.Connection.GameState.PlayerList,
                textCommandController.Connection.GameState.MapPool
            );
        }
    }
}
