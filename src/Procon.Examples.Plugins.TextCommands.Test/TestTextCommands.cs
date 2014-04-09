using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Shared;
using Procon.Net.Shared;
using Procon.Net.Shared.Sandbox;

namespace Procon.Examples.Plugins.TextCommands.Test {
    /// <summary>
    /// This is even lazier than the other examples. There is zero testing as
    /// events are fired and forgotten by Procon.
    /// </summary>
    /// <remarks>This just allows you convientient way of testing plugins via debugging</remarks>
    [TestFixture]
    public class TestTextCommands {

        /// <summary>
        /// Creates a connection object with a MockGame, including players and maps etc.
        /// </summary>
        /// <remarks>The plugin will be enabled during this process.</remarks>
        /// <returns></returns>
        protected ConnectionController CreateConnection() {
            ISandboxProtocolController protocol = new SandboxProtocolController() {
                SandboxedProtocol = new MockGame()
            };

            protocol.Setup(new ProtocolSetup() {
                Hostname = "localhost",
                Port = 9000
            });

            ConnectionController connection = new ConnectionController() {
                // This won't actually connect to anything.
                // It's just a mock so the GameState is available to be modified.
                // See MockGame for all the mock data we create.
                Protocol = protocol
            };

            // 1. When this is called you will see the constructor in the plugin executed.
            // Procon.Examples.TextCommands.Program 
            connection.Execute();

            // Note: Enable the single plugin that was loaded, otherwise it won't recieve any tunneled commands or events.

            // 2. When this is executed you will see a GenericEvent fired. Place your breakpoint in
            // Procon.Examples.TextCommands.Program.GenericEvent -> PluginsPluginEnabled
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                ScopeModel = {
                    PluginGuid = connection.Plugins.LoadedPlugins.First().PluginGuid
                }
            });

            return connection;
        }

        /// <summary>
        /// We just test here that "test ..." will eventually execute the command 
        /// Set a breakpoint within Procon.Examples.TextCommands.TestCommand
        /// </summary>
        [Test]
        public void TestBasicCommand() {
            ConnectionController connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                // You should see zero players and other attributes from this
                                "blah!"
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.TextCommands.FuzzyCommand
        /// </summary>
        [Test]
        public void TestCommandPhogue() {
            ConnectionController connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                // You should see a single player "phogue"
                                "phogue, hurry up and test this"
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.TextCommands.FuzzyCommand
        /// </summary>
        [Test]
        public void TestCommandScoreEqualsZero() {
            ConnectionController connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                // You should see a single player "zaeed"
                                "test players with score = 0"
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.TextCommands.RouteCommand
        /// </summary>
        [Test]
        public void TestRouteCommand() {
            ConnectionController connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "route"
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.TextCommands.RouteCommand
        /// </summary>
        [Test]
        public void TestRouteCommandPlayer() {
            ConnectionController connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                // You should see a single player "phogue"
                                "route phogue"
                            }
                        }
                    }
                }
            });
        }
    }
}
