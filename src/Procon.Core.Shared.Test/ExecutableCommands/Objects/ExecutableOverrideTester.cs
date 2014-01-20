#region

using System;
using System.Collections.Generic;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutableOverrideTester : CoreController {
        public ExecutableOverrideTester() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    },
                    Handler = this.SetTestFlagInteger
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (float)
                        }
                    },
                    Handler = this.SetTestFlagFloat
                }
            });
        }

        public int TestNumber { get; set; }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagInteger(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            int value = parameters["value"].First<int>();

            TestNumber = value;

            return new CommandResult() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Set Number",
                Now = new CommandData() {
                    Variables = new List<VariableModel>() {
                        new VariableModel() {
                            Name = "TestNumber",
                            Value = TestNumber
                        }
                    }
                }
            };
        }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagFloat(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            float value = parameters["value"].First<float>();

            TestNumber = (int) value;

            return new CommandResult() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Set Number",
                Now = new CommandData() {
                    Variables = new List<VariableModel>() {
                        new VariableModel() {
                            Name = "TestNumber",
                            Value = TestNumber
                        }
                    }
                }
            };
        }
    }
}