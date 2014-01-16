using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsEnabled : CoreController {

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

        protected CommandResult TestPluginsEnabledCommandResult(Command command, Dictionary<String, CommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }


    }
}
