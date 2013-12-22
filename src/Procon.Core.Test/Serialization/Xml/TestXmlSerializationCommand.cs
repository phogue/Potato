#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Net.Actions;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Serialization.Xml {
    [TestFixture]
    public class TestXmlSerializationCommand {
        /// <summary>
        ///     Tests that a derived type will be serialized as the known base type.
        /// </summary>
        [Test]
        public void TestXmlSerializationCommandResultDerivedType() {
            DateTime now = DateTime.Now;

            var result = new CommandResultArgs() {
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

            Assert.AreEqual(now.ToString("s", CultureInfo.InvariantCulture), DateTime.Parse(element.Element("Stamp").Value).ToString("s", CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Tests the command result with no event data (blank Scope, Then & Now)
        ///     will succeed with xml serialization.
        /// </summary>
        [Test]
        public void TestXmlSerializationCommandResultNoData() {
            DateTime now = DateTime.Now;

            var result = new CommandResultArgs() {
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
            Assert.AreEqual(now.ToString("s", CultureInfo.InvariantCulture), DateTime.Parse(element.Element("Stamp").Value).ToString("s", CultureInfo.InvariantCulture));
        }
    }
}