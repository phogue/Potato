using System;
using System.Collections.Generic;
using System.Linq;
using Myrcon.Plugins.Test.Tests;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Plugins;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Myrcon.Plugins.Test {

    public class Program : PluginController {
        //Ignore this: - Actually, depending on how you wanna save info for plugins, the actually plugin could save it here.
        // For now, it's in PluginAPI
        // 
        // protected override void WriteConfig(System.Xml.Linq.XElement xNamespace) { }
        // public override void Dispose() { }

        private List<TextCommandModel> Commands { get; set; }

        public List<ICoreController> Tests = new List<ICoreController>() {
            new TestPluginsSerialization(),
            new TestPluginsEnabled(),
            new TestPluginsIsolation(),
            new TestPluginsWebUi(),
            new TestPluginsCommands()
        };

        public Program() : base() {
            //this.Author = "Phogue";
            //this.Website = "http://phogue.net";
            //this.Description = "herro";

            this.Commands = new List<TextCommandModel>();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "HelpCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = this.HelpCommand
                },
                new CommandDispatch() {
                    Name = "KillCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = this.KillCommand
                },
                new CommandDispatch() {
                    Name = "TestCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "e",
                            Type = typeof(ICommandResult)
                        }
                    },
                    Handler = this.TestCommand
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
                    Handler = this.RegisterTextCommandPreview
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
                    Handler = this.RegisterTextCommandExecuted
                }
            });
        }

        protected override IList<ICoreController> TunnelExecutableObjects(ICommand command) {
            return this.Tests;
        }

        protected List<string> ShortCommandList(ICommandResult e) {
            return this.Commands.Select(x => x.Commands.FirstOrDefault()).ToList();
        }

        protected ICommandResult HelpCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult e = parameters["e"].First<ICommandResult>();

            NetworkAction output = new NetworkAction() {
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

                if (this.GameState.Settings.Maximum.ChatLinesCount.HasValue == true) {
                    foreach (string line in String.Join(", ", this.ShortCommandList(e).ToArray()).WordWrap(this.GameState.Settings.Maximum.ChatLinesCount.Value)) {
                        output.Now.Content.Add(line);
                    }
                }
            }
            else {
                foreach (TextCommandModel alternate in e.Now.TextCommands.Skip(1)) {
                    //string description = String.Format("> {0}: {1}", alternate.Commands.FirstOrDefault(), this.NamespacePlayerLoc(e.Now.Players.First(), this.GetType().Namespace + "." + alternate.PluginUid, alternate.PluginCommand));
                    string description = String.Format("> {0}", alternate.Commands.FirstOrDefault());

                    if (this.GameState.Settings.Maximum.ChatLinesCount.HasValue == true) {
                        foreach (string line in description.WordWrap(this.GameState.Settings.Maximum.ChatLinesCount.Value)) {
                            output.Now.Content.Add(line);
                        }
                    }
                }
            }

            this.Action(output);

            return command.Result;
        }

        protected ICommandResult KillCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult e = parameters["e"].First<ICommandResult>();

            TextCommandMatchModel match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                this.Action(new DeferredAction<NetworkAction>() {
                    Action = new NetworkAction() {
                        ActionType = NetworkActionType.NetworkPlayerKill,
                        Scope = {
                            Players = new List<PlayerModel>(match.Players),
                                Content = new List<String>() {
                                "Testing"
                            }
                        }
                    },
                    Sent = (action, requests) => {
                        Console.WriteLine("KillCommand: {0}", action.Uid);

                        foreach (IPacket packet in requests) {
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

        protected ICommandResult TestCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult e = parameters["e"].First<ICommandResult>();

            NetworkAction output = new NetworkAction() {
                ActionType = NetworkActionType.NetworkTextSay
            };

            Console.WriteLine(e.Now.TextCommands.First().DescriptionKey);

            TextCommandMatchModel match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                output.Now.Content.Add("Players: " + String.Join(", ", match.Players.Select(x => x.Name).ToArray()));
            }

            if (match.Maps != null && match.Maps.Count > 0) {
                output.Now.Content.Add("Maps: " + String.Join(", ", match.Maps.Select(x => x.Name).ToArray()));
            }

            if (match.Numeric != null && match.Numeric.Count > 0) {
                output.Now.Content.Add("Numeric: " + String.Join(", ", match.Numeric.Select(x => String.Format("{0:F2}", x)).ToArray()));
            }

            if (match.Delay != null) {
                output.Now.Content.Add(String.Format("Delay: {0} (UTC+9:30 Adelaide)", match.Delay));
            }

            if (match.Interval != null) {
                output.Now.Content.Add(String.Format("Interval: {0}", match.Interval));
            }

            if (match.Period != null) {
                output.Now.Content.Add(String.Format("Duration: {0}", match.Period));
            }

            if (e.Now.TextCommands.Count > 1) {
                output.Now.Content.Add("Alternate Commands: " + String.Join(" ", e.Now.TextCommands.Skip(1).Select(x => x.PluginCommand).ToArray()));
            }

            if (match.Quotes != null && match.Quotes.Count > 0) {
                output.Now.Content.Add("Quotes: " + String.Join(", ", match.Quotes.Select(x => String.Format("--{0}--", x)).ToArray()));
            }

            this.Action(output);

            return command.Result;
        }

        public ICommandResult RegisterTextCommandPreview(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public ICommandResult RegisterTextCommandExecuted(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public override void GenericEvent(GenericEvent e) {
            Console.WriteLine("Test Plugin ({0}) Event: {1}", this.PluginGuid, e.Name);

            if (e.GenericEventType == GenericEventType.TextCommandRegistered) {
                this.Commands.Add(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.TextCommandUnregistered) {
                this.Commands.Remove(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.PluginsEnabled) {

                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    ScopeModel = new CommandScopeModel() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        PluginCommand = "TestCommand",
                                        DescriptionKey = "TestCommandDescription",
                                        Commands = new List<String>() {
                                            "Test"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    ScopeModel = new CommandScopeModel() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        PluginCommand = "KillCommand",
                                        DescriptionKey = "KillCommandDescription",
                                        Commands = new List<String>() {
                                            "Kill"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                this.Bubble(new Command() {
                    CommandType = CommandType.TextCommandsRegister,
                    ScopeModel = new CommandScopeModel() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                TextCommands = new List<TextCommandModel>() {
                                    new TextCommandModel() {
                                        PluginUid = this.PluginGuid.ToString(),
                                        PluginCommand = "HelpCommand",
                                        Priority = 100,
                                        DescriptionKey = "HelpCommandDescription",
                                        Commands = new List<String>() {
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