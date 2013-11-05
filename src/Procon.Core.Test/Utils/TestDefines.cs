using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Utils;

namespace Procon.Core.Test.Utils {
    [TestFixture]
    public class TestDefines {

        /// <summary>
        /// This whole class is just code coverage.
        /// </summary>
        [Test]
        public void TestDefinesBaseDirectory() {
            Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, Defines.BaseDirectory);
        }
    }
}
