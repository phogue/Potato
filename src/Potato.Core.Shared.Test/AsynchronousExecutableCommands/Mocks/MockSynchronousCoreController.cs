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
using System;
using System.Collections.Generic;

namespace Potato.Core.Shared.Test.AsynchronousExecutableCommands.Mocks {
    /// <summary>
    /// Captures and edits a command message with the supplied text
    /// </summary>
    public class MockSynchronousCoreController : CoreController {

        public MockSynchronousCoreController() : base() {
            CommandDispatchers.Add(
                new CommandDispatch() {
                    Name = "AppendMessage",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof (string)
                        }
                    },
                    Handler = AppendMessage
                }
            );
        }

        /// <summary>
        ///     Sets the value of the test flag.
        /// </summary>
        public ICommandResult AppendMessage(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var value = parameters["value"].First<string>();

            return new CommandResult() {
                Success = true,
                CommandResultType = CommandResultType.Success,
                Message = "SetMessage: " + value
            };
        }
    }
}
