﻿#region Copyright
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
using NUnit.Framework;
using Potato.Core.Connections.TextCommands;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Models;

namespace Potato.Core.Test.TextCommands.Route {
    [TestFixture]
    public class TestRoute : TestTextCommandParserBase {
        [Test]
        public void TestSingleCommandMatching() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertExecutedCommand(ExecuteTextCommand(textCommandController, "Command"), command);
        }

        [Test]
        public void TestCommandNumber() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :number"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertExecutedCommandAgainstNumericValue(ExecuteTextCommand(textCommandController, "Command 10"), command, 10);
        }

        [Test]
        public void TestCommandNumberSentence() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :number :text"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            ICommandResult result = ExecuteTextCommand(textCommandController, "command 20 for something and something");

            AssertExecutedCommandAgainstNumericValue(result, command, 20);
            AssertExecutedCommandAgainstSentencesList(result, command, new List<String>() {
                "for something and something"
            });
        }

        [Test]
        public void TestCommandTextSingleWord() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :text"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertCommandSentencesList(textCommandController, "Command hello", command, new List<String>() {
                "hello"
            });
        }

        [Test]
        public void TestCommandTextSentence() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :text"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertCommandSentencesList(textCommandController, "Command hello, this is a longer sentence!", command, new List<String>() {
                "hello, this is a longer sentence!"
            });
        }

        [Test]
        public void TestCommandPlayer() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertCommandPlayerListMapList(textCommandController, "Command phogue", command, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }

        [Test]
        public void TestCommandPlayerPlayer() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player :player"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertCommandPlayerListMapList(textCommandController, "Command phogue morpheus", command, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestCommandPlayerMap() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :map"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            AssertCommandPlayerListMapList(textCommandController, "Command port valdez", command, new List<PlayerModel>() , new List<MapModel>() {
                MapPortValdez
            });
        }

        [Test]
        public void TestCommandPlayerPlayerText() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player :player :text"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            ICommandResult result = ExecuteTextCommand(textCommandController, "command phogue morpheus for something and something");

            AssertExecutedCommandAgainstSentencesList(result, command, new List<String>() {
                "for something and something"
            });

            AssertExecutedCommandAgainstPlayerListMapList(result, command, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestCommandPlayerTextPlayer() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player :text :player"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            ICommandResult result = ExecuteTextCommand(textCommandController, "command phogue something, something, something dark side morpheus");

            AssertExecutedCommandAgainstSentencesList(result, command, new List<String>() {
                "something, something, something dark side"
            });

            AssertExecutedCommandAgainstPlayerListMapList(result, command, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestCommandPlayerTextPlayerNumber() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player :text :player :number"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            ICommandResult result = ExecuteTextCommand(textCommandController, "command phogue something, something, something dark side morpheus 25");

            AssertExecutedCommandAgainstSentencesList(result, command, new List<String>() {
                "something, something, something dark side"
            });

            AssertExecutedCommandAgainstNumericValue(result, command, 25);

            AssertExecutedCommandAgainstPlayerListMapList(result, command, new List<PlayerModel>() {
                PlayerPhogue,
                PlayerMorpheus
            }, new List<MapModel>());
        }

        [Test]
        public void TestCommandPlayerForText() {
            TextCommandController textCommandController = CreateTextCommandController();

            TextCommandModel command = new TextCommandModel() {
                Commands = new List<string>() {
                    "Command :player for :text"
                },
                Parser = TextCommandParserType.Route,
                PluginCommand = "Command",
                Description = "Command"
            };

            textCommandController.TextCommands.Add(command);

            ICommandResult result = ExecuteTextCommand(textCommandController, "command phogue for some time");

            AssertExecutedCommandAgainstSentencesList(result, command, new List<String>() {
                "some time"
            });

            AssertExecutedCommandAgainstPlayerListMapList(result, command, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>());
        }
    }
}
