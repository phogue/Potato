using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestSearchRelativeSearchPath {
        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileExistsInBaseDirectory() {
            List<String> paths = Defines.SearchRelativeSearchPath("Procon.Service.Shared.Test.dll");

            Assert.IsNotEmpty(paths);
        }

        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileDoesNotExist() {
            List<String> paths = Defines.SearchRelativeSearchPath("lulz.dll");

            Assert.IsEmpty(paths);
        }
    }
}
