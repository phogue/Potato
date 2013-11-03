using System;
using System.Collections.Generic;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableEnumTester : ExecutableBase {

        public ExecutableEnumTester() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet, 
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(ExecutableFlagsEnum)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetTestFlagsEnum)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet, 
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(ExecutableEnum)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetTestEnum)
                }
            });
        }

        public ExecutableFlagsEnum TestExecutableFlagsEnum { get; set; }

        public ExecutableEnum TestExecutableEnum { get; set; }

        /// <summary>
        /// Tests that a flags enumerator will be passed through
        /// </summary>
        public CommandResultArgs SetTestFlagsEnum(Command command, Dictionary<String, CommandParameter> parameters) {
            ExecutableFlagsEnum value = parameters["value"].First<ExecutableFlagsEnum>();

            CommandResultArgs result = command.Result;

            this.TestExecutableFlagsEnum = value;

            return result;
        }

        /// <summary>
        /// Tests that a enumerator will be passed through
        /// </summary>
        public CommandResultArgs SetTestEnum(Command command, Dictionary<String, CommandParameter> parameters) {
            ExecutableEnum value = parameters["value"].First<ExecutableEnum>();

            CommandResultArgs result = command.Result;

            this.TestExecutableEnum = value;

            return result;
        }
    }
}
