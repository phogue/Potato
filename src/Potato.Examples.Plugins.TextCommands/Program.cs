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
using System;
using System.Collections.Generic;
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Plugins;

namespace Potato.Examples.Plugins.TextCommands {
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

        public Program() : base() {

            // 1. Setup a command dispatch to let us know when the command has been executed.
            // Commands can be executed from within game or via the daemon
            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "FuzzyCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = FuzzyCommand
                },
                new CommandDispatch() {
                    Name = "RouteCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = RouteCommand
                }
            });
        }
        
        public override void GenericEvent(GenericEvent e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsEnabled) {

                // Plugin is enabled and Potato will now accept our commands/actions.

                // 2. Register a text command (tell it what command to call when it is matched)
                // If you wanted to follow the execution of this eventually you will find yourself in Potato.Core.Connections.TextCommands.TextCommandController.RegisterTextCommand
                Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    Scope = new CommandScopeModel() {
                        ConnectionGuid = ConnectionGuid
                    },
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginGuid = PluginGuid,
                                        Parser = TextCommandParserType.Fuzzy,
                                        PluginCommand = "FuzzyCommand", // This will be the command name that comes through
                                        Description = "FuzzyCommandDescription",
                                        // When using the fuzzy parser you just need to supply keywords to pickup in a text command
                                        Commands = new List<string>() {
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
                Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    Scope = new CommandScopeModel() {
                        ConnectionGuid = ConnectionGuid
                    },
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginGuid = PluginGuid,
                                        Parser = TextCommandParserType.Route,
                                        PluginCommand = "RouteCommand", // This will be the command name that comes through
                                        Description = "RouteCommandDescription",
                                        // When using the route parser the structure must be identical
                                        // to one of the command "routes" supplied below.
                                        Commands = new List<string>() {
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
        protected ICommandResult FuzzyCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var e = parameters["e"].First<ICommandResult>();

            var match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                Console.WriteLine("Players: " + string.Join(", ", match.Players.Select(x => x.Name).ToArray()));
            }

            if (match.Maps != null && match.Maps.Count > 0) {
                Console.WriteLine("Maps: " + string.Join(", ", match.Maps.Select(x => x.Name).ToArray()));
            }

            if (match.Numeric != null && match.Numeric.Count > 0) {
                Console.WriteLine("Numeric: " + string.Join(", ", match.Numeric.Select(x => string.Format("{0:F2}", x)).ToArray()));
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
                Console.WriteLine("Alternate Commands: " + string.Join(" ", e.Now.TextCommands.Skip(1).Select(x => x.PluginCommand).ToArray()));
            }

            if (match.Quotes != null && match.Quotes.Count > 0) {
                Console.WriteLine("Quotes: " + string.Join(", ", match.Quotes.Select(x => string.Format("--{0}--", x)).ToArray()));
            }

            return command.Result;
        }

        // 3. Handle the test route command.
        protected ICommandResult RouteCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var e = parameters["e"].First<ICommandResult>();

            var match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                Console.WriteLine("Players: " + string.Join(", ", match.Players.Select(x => x.Name).ToArray()));
            }

            if (match.Maps != null && match.Maps.Count > 0) {
                Console.WriteLine("Maps: " + string.Join(", ", match.Maps.Select(x => x.Name).ToArray()));
            }

            if (match.Numeric != null && match.Numeric.Count > 0) {
                Console.WriteLine("Numeric: " + string.Join(", ", match.Numeric.Select(x => string.Format("{0:F2}", x)).ToArray()));
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
                Console.WriteLine("Alternate Commands: " + string.Join(" ", e.Now.TextCommands.Skip(1).Select(x => x.PluginCommand).ToArray()));
            }

            if (match.Quotes != null && match.Quotes.Count > 0) {
                Console.WriteLine("Quotes: " + string.Join(", ", match.Quotes.Select(x => string.Format("--{0}--", x)).ToArray()));
            }

            return command.Result;
        }
    }
}
