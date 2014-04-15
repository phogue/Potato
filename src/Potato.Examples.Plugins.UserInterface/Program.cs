#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using Potato.Core.Shared;
using Potato.Core.Shared.Plugins;
using Potato.Examples.Plugins.UserInterface.Pages;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Examples.Plugins.UserInterface {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

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
                CommandResultType = CommandResultType.Success,
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
                CommandResultType = CommandResultType.Success,
                Success = true
            };
        }
    }
}
