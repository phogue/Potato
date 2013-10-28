using System;

namespace TestPlugin.Tests {
    using Procon.Core;

    public class TestPluginsCommands : ExecutableBase {

        [CommandAttribute(Name = "TestPluginsCommandsZeroParameters")]
        protected CommandResultArgs TestPluginsCommandsZeroParameters(Command command) {
            command.Result.Message = command.Name;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }
    }
}
