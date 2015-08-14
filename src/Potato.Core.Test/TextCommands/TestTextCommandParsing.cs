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
using Potato.Core.Connections;
using Potato.Core.Connections.TextCommands;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Test.Mocks.Protocols;
using Potato.Net.Shared.Protocols;
using Potato.Net.Shared.Sandbox;

#endregion

namespace Potato.Core.Test.TextCommands {
    [TestFixture]
    public class TestTextCommandParsing {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that a text command can be executed
        /// </summary>
        [Test]
        public void TestTextCommandParsingExecute() {
            var textCommands = new TextCommandController() {
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new SandboxProtocolController() {
                        SandboxedProtocol = new MockProtocol() {
                            Additional = "",
                            Password = ""
                        }
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = Guid.NewGuid(),
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "ExecuteTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("ExecuteTest", textCommands.TextCommands.First().Commands.First());

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests a text command cannot be executed without sufficient permissions
        /// </summary>
        [Test]
        public void TestTextCommandParsingExecuteInsufficientPermissions() {
            var textCommands = new TextCommandController() {
                Shared = {
                    Security = new SecurityController().Execute() as SecurityController
                }
            };

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Authentication = {
                    Username = "Phogue"
                },
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "!test something something something dark side"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that the parser will use the persons preferred language when executing a command.
        /// </summary>
        [Test]
        public void TestTextCommandParsingExecuteUsePreferredLanguage() {
            var security = (SecurityController)new SecurityController().Execute();

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "TestGroup"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "TestGroup",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "EA_63A9F96745B22DFB509C558FC8B5C50F"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "TestGroup",
                    CommandType.TextCommandsExecute,
                    100
                })
            });

            var textCommands = new TextCommandController() {
                Shared = {
                    Security = security
                },
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new SandboxProtocolController() {
                        SandboxedProtocol = new MockProtocol() {
                            Additional = "",
                            Password = ""
                        }
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = Guid.NewGuid(),
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "ExecuteTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("ExecuteTest", textCommands.TextCommands.First().Commands.First());

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Plugin,
                Authentication = {
                    Username = "Phogue"
                },
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a text command can be executed
        /// </summary>
        [Test]
        public void TestTextCommandParsingPreview() {
            var textCommands = new TextCommandController() {
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new SandboxProtocolController() {
                        SandboxedProtocol = new MockProtocol() {
                            Additional = "",
                            Password = ""
                        }
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = Guid.NewGuid(),
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "ExecuteTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("ExecuteTest", textCommands.TextCommands.First().Commands.First());

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsPreview,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests a text command cannot be previewed without sufficient permissions
        /// </summary>
        [Test]
        public void TestTextCommandParsingPreviewInsufficientPermissions() {
            var textCommands = new TextCommandController() {
                Shared = {
                    Security = new SecurityController().Execute() as SecurityController
                }
            };

            var result = textCommands.Tunnel(new Command() {
                CommandType = CommandType.TextCommandsPreview,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "!test something something something dark side"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}