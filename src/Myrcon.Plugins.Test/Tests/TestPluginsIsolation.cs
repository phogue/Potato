using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Procon.Core.Shared;

namespace Myrcon.Plugins.Test.Tests {
    public class TestPluginsIsolation : CoreController {

        public TestPluginsIsolation() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandDispatch, CommandDispatchHandler>() {
                {
                    new CommandDispatch() {
                        Name = "TestPluginsIsolationCleanCurrentAppDomain",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "parameterMessage",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TestPluginsIsolationCleanCurrentAppDomain)
                }, {
                    new CommandDispatch() {
                        Name = "TestPluginsIsolationWriteToDirectory",
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "path",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TestPluginsIsolationWriteToDirectory)
                }
            });
        }

        protected ICommandResult TestPluginsIsolationCleanCurrentAppDomain(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String parameterMessage = parameters["parameterMessage"].First<String>();

            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }

        protected ICommandResult TestPluginsIsolationWriteToDirectory(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String path = parameters["path"].First<String>();

            try {
                using (StreamWriter file = new StreamWriter(Path.Combine(path, "IsolationWriteTest.txt"))) {
                    file.WriteLine("It's potentially bad if you can see this.");
                }

                command.Result.Success = true;
                command.Result.Status = CommandResultType.Success;
            }
            catch (SecurityException e) {
                command.Result.Message = e.Message;
                command.Result.Success = false;
                command.Result.Status = CommandResultType.Failed;
            }
            catch (Exception e) {
                var x = 0;
            }

            return command.Result;
        }
    }
}
