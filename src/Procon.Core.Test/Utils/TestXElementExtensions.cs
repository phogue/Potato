#region

using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Test.Utils.Objects;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Utils {
    [TestFixture]
    public class TestXElementExtensions {
        [Test]
        public void TestFromXElement() {
            XElement element = XElement.Parse(@"<XElementTester>
  <Word>This</Word>
  <Number>1</Number>
</XElementTester>");

            var elementTester = element.FromXElement<XElementTester>();

            Assert.AreEqual("This", elementTester.Word);
            Assert.AreEqual(1, elementTester.Number);
        }

        /// <summary>
        ///     Tests that a object can be serialized to a XElement object.
        /// </summary>
        [Test]
        public void TestToXElement() {
            XElement element = new XElementTester() {
                Word = "This",
                Number = 1
            }.ToXElement();

            Assert.AreEqual("This", element.Element("Word").Value);
            Assert.AreEqual("1", element.Element("Number").Value);
        }
    }
}