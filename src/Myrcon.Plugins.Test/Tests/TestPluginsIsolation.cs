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
using System.IO;
using System.Security;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsIsolation : CoreController {

        public TestPluginsIsolation() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "TestPluginsIsolationCleanCurrentAppDomain",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "parameterMessage",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.TestPluginsIsolationCleanCurrentAppDomain
                },
                new CommandDispatch() {
                    Name = "TestPluginsIsolationWriteToDirectory",
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "path",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.TestPluginsIsolationWriteToDirectory
                }
            });
        }

        protected ICommandResult TestPluginsIsolationCleanCurrentAppDomain(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.CommandResultType = CommandResultType.Success;

            return command.Result;
        }

        protected ICommandResult TestPluginsIsolationWriteToDirectory(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String path = parameters["path"].First<String>();

            try {
                using (StreamWriter file = new StreamWriter(Path.Combine(path, "IsolationWriteTest.txt"))) {
                    file.WriteLine("It's potentially bad if you can see this.");
                }

                command.Result.Success = true;
                command.Result.CommandResultType = CommandResultType.Success;
            }
            catch (SecurityException e) {
                command.Result.Message = e.Message;
                command.Result.Success = false;
                command.Result.CommandResultType = CommandResultType.Failed;
            }
            catch (Exception e) {
                var x = 0;
            }

            return command.Result;
        }
    }
}
