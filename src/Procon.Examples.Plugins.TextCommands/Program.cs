using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Plugins;

namespace Procon.Examples.Plugins.TextCommands {
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {

            // 1. Setup a command dispatch to let us know when the command has been executed.
            // Commands can be executed from within game or via the daemon
            this.AppendDispatchHandlers(new Dictionary<CommandDispatch, CommandDispatchHandler>() {
                {
                    new CommandDispatch() {
                        Name = "FuzzyCommand",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "e",
                                Type = typeof(ICommandResult)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.FuzzyCommand)
                },
                {
                    new CommandDispatch() {
                        Name = "RouteCommand",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "e",
                                Type = typeof(ICommandResult)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RouteCommand)
                }
            });
        }
        
        public override void GenericEvent(GenericEvent e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsEnabled) {

                // Plugin is enabled and Procon will now accept our commands/actions.

                // 2. Register a text command (tell it what command to call when it is matched)
                // If you wanted to follow the execution of this eventually you will find yourself in Procon.Core.Connections.TextCommands.TextCommandController.RegisterTextCommand
                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    ScopeModel = new CommandScopeModel() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        Parser = TextCommandParserType.Fuzzy,
                                        PluginCommand = "FuzzyCommand", // This will be the command name that comes through
                                        DescriptionKey = "FuzzyCommandDescription",
                                        // When using the fuzzy parser you just need to supply keywords to pickup in a text command
                                        Commands = new List<String>() {
                                            "Test",
                                            "Blah",
                                            "Fuzzy"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                // Register a route command, which is a very specific format of text command
                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    ScopeModel = new CommandScopeModel() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        Parser = TextCommandParserType.Route,
                                        PluginCommand = "RouteCommand", // This will be the command name that comes through
                                        DescriptionKey = "RouteCommandDescription",
                                        // When using the route parser the structure must be identical
                                        // to one of the command "routes" supplied below.
                                        Commands = new List<String>() {
                                            "route",
                                            "route :player",
                                            "route :player :text",
                                            "route :map",
                                            "route :map :text",
                                            "route :number",
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        // 3. Handle the test fuzzy command.
        protected ICommandResult FuzzyCommand(ICommand command, Dictionary<String, CommandParameter> parameters) {
            ICommandResult e = parameters["e"].First<ICommandResult>();

            TextCommandMatchModel match = e.Now.TextCommandMatches.First();

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

        // 3. Handle the test route command.
        protected ICommandResult RouteCommand(ICommand command, Dictionary<String, CommandParameter> parameters) {
            ICommandResult e = parameters["e"].First<ICommandResult>();

            TextCommandMatchModel match = e.Now.TextCommandMatches.First();

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
