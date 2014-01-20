#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands {
    [TestFixture]
    public class TestTextCommandRegistration {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that we can register a command via the command interface.
        /// </summary>
        [Test]
        public void TestTextCommandRegister() {
            var textCommands = new TextCommandController();

            ICommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("RegisterTest", textCommands.TextCommands.First().Commands.First());
        }

        /// <summary>
        ///     Tests that adding the same command twice (same plugin uid & plugin command) will
        ///     only succeed the first time around.
        /// </summary>
        [Test]
        public void TestTextCommandRegisterDuplication() {
            var textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            ICommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.Status);
            Assert.AreEqual(1, textCommands.TextCommands.Count);
        }

        /// <summary>
        ///     Tests that commands can only be executed if the user has sufficient permissions.
        /// </summary>
        [Test]
        public void TestTextCommandRegisterInsufficientPermission() {
            var textCommands = new TextCommandController();

            ICommandResult result = textCommands.Tunnel(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a command can successfully remove a text command.
        /// </summary>
        [Test]
        public void TestTextCommandUnregister() {
            var textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            // Test that the initial command was entered before we attempt to remove it.
            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("RegisterTest", textCommands.TextCommands.First().Commands.First());

            ICommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1"
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(0, textCommands.TextCommands.Count);
        }

        /// <summary>
        ///     Tests the unregister command fails with the correct status if the command does not exist.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterDoesNotExist() {
            var textCommands = new TextCommandController();

            ICommandResult result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1"
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Tests that a command cannot be unregistered without the correct permissions to do so.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterInsufficientPermission() {
            var textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            // Test that the initial command was entered before we attempt to remove it.
            Assert.AreEqual(1, textCommands.TextCommands.Count);
            Assert.AreEqual("RegisterTest", textCommands.TextCommands.First().Commands.First());

            ICommandResult result = textCommands.Tunnel(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginUid = "Plugin1",
                                    PluginCommand = "Command1",
                                    Commands = new List<String>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}