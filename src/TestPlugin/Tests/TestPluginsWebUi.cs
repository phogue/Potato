using System;
using System.Collections.Generic;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils.HTTP;

namespace TestPlugin.Tests {
    using Procon.Core;
    using Pages;

    public class TestPluginsWebUi : ExecutableBase {

        public TestPluginsWebUi() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "TestPlugin/index",
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
                    new CommandDispatchHandler(this.TestPluginWebUiIndex)
                }
            });
        }

        protected CommandResultArgs TestPluginWebUiIndex(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            int score = parameters["score"].First<int>();

            Player player = new Player() {
                Name = name,
                Score = score
            };
            
            IndexPageView index = new IndexPageView() {
                Player = player
            };

            command.Result = new CommandResultArgs() {
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
