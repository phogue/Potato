using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Connections.TextCommands;
using Procon.Net.Actions;
using Procon.Net.Data;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicObjectMatching : TestFuzzyBase {

        [Test]
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


        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
    }
}
