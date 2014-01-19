#region

using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicObjectMatching : TestTextCommandParserBase {
        [Test]
        public void TestBasicAlternateKickPhogueCommandSevereTypo() {
            CommandResult result = CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                {"text", new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
                            "getch rids of phogue"
                        }
                    }
                }}
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [Test]
        public void TestBasicAlternateKickPhogueCommandSmallTypo() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "get rdi of phogue", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        [Test]
        public void TestBasicKickDiacritic() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick MrDiacritic", TextCommandKick, new List<PlayerModel>() {
                PlayerMrDiacritic
            }, new List<MapModel>());
        }

        [Test]
        public void TestBasicKickPhogue() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        [Test]
        public void TestBasicKickPhogueCommandSevereTypo() {
            CommandResult result = CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                {"text", new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
                            "kik phogue"
                        }
                    }
                }}
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [Test]
        public void TestBasicKickPhogueCommandSmallTypo() {
            CommandResult result = CreateTextCommandController().ExecuteTextCommand(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                {"text", new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
                            "kcik phogue"
                        }
                    }
                }}
            });

            // No event should be fired. The command has a much lower tolerance for typos (good thing)
            Assert.IsNull(result, "Argument has passed, but should have failed due to severe typo in command");
        }

        [Test]
        public void TestBasicKickPhogueNameTypo() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phouge", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        [Test]
        public void TestBasicKickSelf() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick me", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueButNotPhogueIsAButterflyWithHighSimilarity() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue is perhaps not a butterfly", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        /// <summary>
        ///     Kicks Phogue and morpheus using a comma to seperate the two items.
        /// </summary>
        [Test]
        public void TestKickPhogueCommaMorpheus() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue, morpheus(aut)", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueIsAButterfly() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue is a butterfly", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueIsAButterflySmallTypo() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phogue is a buttrfly", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogueIsAButterfly
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueMorpheusSevereTypo() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick phage, marphius aut", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickPhogueMorpheusTruncated() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick pho, morph", TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestKickSplitNameDoubleSubsetMatch() {
            AssertCommandPlayerListMapList(CreateTextCommandController(), "kick say nish", TextCommandKick, new List<PlayerModel>() {
                PlayerSayaNishino
            }, new List<MapModel>());
        }
    }
}