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
using Myrcon.Plugins.Test.Pages;
using Myrcon.Plugins.Test.Properties;
using Potato.Core.Shared;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Utils.HTTP;

namespace Myrcon.Plugins.Test.Tests {

    public class TestPluginsWebUi : CoreController {

        public TestPluginsWebUi() : base() {
            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "/",
                    Handler = TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/overview",
                    Handler = TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/settings",
                    Handler = TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/player",
                    Handler = TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/script.js",
                    Handler = TestPluginScript
                },
                new CommandDispatch() {
                    Name = "/style.css",
                    Handler = TestPluginStyle
                },
                new CommandDispatch() {
                    Name = "TestPluginSimpleMultiplyByTwoCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "number",
                            Type = typeof(int)
                        }
                    },
                    Handler = TestPluginSimpleMultiplyByTwoCommand
                },
                new CommandDispatch() {
                    Name = "/test/parameters",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "score",
                            Type = typeof(int)
                        }
                    },
                    Handler = TestPluginParameters
                }
            });
        }

        protected ICommandResult TestPluginIndex(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var index = new IndexPageView();

            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        index.TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult TestPluginScript(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        Resources.Script
                    }
                },
                ContentType = Mime.ApplicationJavascript,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult TestPluginStyle(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        Resources.Style
                    }
                },
                ContentType = Mime.TextCss,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult TestPluginSimpleMultiplyByTwoCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var number = parameters["number"].First<int>();

            command.Result.Message = string.Format("{0}", number * 2);
            command.Result.CommandResultType = CommandResultType.Success;

            return command.Result;
        }

        protected ICommandResult TestPluginParameters(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var score = parameters["score"].First<int>();

            var player = new PlayerModel() {
                Name = name,
                Score = score
            };

            var index = new TestParameterPageView() {
                Player = player
            };

            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<string>() {
                        index.TransformText()
                    }
                },
                ContentType = Mime.TextHtml,
                CommandResultType = CommandResultType.Success,
                Success = true,
                Message = name
            };

            return command.Result;
        }
    }
}
