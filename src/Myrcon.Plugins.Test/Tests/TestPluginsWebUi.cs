using System;
using System.Collections.Generic;
using Myrcon.Plugins.Test.Pages;
using Myrcon.Plugins.Test.Properties;
using Procon.Core.Shared;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils.HTTP;

namespace Myrcon.Plugins.Test.Tests {

    public class TestPluginsWebUi : CoreController {

        public TestPluginsWebUi() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "/",
                    Handler = this.TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/overview",
                    Handler = this.TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/settings",
                    Handler = this.TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/widget/player",
                    Handler = this.TestPluginIndex
                },
                new CommandDispatch() {
                    Name = "/script.js",
                    Handler = this.TestPluginScript
                },
                new CommandDispatch() {
                    Name = "/style.css",
                    Handler = this.TestPluginStyle
                },
                new CommandDispatch() {
                    Name = "TestPluginSimpleMultiplyByTwoCommand",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "number",
                            Type = typeof(int)
                        }
                    },
                    Handler = this.TestPluginSimpleMultiplyByTwoCommand
                },
                new CommandDispatch() {
                    Name = "/settings",
                    Handler = this.TestPluginSettings
                },
                new CommandDispatch() {
                    Name = "/test/parameters",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "score",
                            Type = typeof(int)
                        }
                    },
                    Handler = this.TestPluginParameters
                }
            });
        }

        protected ICommandResult TestPluginIndex(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            IndexPageView index = new IndexPageView();

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

        protected ICommandResult TestPluginScript(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<String>() {
                        Resources.Script
                    }
                },
                ContentType = Mime.ApplicationJavascript,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult TestPluginStyle(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result = new CommandResult() {
                Now = new CommandData() {
                    Content = new List<String>() {
                        Resources.Style
                    }
                },
                ContentType = Mime.TextCss,
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            return command.Result;
        }

        protected ICommandResult TestPluginSimpleMultiplyByTwoCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            int number = parameters["number"].First<int>();

            command.Result.Message = String.Format("{0}", number * 2);
            command.Result.CommandResultType = CommandResultType.Success;

            return command.Result;
        }

        protected ICommandResult TestPluginSettings(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            SettingsPageView index = new SettingsPageView();

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

        protected ICommandResult TestPluginParameters(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            int score = parameters["score"].First<int>();

            PlayerModel player = new PlayerModel() {
                Name = name,
                Score = score
            };

            TestParameterPageView index = new TestParameterPageView() {
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
