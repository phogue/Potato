using System;
using System.Collections.Generic;
using Myrcon.Plugins.Test.Pages;
using Procon.Core.Shared;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils.HTTP;

namespace Myrcon.Plugins.Test.Tests {

    public class TestPluginsWebUi : CoreController {

        public TestPluginsWebUi() : base() {
            this.CommandDispatchers.AddRange(new List<CommandDispatch>() {
                new CommandDispatch() {
                    Name = "/",
                    Handler = this.TestPluginIndex
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
                Status = CommandResultType.Success,
                Success = true
            };

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
                Status = CommandResultType.Success,
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
                Status = CommandResultType.Success,
                Success = true,
                Message = name
            };

            return command.Result;
        }
    }
}
