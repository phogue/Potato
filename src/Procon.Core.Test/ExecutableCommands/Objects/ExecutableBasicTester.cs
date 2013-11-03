using System;
using System.Collections.Generic;
using Procon.Core.Variables;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableBasicTester : ExecutableBase {

        public ExecutableBasicTester() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(int)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetTestFlag)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesGet
                    },
                    new CommandDispatchHandler(this.GetTestFlag)
                }, {
                    new CommandAttribute() {
                        Name = "CustomSet",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(int)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CustomSetTestFlag)
                }
            });
        }

        private int _testNumber;
        public int TestNumber {
            get { return _testNumber; }
            set {
                _testNumber = value;
                this.OnPropertyChanged(this, "TestNumber");
            }
        }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        public CommandResultArgs SetTestFlag(Command command, Dictionary<String, CommandParameter> parameters) {
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
        /// Gets the value as an integer for the single test flag.
        /// </summary>
        public CommandResultArgs GetTestFlag(Command command, Dictionary<String, CommandParameter> parameters) {
            return new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Get Number",
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
        /// Sets the value of the test flag, but with a custom name.
        /// </summary>
        public CommandResultArgs CustomSetTestFlag(Command command, Dictionary<String, CommandParameter> parameters) {
            return this.SetTestFlag(command, parameters);
        }
    }
}
