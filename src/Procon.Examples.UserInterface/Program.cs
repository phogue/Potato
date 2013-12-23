using System;
using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Plugins;
using Procon.Examples.UserInterface.Pages;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Examples.UserInterface {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "/",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.PageIndex)
                },
                {
                    new CommandAttribute() {
                        Name = "/settings",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.PageSettings)
                }
            });

        }

        protected CommandResultArgs PageIndex(Command command, Dictionary<String, CommandParameter> parameters) {
            return new CommandResultArgs() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        new IndexPageView().TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                Status = CommandResultType.Success,
                Success = true
            };
        }

        protected CommandResultArgs PageSettings(Command command, Dictionary<String, CommandParameter> parameters) {
            return new CommandResultArgs() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        new SettingsPageView() {
                            MyStringyVariable = "Output of the variable!",
                            MyListOfPlayers = new List<Player>() {
                                new Player() {
                                    Name = "Player1",
                                    Score = 100
                                },
                                new Player() {
                                    Name = "Player2",
                                    Score = 250
                                }
                            }
                        }.TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                Status = CommandResultType.Success,
                Success = true
            };
        }
    }
}
