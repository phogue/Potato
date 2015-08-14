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
using Potato.Core.Shared;
using Potato.Core.Shared.Plugins;

namespace Potato.Examples.Plugins.Configs {
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

        public Program() : base() {
            CommandDispatchers.Add(
                new CommandDispatch() {
                    Name = "ThisIsJustACommand",
                    CommandAttributeType = CommandAttributeType.Executed,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "param1",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "param2",
                            Type = typeof(string)
                        }
                    },
                    Handler = ThisIsJustACommand
                }
            );
        }

        /// <summary>
        /// A command to accept from the config.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected ICommandResult ThisIsJustACommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
// ReSharper disable UnusedVariable
            var param1 = parameters["param1"].First<string>();
            var param2 = parameters["param2"].First<string>();
// ReSharper restore UnusedVariable

            return command.Result;
        }

        public override void WriteConfig(IConfig config, string password = null) {
            base.WriteConfig(config, password);

            // Writing configs may seem a little bit convoluted in Potato 2, but you should
            // think of it simply as Command serialization, which allows you to save complex
            // parameters.

            config.Append(new Command() {
                Name = "ThisIsJustACommand",
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "Parameter1Value"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "Parameter2Value"
                            }
                        }
                    }
                }
            }.ToConfigCommand());
        }

    }
}
