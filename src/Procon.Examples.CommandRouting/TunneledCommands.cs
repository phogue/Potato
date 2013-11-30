using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procon.Core;

namespace Procon.Examples.CommandRouting {
    /// <summary>
    /// Note we need to inherit from ExecutableBase which has all the methods
    /// required to dispatch commands
    /// </summary>
    public class TunneledCommands : ExecutableBase {

        public TunneledCommands() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "ThisCommandIsInAChildObject",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.ThisCommandIsInAChildObject)
                }
            });
        }

        protected CommandResultArgs ThisCommandIsInAChildObject(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result.Message = "ThisCommandIsInAChildObjectResult";

            return command.Result;
        }
    }
}
