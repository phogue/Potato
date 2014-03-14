using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsSerialization : CoreController {

        public TestPluginsSerialization() : base() {
            this.CommandDispatchers.Add(
                new CommandDispatch() {
                    Name = "TestPluginsSerializationCommandResult",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "parameterMessage",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.TestPluginsSerializationCommandResult
                }
            );
        }

        protected ICommandResult TestPluginsSerializationCommandResult(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.CommandResultType = CommandResultType.Success;

            return command.Result;
        }


    }
}
