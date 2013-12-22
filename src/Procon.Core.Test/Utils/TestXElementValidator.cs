#region

using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Utils;

#endregion

namespace Procon.Core.Test.Utils {
    [TestFixture]
    public class TestXElementValidator {
        /// <summary>
        ///     Tests that the parser will fail and null the out element.
        /// </summary>
        [Test]
        public void TestXElementValidatorTryParseFailed() {
            XElement element = null;

            Assert.IsFalse(XElementValidator.TryParse("<root><ke</key></root>", out element));
            Assert.IsNull(element);
        }

        /// <summary>
        ///     Tests that valid xml will be passed and the value fetched from the reslting
        ///     XElement.
        /// </summary>
        [Test]
        public void TestXElementValidatorTryParseSuccess() {
            XElement element = null;

            Assert.IsTrue(XElementValidator.TryParse("<root><key>value</key></root>", out element));
            Assert.AreEqual("value", element.Element("key").Value);
        }
    }
}