using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPlugin.Tests {
    using Procon.Core;

    public class TestPluginsSerialization : ExecutableBase {


        [CommandAttribute(Name = "TestPluginsSerializationCommandResult")]
        protected CommandResultArgs TestPluginsSerializationCommandResult(Command command, String parameterMessage) {
            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }


    }
}
