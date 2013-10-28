using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Utils;

namespace Procon.Core.Test.Utils {
    [TestClass]
    public class TestXElementValidator {

        /// <summary>
        /// Tests that valid xml will be passed and the value fetched from the reslting
        /// XElement.
        /// </summary>
        [TestMethod]
        public void TestXElementValidatorTryParseSuccess() {
            XElement element = null;

            Assert.IsTrue(XElementValidator.TryParse("<root><key>value</key></root>", out element));
            Assert.AreEqual("value", element.Element("key").Value);
        }

        /// <summary>
        /// Tests that the parser will fail and null the out element.
        /// </summary>
        [TestMethod]
        public void TestXElementValidatorTryParseFailed() {
            XElement element = null;

            Assert.IsFalse(XElementValidator.TryParse("<root><ke</key></root>", out element));
            Assert.IsNull(element);
        }
    }
}
