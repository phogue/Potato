using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Net;

namespace TestPlugin {
    using Tests;
    using Procon.Core;
    using Procon.Core.Events;
    using Procon.Core.Connections.Plugins;
    using Procon.Core.Connections.TextCommands;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;

    public class Program : RemotePlugin {
        //Ignore this: - Actually, depending on how you wanna save info for plugins, the actually plugin could save it here.
        // For now, it's in PluginAPI
        // 
        // protected override void WriteConfig(System.Xml.Linq.XElement xNamespace) { }
        // public override void Dispose() { }

        private List<TextCommand> Commands { get; set; }

        public List<IExecutableBase> Tests = new List<IExecutableBase>() {
            new TestPluginsSerialization(),
            new TestPluginsEnabled(),
            new TestPluginsIsolation(),
            new TestPluginsWebUi(),
            new TestPluginsCommands()
        };

        /// <summary>
        /// A list of request id's we're waiting for responses on. This is not correct as the operation is asynchronous, so
        /// there is a good chance the response will be missed.
        /// </summary>
        protected List<int?> KillCommandWaitingRequestIds { get; set; } 

        public Program() : base() {
            //this.Author = "Phogue";
            //this.Website = "http://phogue.net";
            //this.Description = "herro";

            this.Commands = new List<TextCommand>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "HelpCommand",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "e",
                                Type = typeof(CommandResultArgs)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.HelpCommand)
                }, {
                    new CommandAttribute() {
                        Name = "KillCommand",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "e",
                                Type = typeof(CommandResultArgs)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.KillCommand)
                }, {
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
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.TextCommandsRegister,
                        CommandAttributeType = CommandAttributeType.Preview,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "textCommand",
                                Type = typeof(TextCommand)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RegisterTextCommandPreview)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.TextCommandsRegister,
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "textCommand",
                                Type = typeof(TextCommand)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RegisterTextCommandExecuted)
                }
            });
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return this.Tests;
        }

        protected List<string> ShortCommandList(CommandResultArgs e) {
            return this.Commands.Select(x => x.Commands.FirstOrDefault()).ToList();
        }

        protected CommandResultArgs HelpCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs e = parameters["e"].First<CommandResultArgs>();

            Chat output = new Chat() {
                Now = new NetworkActionData() {
                    Content = new List<String>()
                }
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
                foreach (TextCommand alternate in e.Now.TextCommands.Skip(1)) {
                    //string description = String.Format("> {0}: {1}", alternate.Commands.FirstOrDefault(), this.NamespacePlayerLoc(e.Now.Players.First(), this.GetType().Namespace + "." + alternate.PluginUid, alternate.PluginCommand));
                    string description = String.Format("> {0}", alternate.Commands.FirstOrDefault());

                    if (this.GameState.Settings.Maximum.ChatLinesCount.HasValue == true) {
                        foreach (string line in description.WordWrap(this.GameState.Settings.Maximum.ChatLinesCount.Value)) {
                            output.Now.Content.Add(line);
                        }
                    }
                }
            }

            this.ProxyNetworkAction(output);

            return command.Result;
        }

        public override void ClientEvent(ClientEventArgs e) {
            base.ClientEvent(e);

            if (e.EventType == ClientEventType.ClientPacketReceived) {
                if (this.KillCommandWaitingRequestIds != null && this.KillCommandWaitingRequestIds.Contains(e.Packet.RequestId) == true) {
                    Console.WriteLine("RECV: {0} {1} {2} {3}", e.Packet.Origin, e.Packet.Type, e.Packet.RequestId, e.Packet.DebugText);
                }
            }
        }

        protected CommandResultArgs KillCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs e = parameters["e"].First<CommandResultArgs>();

            TextCommandMatch match = e.Now.TextCommandMatches.First();

            if (match.Players != null && match.Players.Count > 0) {
                CommandResultArgs result = this.ProxyNetworkAction(new Kill() {
                    Scope = {
                        Players = new List<Player>(match.Players),
                        Content = new List<String>() {
                            "Testing"
                        }
                    }
                });

                this.KillCommandWaitingRequestIds = new List<int?>();

                foreach (IPacket packet in result.Now.Packets) {
                    this.KillCommandWaitingRequestIds.Add(packet.RequestId);
                    Console.WriteLine("SEND: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText);
                }
            }

            return command.Result;
        }

        protected CommandResultArgs TestCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs e = parameters["e"].First<CommandResultArgs>();

            Chat output = new Chat() {
                Now = new NetworkActionData() {
                    Content = new List<String>()
                }
            };

            Console.WriteLine(e.Now.TextCommands.First().DescriptionKey);

            TextCommandMatch match = e.Now.TextCommandMatches.First();

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

            this.ProxyNetworkAction(output);

            return command.Result;
        }

        public CommandResultArgs RegisterTextCommandPreview(Command command, Dictionary<String, CommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public CommandResultArgs RegisterTextCommandExecuted(Command command, Dictionary<String, CommandParameter> parameters) {
            //TextCommand textCommand = parameters["textCommand"].First<TextCommand>();

            return command.Result;
        }

        public override void GenericEvent(GenericEventArgs e) {
            Console.WriteLine("Test Plugin ({0}) Event: {1}", this.PluginGuid, e.Name);

            if (e.GenericEventType == GenericEventType.TextCommandRegistered) {
                this.Commands.Add(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.TextCommandUnregistered) {
                this.Commands.Remove(e.Now.TextCommands.First());
            }
            else if (e.GenericEventType == GenericEventType.PluginsPluginEnabled) {

                this.ProxyExecute(new Command() {
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

                this.ProxyExecute(new Command() {
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

                this.ProxyExecute(new Command() {
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