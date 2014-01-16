using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestCompleteResponsePacket {
        /// <summary>
        /// Tests that the completed json serialization can be deserialized again.
        /// </summary>
        [Test]
        public void TestSerializeJson() {
            var packet = CommandServerSerializer.CompleteResponsePacket(Mime.ApplicationJson, new CommandServerPacket(), new CommandResult() {
                Now = {
                    Content = new List<String>() {
                        "A"
                    }
                }
            });

            var array = (JContainer)JsonConvert.DeserializeObject(packet.Content);

            Assert.AreEqual("A", array["Now"]["Content"].First().Value<String>());
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
            Assert.AreEqual(Mime.ApplicationJson, packet.Headers[HttpRequestHeader.ContentType]);
        }

        /// <summary>
        /// Tests that the completed xml serialization can be deserialized again.
        /// </summary>
        [Test]
        public void TestSerializeXml() {
            var packet = CommandServerSerializer.CompleteResponsePacket(Mime.ApplicationXml, new CommandServerPacket(), new CommandResult() {
                Now = {
                    Content = new List<String>() {
                        "A"
                    }
                }
            });

            var array = XDocument.Parse(packet.Content);

            Assert.AreEqual("A", array.Descendants("Content").First().Value);
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
            Assert.AreEqual(Mime.ApplicationXml, packet.Headers[HttpRequestHeader.ContentType]);
        }

        /// <summary>
        /// Tests the content from the packet matches the input serialization contwent (straight assignment)
        /// </summary>
        [Test]
        public void TestSerializeTextHtml() {
            var packet = CommandServerSerializer.CompleteResponsePacket(Mime.TextHtml, new CommandServerPacket(), new CommandResult() {
                Now = {
                    Content = new List<String>() {
                        "A"
                    }
                }
            });

            Assert.AreEqual("A", packet.Content);
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
            Assert.AreEqual(Mime.TextHtml, packet.Headers[HttpRequestHeader.ContentType]);
        }
    }
}
