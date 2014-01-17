#region

using System;
using System.Collections.Generic;
using System.Linq;
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield3;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Test.TextCommands.Mocks;
using Procon.Net.Protocols;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.TextCommands {
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
                    Protocol = new MockProtocol() {
                        Additional = "",
                        Password = ""
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
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

            CommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
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

            CommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Username = "Phogue",
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "!test something something something dark side"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that the parser will use the persons preferred language when executing a command.
        /// </summary>
        [Test]
        public void TestTextCommandParsingExecuteUsePreferredLanguage() {
            var security = new SecurityController().Execute() as SecurityController;

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "TestGroup"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "TestGroup",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.DiceBattlefield3,
                    "EA_63A9F96745B22DFB509C558FC8B5C50F"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
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
                    Protocol = new MockProtocol() {
                        Additional = "",
                        Password = ""
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
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

            CommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Plugin,
                Username = "Phogue",
                CommandType = CommandType.TextCommandsExecute,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        ///     Tests that a text command can be executed
        /// </summary>
        [Test]
        public void TestTextCommandParsingPreview() {
            var textCommands = new TextCommandController() {
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new MockProtocol() {
                        Additional = "",
                        Password = ""
                    }
                }.Execute() as ConnectionController
            };

            textCommands.Execute();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
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

            CommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsPreview,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ExecuteTest stuff"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
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

            CommandResult result = textCommands.Tunnel(new Command() {
                CommandType = CommandType.TextCommandsPreview,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "!test something something something dark side"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}