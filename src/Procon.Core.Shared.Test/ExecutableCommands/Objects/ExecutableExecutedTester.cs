#region

using System;
using System.Collections.Generic;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutableExecutedTester : ExecutableBasicTester {
        public ExecutableExecutedTester() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {new CommandAttribute() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    }
                },
                    new CommandDispatchHandler(SetTestFlagPreview)}
            });
        }

        public int ExecutedTestValue { get; set; }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagPreview(ICommand command, Dictionary<String, CommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            ICommandResult result = command.Result;

            ExecutedTestValue = TestNumber * 2;

            return result;
        }
    }
}