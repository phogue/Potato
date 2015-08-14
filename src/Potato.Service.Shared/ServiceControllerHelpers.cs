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

namespace Potato.Service.Shared {
    /// <summary>
    /// A series of static methods to use within the ServiceController, but it 
    /// neatens the code up a little having some of the functionality here.
    /// </summary>
    public static class ServiceControllerHelpers {
        /// <summary>
        /// Self enclosed exception log, opens a file, writes the exception and flushes/closes the file.
        /// </summary>
        /// <param name="hint">A hint for where the exception occured</param>
        /// <param name="e">The exception to log</param>
        public static void LogUnhandledException(string hint, Exception e) {
            Defines.ErrorsLogsDirectory.Create();

            var lines = new List<string>() {
                string.Format("Hint: {0}", hint),
                string.Format("Exception: {0}", e)
            };

            File.WriteAllLines(Path.Combine(Defines.ErrorsLogsDirectory.FullName, DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss-fffffff")), lines);

            lines.ForEach(line => Console.WriteLine("Error: {0}", line));
        }
    }
}
