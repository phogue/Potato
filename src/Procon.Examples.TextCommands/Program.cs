using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core;
using Procon.Core.Connections.Plugins;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Events;

namespace Procon.Examples.TextCommands {
    public class Program : RemotePlugin {

        public Program() : base() {

            // 1. Setup a command dispatch to let us know when the command has been executed.
            // Commands can be executed from within game or via the daemon
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "TestCommand",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "e",
                                Type = typeof(CommandResultArgs)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TestCommand)
                }
            });
        }
        
        public override void GenericEvent(Core.Events.GenericEventArgs e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsPluginEnabled) {

                // Plugin is enabled and Procon will now accept our commands/actions.

                // 2. Register a text command (tell it what command to call when it is matched)
                // If you wanted to follow the execution of this eventually you will find yourself in Procon.Core.Connections.TextCommands.TextCommandController.RegisterTextCommand
                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    Scope = new CommandScope() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommand>() {
                                    new TextCommand() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        PluginCommand = "TestCommand", // This will be the command name that comes through
                                        DescriptionKey = "TestCommandDescription",
                                        Commands = new List<String>() {
                                            "Test",
                                            "Blah"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        // 3. Handle the test command.
        protected CommandResultArgs TestCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs e = parameters["e"].First<CommandResultArgs>();

            TextCommandMatch match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                Console.WriteLine("Players: " + String.Join(", ", match.Players.Select(x => x.Name).ToArray()));
            }

            if (match.Maps != null && match.Maps.Count > 0) {
                Console.WriteLine("Maps: " + String.Join(", ", match.Maps.Select(x => x.Name).ToArray()));
            }

            if (match.Numeric != null && match.Numeric.Count > 0) {
                Console.WriteLine("Numeric: " + String.Join(", ", match.Numeric.Select(x => String.Format("{0:F2}", x)).ToArray()));
            }

            if (match.Delay != null) {
                Console.WriteLine("Delay: {0} (UTC+9:30 Adelaide)", match.Delay);
            }

            if (match.Interval != null) {
                Console.WriteLine("Interval: {0}", match.Interval);
            }

            if (match.Period != null) {
                Console.WriteLine("Duration: {0}", match.Period);
            }

            if (e.Now.TextCommands.Count > 1) {
                Console.WriteLine("Alternate Commands: " + String.Join(" ", e.Now.TextCommands.Skip(1).Select(x => x.PluginCommand).ToArray()));
            }

            if (match.Quotes != null && match.Quotes.Count > 0) {
                Console.WriteLine("Quotes: " + String.Join(", ", match.Quotes.Select(x => String.Format("--{0}--", x)).ToArray()));
            }

            return command.Result;
        }
    }
}
