using System;
using System.Collections.Generic;

namespace Procon.Core.Shared.Test.AsynchronousExecutableCommands.Mocks {
    /// <summary>
    /// Captures and edits a command message with the supplied text
    /// </summary>
    public class MockSynchronousCoreController : CoreController {

        public MockSynchronousCoreController() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "AppendMessage",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof (String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.AppendMessage)
                }
            });
        }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult AppendMessage(Command command, Dictionary<String, CommandParameter> parameters) {
            String value = parameters["value"].First<String>();

            return new CommandResult() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "SetMessage: " + value
            };
        }
    }
}
