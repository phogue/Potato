using System;
using System.Collections.Generic;

namespace TestPlugin.Tests {
    using Procon.Core;

    public class TestPluginsEnabled : ExecutableBase {

        public TestPluginsEnabled() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "TestPluginsEnabledCommandResult",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "parameterMessage",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TestPluginsEnabledCommandResult)
                }
            });
        }

        protected CommandResultArgs TestPluginsEnabledCommandResult(Command command, Dictionary<String, CommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }


    }
}
