using System;
using System.Collections.Generic;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsCommands : CoreController {

        public TestPluginsCommands() : base() {
            this.CommandDispatchers.Add(
                new CommandDispatch() {
                    Name = "TestPluginsCommandsZeroParameters",
                    Handler = this.TestPluginsCommandsZeroParameters
                }
            );
        }

        protected ICommandResult TestPluginsCommandsZeroParameters(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result.Message = command.Name;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }
    }
}
