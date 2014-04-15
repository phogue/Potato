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
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Net.Shared.Models;

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicObjectMatching : TestTextCommandParserBase {
        [Test]
        public void TestBasicAlternateKickPhogueCommandSevereTypo() {
            ICommandResult result = CreateTextCommandController().TextCommandsExecute(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Local
            }, new Dictionary<String, ICommandParameter>() {
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
            ICommandResult result = CreateTextCommandController().TextCommandsExecute(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Local
            }, new Dictionary<String, ICommandParameter>() {
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
            ICommandResult result = CreateTextCommandController().TextCommandsExecute(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Local
            }, new Dictionary<String, ICommandParameter>() {
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