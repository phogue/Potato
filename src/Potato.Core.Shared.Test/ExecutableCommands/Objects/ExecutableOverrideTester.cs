#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Core.Shared.Models;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands.Objects {
    public class ExecutableOverrideTester : CoreController {
        public ExecutableOverrideTester() : base() {
            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (int)
                        }
                    },
                    Handler = SetTestFlagInteger
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
                    Handler = SetTestFlagFloat
                }
            });
        }

        public int TestNumber { get; set; }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagInteger(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var value = parameters["value"].First<int>();

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
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult SetTestFlagFloat(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var value = parameters["value"].First<float>();

            TestNumber = (int) value;

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
    }
}