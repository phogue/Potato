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
using System;
using System.Collections.Generic;
using Potato.Core.Shared;

namespace Potato.Examples.Plugins.Commands {
    /// <summary>
    /// Note we need to inherit from ExecutableBase which has all the methods
    /// required to dispatch commands
    /// </summary>
    public class TunneledCommands : CoreController {

        public TunneledCommands() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "ThisCommandIsInAChildObject",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.ThisCommandIsInAChildObject
                },
                new CommandDispatch() {
                    Name = "NoParameterBubbleCommand",
                    CommandAttributeType = CommandAttributeType.Handler,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "number",
                            Type = typeof(int)
                        }
                    },
                    Handler = this.NoParameterBubbleCommand
                }
            });
        }

        protected ICommandResult ThisCommandIsInAChildObject(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result.Message = "ThisCommandIsInAChildObjectResult";

            return command.Result;
        }

        protected ICommandResult NoParameterBubbleCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Name = "SingleConvertedParameterCommand";

            // Bubble the command back up to Program.cs
            return this.Bubble(command);
        }
    }
}
