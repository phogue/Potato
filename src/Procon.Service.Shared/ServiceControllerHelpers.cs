using System;
using System.Collections.Generic;
using System.IO;

namespace Procon.Service.Shared {
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
        public static void LogUnhandledException(String hint, Exception e) {
            Directory.CreateDirectory(Defines.ErrorsLogsDirectory);

            var lines = new List<String>() {
                String.Format("Hint: {0}", hint),
                String.Format("Exception: {0}", e)
            };

            File.WriteAllLines(Path.Combine(Defines.ErrorsLogsDirectory, DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss-fffffff")), lines);

            lines.ForEach(line => Console.WriteLine("Error: {0}", line));
        }
    }
}
