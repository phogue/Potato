using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace TestPlugin.Tests {
    using Procon.Core;

    public class TestPluginsIsolation : ExecutableBase {

        [CommandAttribute(Name = "TestPluginsIsolationCleanCurrentAppDomain")]
        protected CommandResultArgs TestPluginsIsolationCleanCurrentAppDomain(Command command, String parameterMessage) {
            command.Result.Message = parameterMessage;
            command.Result.Status = CommandResultType.Success;

            return command.Result;
        }

        [CommandAttribute(Name = "TestPluginsIsolationWriteToDirectory")]
        protected CommandResultArgs TestPluginsIsolationWriteToDirectory(Command command, String path) {

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
