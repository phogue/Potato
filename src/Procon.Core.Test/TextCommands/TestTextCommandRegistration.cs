using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Test.TextCommands {
    using Procon.Core.Connections.TextCommands;

    [TestFixture]
    public class TestTextCommandRegistration {

        #region Register

        /// <summary>
        /// Tests that we can register a command via the command interface.
        /// </summary>
        [Test]
        public void TestTextCommandRegister() {
            TextCommandController textCommands = new TextCommandController();

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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
        /// Tests that adding the same command twice (same plugin uid & plugin command) will 
        /// only succeed the first time around.
        /// </summary>
        [Test]
        public void TestTextCommandRegisterDuplication() {
            TextCommandController textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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
        /// Tests that commands can only be executed if the user has sufficient permissions.
        /// </summary>
        [Test]
        public void TestTextCommandRegisterInsufficientPermission() {
            TextCommandController textCommands = new TextCommandController();

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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

        #endregion

        #region Unregister

        /// <summary>
        /// Tests that a command can successfully remove a text command.
        /// </summary>
        [Test]
        public void TestTextCommandUnregister() {
            TextCommandController textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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
        /// Tests the unregister command fails with the correct status if the command does not exist.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterDoesNotExist() {
            TextCommandController textCommands = new TextCommandController();

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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
        /// Tests that a command cannot be unregistered without the correct permissions to do so.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterInsufficientPermission() {
            TextCommandController textCommands = new TextCommandController();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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

            CommandResultArgs result = textCommands.Tunnel(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommand>() {
                                new TextCommand() {
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

        #endregion
    }
}
