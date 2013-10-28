using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Utils;

namespace Procon.Core.Test.Utils {
    [TestClass]
    public class TestDefines {

        /// <summary>
        /// This whole class is just code coverage.
        /// </summary>
        [TestMethod]
        public void TestDefinesBaseDirectory() {
            Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, Defines.BaseDirectory);
        }
    }
}
