using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;

namespace Procon.Core.Test.Plugins {
    using Procon.Core.Connections.Plugins;
    [TestFixture]
    public class TestPluginsEnabled {

        /// <summary>
        /// Tests that a plugin can be enabled if the parameter matches up to an existing plugin
        /// and the user has permission
        /// </summary>
        [Test]
        public void TestPluginEnable() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                plugins.Plugins.First().PluginGuid.ToString()
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that a plugin will return false if it is already enabled
        /// </summary>
        [Test]
        public void TestPluginEnableAlreadyEnabled() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                plugins.Plugins.First().PluginGuid.ToString()
                            }
                        }
                    }
                }
            });

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                plugins.Plugins.First().PluginGuid.ToString()
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.Status);
        }

        /// <summary>
        /// Tests that a plugin will return a DoesNotExist message if the player guid does not
        /// match up to any loaded plugin
        /// </summary>
        [Test]
        public void TestPluginEnableDoesNotExist() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }
        /// <summary>
        /// Tests that a parameter is validated for a conversion to a GUID
        /// </summary>
        [Test]
        public void TestPluginEnableInvalidParameter() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "This is an invalid GUID"
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Tests that a plugin cannot be enabled if the user has insufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestPluginEnableInsufficientPermission() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Remote,
                Username = "Phogue",
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                plugins.Plugins.First().PluginGuid.ToString()
                            }
                        }
                    }
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that enabling a plugin will allow commands within that plugin to be executed.
        /// </summary>
        [Test]
        public void TestPluginEnableCommandSuccessful() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                plugins.Plugins.First().PluginGuid.ToString()
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);

            result = plugins.Execute(new Command() {
                Name = "TestPluginsEnabledCommandResult",
                Username = "Phogue",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("Return Message", result.Message);
        }
    }
}
