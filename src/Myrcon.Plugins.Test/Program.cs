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
using Myrcon.Plugins.Test.Tests;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Plugins;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Actions.Deferred;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Utils;

namespace Myrcon.Plugins.Test {

    public class Program : PluginController {

        private List<TextCommandModel> Commands { get; set; }

        public Program() : base() {
            Title = "Myrcon Test Plugin";

            Commands = new List<TextCommandModel>();

            TunnelObjects = new List<ICoreController>() {
                new TestPluginsSerialization(),
                new TestPluginsEnabled(),
                new TestPluginsIsolation(),
                new TestPluginsWebUi(),
                new TestPluginsCommands()
            };

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "HelpCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = HelpCommand
                },
                new CommandDispatch() {
                    Name = "KillCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = KillCommand
                },
                new CommandDispatch() {
                    Name = "TestCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = TestCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsRegister,
                    CommandAttributeType = CommandAttributeType.Preview,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = RegisterTextCommandPreview
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsRegister,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = RegisterTextCommandExecuted
                }
            });
        }

        protected List<string> ShortCommandList(ICommandResult e) {
            return Commands.Select(x => x.Commands.FirstOrDefault()).ToList();
        }

        protected ICommandResult HelpCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var e = parameters["e"].First<ICommandResult>();

            var output = new NetworkAction() {
                ActionType = NetworkActionType.NetworkTextSay
            };

            if (e.Now.TextCommands.Count > 1) {
                output.Now.Content.Add("Place Holder");

                /*
                // List of commands.
                this.ProxyNetworkAction(new Chat() {
                    Text = "Place Holder", //this.PlayerLoc(e.Now.Players.First(), "HelpCommandHeader", e.Now.TextCommandMatches.First().Prefix),
                    Subset = new PlayerSubset()
                });
                */

                if (ProtocolState.Settings.Maximum.ChatLinesCount.HasValue == true) {
                    foreach (var line in string.Join(", ", ShortCommandList(e).ToArray()).WordWrap(ProtocolState.Settings.Maximum.ChatLinesCount.Value)) {
                        output.Now.Content.Add(line);
                    }
                }
            }
            else {
                foreach (var alternate in e.Now.TextCommands.Skip(1)) {
                    //string description = String.Format("> {0}: {1}", alternate.Commands.FirstOrDefault(), this.NamespacePlayerLoc(e.Now.Players.First(), this.GetType().Namespace + "." + alternate.PluginUid, alternate.PluginCommand));
                    var description = string.Format("> {0}", alternate.Commands.FirstOrDefault());

                    if (ProtocolState.Settings.Maximum.ChatLinesCount.HasValue == true) {
                        foreach (var line in description.WordWrap(ProtocolState.Settings.Maximum.ChatLinesCount.Value)) {
                            output.Now.Content.Add(line);
                        }
                    }
                }
            }

            Action(output);

            return command.Result;
        }

        protected ICommandResult KillCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var e = parameters["e"].First<ICommandResult>();

            var match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                Action(new DeferredAction<NetworkAction>() {
                    Action = new NetworkAction() {
                        ActionType = NetworkActionType.NetworkPlayerKill,
                        Scope = {
                            Players = new List<PlayerModel>(match.Players),
                                Content = new List<string>() {
                                "Testing"
                            }
                        }
                    },
                    Sent = (action, requests) => {
                        Console.WriteLine("KillCommand: {0}", action.Uid);

                        foreach (var packet in requests) {
                            Console.WriteLine("KillCommand.Sent.packet: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText);
                        }
                    },
                    Each = (action, request, response) => {
                        Console.WriteLine("KillCommand.Each {0} {1} ({2}) ({3})", action.Uid, request.RequestId, request.DebugText, response.DebugText);
                    },
                    Done = (action, requests, responses) => {
                        Console.WriteLine("KillCommand.Done {0}", action.Uid);
                    },
                    Expired = (action, requests, responses) => {
                        Console.WriteLine("KillCommand.Expired {0}", action.Uid);
                    },
                    Always = action => {
                        Console.WriteLine("KillCommand.Always {0}", action.Uid);
                    }
                });
            }

            return command.Result;
        }

        protected ICommandResult TestCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var e = parameters["e"].First<ICommandResult>();

            var output = new NetworkAction() {
                ActionType = NetworkActionType.NetworkTextSay,
                Now = {
                    Content = new List<string>()
                }
            };

            Console.WriteLine(e.Now.TextCommands.First().Description);

            var match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                output.Now.Content.Add("Players: " + string.Join(", ", match.Players.Select(x => x.Name).ToArray()));
            }

            if (match.Maps != null && match.Maps.Count > 0) {
                output.Now.Content.Add("Maps: " + string.Join(", ", match.Maps.Select(x => x.Name).ToArray()));
            }

            if (match.Numeric != null && match.Numeric.Count > 0) {
                output.Now.Content.Add("Numeric: " + string.Join(", ", match.Numeric.Select(x => string.Format("{0:F2}", x)).ToArray()));
            }

            if (match.Delay != null) {
                output.Now.Content.Add(string.Format("Delay: {0} (UTC+9:30 Adelaide)", match.Delay));
            }

            if (match.Interval != null) {
                output.Now.Content.Add(string.Format("Interval: {0}", match.Interval));
            }

            if (match.Period != null) {
                output.Now.Content.Add(string.Format("Duration: {0}", match.Period));
            }

            if (e.Now.TextCommands.Count > 1) {
                output.Now.Content.Add("Alternate Commands: " + string.Join(" ", e.Now.TextCommands.Skip(1).Select(x => x.PluginCommand).ToArray()));
            }

            if (match.Quotes != null && match.Quotes.Count > 0) {
                output.Now.Content.Add("Quotes: " + string.Join(", ", match.Quotes.Select(x => string.Format("--{0}--", x)).ToArray()));
            }

            Action(output);

            return command.Result;
        }

        public ICommandResult RegisterTextCommandPreview(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public ICommandResult RegisterTextCommandExecuted(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public override void GenericEvent(GenericEvent e) {
            Console.WriteLine("Test Plugin ({0}) Event: {1}", PluginGuid, e.Name);

            if (e.GenericEventType == GenericEventType.TextCommandRegistered) {
                Commands.Add(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.TextCommandUnregistered) {
                Commands.Remove(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.PluginsEnabled) {
                
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
                                        PluginCommand = "TestCommand",
                                        Name = "Test",
                                        Description = "Tests a command, outputting the results to player chat.",
                                        Commands = new List<string>() {
                                            "Test"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

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
                                        PluginCommand = "KillCommand",
                                        Name = "Kill",
                                        Description = "Kills any matching players, sending a message to them that the action was for tesing.",
                                        Commands = new List<string>() {
                                            "Kill"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

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
                                        PluginCommand = "HelpCommand",
                                        Priority = 100,
                                        Name = "Help",
                                        Description = "Provides help about another command.",
                                        Commands = new List<string>() {
                                            "Help"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }

            base.GenericEvent(e);
        }
    }
}