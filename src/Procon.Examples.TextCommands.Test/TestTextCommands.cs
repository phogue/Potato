using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Shared;

namespace Procon.Examples.TextCommands.Test {
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
        protected Connection CreateConnection() {
            Connection connection = new Connection() {
                // This won't actually connect to anything.
                // It's just a mock so the GameState is available to be modified.
                // See MockGame for all the mock data we create.
                Game = new MockGame("localhost", 9000)
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
                Scope = {
                    PluginGuid = connection.Plugins.Plugins.First().PluginModel.PluginGuid
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
            Connection connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<CommandParameter>() {
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

        [Test]
        public void TestCommandPhogue() {
            Connection connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<CommandParameter>() {
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

        [Test]
        public void TestCommandScoreEqualsZero() {
            Connection connection = this.CreateConnection();

            // 3. Execute a text command. This could come from in game text or via the daemon.
            // When executed from in game the command built is identical but the content is
            // the in game text said and this text must be prefixed with "!, @ or #"
            connection.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsExecute,
                Parameters = new List<CommandParameter>() {
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
    }
}
