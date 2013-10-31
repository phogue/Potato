using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Events;
using Procon.Core.Utils;
using Procon.Net.Protocols.Frostbite.Objects;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils;

namespace Procon.Core.Test.Serialization.Xml {
    [TestClass]
    public class TestXmlSerializationCommand {

        /// <summary>
        /// Tests the command result with no event data (blank Scope, Then & Now)
        /// will succeed with xml serialization.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationCommandResultNoData() {
            DateTime now = DateTime.Now;

            CommandResultArgs result = new CommandResultArgs() {
                Message = "Serialization Message",
                Now = null,
                Then = null,
                Scope = null,
                Status = CommandResultType.Success,
                Success = true,
                Stamp = now
            };

            XElement element = result.ToXElement();

            Assert.AreEqual("Serialization Message", element.Element("Message").Value);
            Assert.AreEqual("Success", element.Element("Status").Value);
            Assert.AreEqual(now.ToString("s", System.Globalization.CultureInfo.InvariantCulture), DateTime.Parse(element.Element("Stamp").Value).ToString("s", System.Globalization.CultureInfo.InvariantCulture));
        }


        /// <summary>
        /// Tests that a derived type will be serialized as the known base type.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationCommandResultDerivedType() {
            DateTime now = DateTime.Now;

            CommandResultArgs result = new CommandResultArgs() {
                Message = "Serialization Message",
                Now = null,
                Then = null,
                Scope = new CommandData() {
                    Chats = new List<Chat>() {
                        new Chat() {
                            Now = new NetworkActionData() {
                                Content = new List<String>() {
                                    "Hello"
                                }
                            }
                        }
                    }
                },
                Status = CommandResultType.Success,
                Success = true,
                Stamp = now
            };

            XElement element = result.ToXElement();

            Assert.AreEqual("Serialization Message", element.Element("Message").Value);
            Assert.AreEqual("Success", element.Element("Status").Value);

            Assert.AreEqual(now.ToString("s", System.Globalization.CultureInfo.InvariantCulture), DateTime.Parse(element.Element("Stamp").Value).ToString("s", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
