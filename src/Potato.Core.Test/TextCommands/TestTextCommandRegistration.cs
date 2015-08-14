#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Core.Connections.TextCommands;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

#endregion

namespace Potato.Core.Test.TextCommands {
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

            var result = textCommands.Tunnel(new Command() {
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
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
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
            var guid = Guid.NewGuid();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = guid,
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = guid,
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.CommandResultType);
            Assert.AreEqual(1, textCommands.TextCommands.Count);
        }

        /// <summary>
        ///     Tests that commands can only be executed if the user has sufficient permissions.
        /// </summary>
        [Test]
        public void TestTextCommandRegisterInsufficientPermission() {
            var textCommands = new TextCommandController();

            var result = textCommands.Tunnel(new Command() {
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
                                    PluginGuid = Guid.NewGuid(),
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a command can successfully remove a text command.
        /// </summary>
        [Test]
        public void TestTextCommandUnregister() {
            var textCommands = new TextCommandController();
            var guid = Guid.NewGuid();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = guid,
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
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

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = guid,
                                    PluginCommand = "Command1"
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual(0, textCommands.TextCommands.Count);
        }

        /// <summary>
        ///     Tests the unregister command fails with the correct status if the command does not exist.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterDoesNotExist() {
            var textCommands = new TextCommandController();

            var result = textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsUnregister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = Guid.NewGuid(),
                                    PluginCommand = "Command1"
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a command cannot be unregistered without the correct permissions to do so.
        /// </summary>
        [Test]
        public void TestTextCommandUnregisterInsufficientPermission() {
            var textCommands = new TextCommandController();
            var guid = Guid.NewGuid();

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            TextCommands = new List<TextCommandModel>() {
                                new TextCommandModel() {
                                    PluginGuid = guid,
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
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

            var result = textCommands.Tunnel(new Command() {
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
                                    PluginGuid = guid,
                                    PluginCommand = "Command1",
                                    Commands = new List<string>() {
                                        "RegisterTest"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}