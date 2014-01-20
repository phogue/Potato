using System;
using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Plugins;
using Procon.Examples.Plugins.UserInterface.Pages;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Examples.Plugins.UserInterface {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "/",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.PageIndex
                },
                new CommandDispatch() {
                    Name = "/settings",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.PageSettings
                }
            });

        }

        protected ICommandResult PageIndex(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return new CommandResult() {
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

        protected ICommandResult PageSettings(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        new SettingsPageView() {
                            MyStringyVariable = "Output of the variable!",
                            MyListOfPlayers = new List<PlayerModel>() {
                                new PlayerModel() {
                                    Name = "Player1",
                                    Score = 100
                                },
                                new PlayerModel() {
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
