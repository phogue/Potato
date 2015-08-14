#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestCompleteResponsePacket {
        /// <summary>
        /// Tests that the completed json serialization can be deserialized again.
        /// </summary>
        [Test]
        public void TestSerializeJson() {
            var packet = CommandServerSerializer.CompleteResponsePacket(Mime.ApplicationJson, new CommandServerPacket(), new CommandResult() {
                Now = {
                    Content = new List<string>() {
                        "A"
                    }
                }
            });

            var array = (JContainer)JsonConvert.DeserializeObject(packet.Content);

            Assert.AreEqual("A", array["Now"]["Content"].First().Value<string>());
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
            Assert.AreEqual(Mime.ApplicationJson, packet.Headers[HttpResponseHeader.ContentType]);
        }

        /// <summary>
        /// Tests the content from the packet matches the input serialization contwent (straight assignment)
        /// </summary>
        [Test]
        public void TestSerializeTextHtml() {
            var packet = CommandServerSerializer.CompleteResponsePacket(Mime.TextHtml, new CommandServerPacket(), new CommandResult() {
                Now = {
                    Content = new List<string>() {
                        "A"
                    }
                }
            });

            Assert.AreEqual("A", packet.Content);
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
            Assert.AreEqual(Mime.TextHtml, packet.Headers[HttpResponseHeader.ContentType]);
        }
    }
}
