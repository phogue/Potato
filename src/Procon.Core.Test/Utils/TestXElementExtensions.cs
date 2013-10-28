using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.Utils.Objects;
using Procon.Core.Utils;

namespace Procon.Core.Test.Utils {

    [TestClass]
    public class TestXElementExtensions {

        /// <summary>
        /// Tests that a object can be serialized to a XElement object.
        /// </summary>
        [TestMethod]
        public void TestToXElement() {

            XElement element = new XElementTester(){
                Word = "This",
                Number = 1
            }.ToXElement();

            Assert.AreEqual("This", element.Element("Word").Value);
            Assert.AreEqual("1", element.Element("Number").Value);
        }

        [TestMethod]
        public void TestFromXElement() {
            XElement element = XElement.Parse(@"<XElementTester>
  <Word>This</Word>
  <Number>1</Number>
</XElementTester>");

            XElementTester elementTester = element.FromXElement<XElementTester>();

            Assert.AreEqual("This", elementTester.Word);
            Assert.AreEqual(1, elementTester.Number);
        }
    }
}
