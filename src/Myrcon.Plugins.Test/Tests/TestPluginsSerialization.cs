using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsSerialization : CoreController {

        public TestPluginsSerialization() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "TestPluginsSerializationCommandResult",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "parameterMessage",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TestPluginsSerializationCommandResult)
                }
            });
        }

        protected ICommandResult TestPluginsSerializationCommandResult(Command command, Dictionary<String, CommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }


    }
}
