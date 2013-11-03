using System;
using System.Collections.Generic;
using Procon.Core.Variables;
using Procon.Core.Events;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableOverrideTester : ExecutableBase {

        public ExecutableOverrideTester() : base() {
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
                    new CommandDispatchHandler(this.SetTestFlagInteger)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet, 
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(float)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetTestFlagFloat)
                }
            });
        }

        public int TestNumber { get; set; }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        public CommandResultArgs SetTestFlagInteger(Command command, Dictionary<String, CommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            this.TestNumber = value;

            return new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Set Number",
                Now = new CommandData() {
                    Variables = new List<Variable>() {
                        new Variable() {
                            Name = "TestNumber",
                            Value = this.TestNumber
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        public CommandResultArgs SetTestFlagFloat(Command command, Dictionary<String, CommandParameter> parameters) {
            float value = parameters["value"].First<float>();

            this.TestNumber = (int)value;

            return new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Set Number",
                Now = new CommandData() {
                    Variables = new List<Variable>() {
                        new Variable() {
                            Name = "TestNumber",
                            Value = this.TestNumber
                        }
                    }
                }
            };
        }
    }
}
