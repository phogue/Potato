using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestServiceControllerHelpers {
    [TestFixture]
    public class TestLogUnhandledException {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            if (Directory.Exists(Defines.ErrorsLogsDirectory) == true) Directory.Delete(Defines.ErrorsLogsDirectory, true);
        }

        /// <summary>
        /// Tests that an error will be logged to the errors log directory. We just test that a file exists
        /// and isn't 0 bytes.
        /// </summary>
        [Test]
        public void TestLogErrorSuccess() {
            ServiceControllerHelpers.LogUnhandledException("None", new Exception("Nothing"));
            
            Assert.IsNotEmpty(Directory.GetFiles(Defines.ErrorsLogsDirectory));
            Assert.Greater(new FileInfo(Directory.GetFiles(Defines.ErrorsLogsDirectory).First()).Length, 0);
        }
    }
}
