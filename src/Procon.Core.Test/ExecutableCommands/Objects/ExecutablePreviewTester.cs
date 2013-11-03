using System;
using System.Collections.Generic;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutablePreviewTester : ExecutableBasicTester {

        public ExecutablePreviewTester() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet, 
                        CommandAttributeType = CommandAttributeType.Preview,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(int)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetTestFlagPreview)
                }
            });
        }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        public CommandResultArgs SetTestFlagPreview(Command command, Dictionary<String, CommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            CommandResultArgs result = command.Result;

            if (value == 10) {
                result.Status = CommandResultType.None;
            }

            return result;
        }
    }
}
