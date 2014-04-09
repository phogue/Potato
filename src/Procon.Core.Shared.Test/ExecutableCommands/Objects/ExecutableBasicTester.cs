#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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
                CommandResultType = CommandResultType.Success,
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
                CommandResultType = CommandResultType.Success,
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