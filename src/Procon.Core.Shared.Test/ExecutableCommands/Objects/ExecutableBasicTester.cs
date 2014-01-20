#region

using System;
using System.Collections.Generic;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutableBasicTester : CoreController {
        private int _testNumber;

        public ExecutableBasicTester() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    },
                    Handler = this.SetTestFlag
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesGet,
                    Handler = this.GetTestFlag
                },
                new CommandDispatch() {
                    Name = "CustomSet",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    },
                    Handler = this.CustomSetTestFlag
                }
            });
        }

        public int TestNumber {
            get { return _testNumber; }
            set {
                _testNumber = value;
                this.OnPropertyChanged(this, "TestNumber");
            }
        }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlag(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
        ///     Gets the value as an integer for the single test flag.
        /// </summary>
        public ICommandResult GetTestFlag(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return new CommandResult() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Get Number",
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
        ///     Sets the value of the test flag, but with a custom name.
        /// </summary>
        public ICommandResult CustomSetTestFlag(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return SetTestFlag(command, parameters);
        }
    }
}