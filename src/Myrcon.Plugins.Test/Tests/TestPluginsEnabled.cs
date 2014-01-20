using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsEnabled : CoreController {

        public TestPluginsEnabled() : base() {
            this.CommandDispatchers.Add(
                new CommandDispatch() {
                    Name = "TestPluginsEnabledCommandResult",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "parameterMessage",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.TestPluginsEnabledCommandResult
                }
            );
        }

        protected ICommandResult TestPluginsEnabledCommandResult(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }


    }
}
