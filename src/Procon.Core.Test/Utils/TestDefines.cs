using System;
using NUnit.Framework;
using Procon.Service.Shared;

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
