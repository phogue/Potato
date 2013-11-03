using System;
using System.Collections.Generic;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableExecutedTester : ExecutableBasicTester {

        public ExecutableExecutedTester() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet, 
                        CommandAttributeType = CommandAttributeType.Executed,
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

        public int ExecutedTestValue { get; set; }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        public CommandResultArgs SetTestFlagPreview(Command command, Dictionary<String, CommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            CommandResultArgs result = command.Result;

            this.ExecutedTestValue = this.TestNumber * 2;

            return result;
        }
    }
}
