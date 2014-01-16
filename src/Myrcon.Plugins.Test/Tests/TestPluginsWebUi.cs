using System;
using System.Collections.Generic;
using Myrcon.Plugins.Test.Pages;
using Procon.Core.Shared;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils.HTTP;

namespace Myrcon.Plugins.Test.Tests {

    public class TestPluginsWebUi : CoreController {

        public TestPluginsWebUi() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "/"
                    },
                    new CommandDispatchHandler(this.TestPluginIndex)
                }
            });

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "/settings"
                    },
                    new CommandDispatchHandler(this.TestPluginSettings)
                }
            }); 
            
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.TestPluginParameters)
                }
            });
        }

        protected CommandResult TestPluginIndex(Command command, Dictionary<String, CommandParameter> parameters) {
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

        protected CommandResult TestPluginSettings(Command command, Dictionary<String, CommandParameter> parameters) {
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

        protected CommandResult TestPluginParameters(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            int score = parameters["score"].First<int>();

            Player player = new Player() {
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
