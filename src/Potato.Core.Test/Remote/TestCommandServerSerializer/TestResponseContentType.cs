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
using System.Net;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Core.Shared.Remote;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestResponseContentType {
        /// <summary>
        /// Tests that the result content type is returned if supplied.
        /// </summary>
        [Test]
        public void TestUseCommandResultContentType() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = new CommandResult() {
                    ContentType = Mime.ApplicationJson
                }
            }, new CommandServerPacket());

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests the request content type is used if the request result content type isn't specified.
        /// </summary>
        [Test]
        public void TestNulledCommandResultContentType() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = new CommandResult() {
                    ContentType = null
                },
                Request = new HttpCommandRequest() {
                    Tags = new Dictionary<String, String>() {
                        { HttpRequestHeader.ContentType.ToString(), Mime.ApplicationJson }
                    }
                }
            }, new CommandServerPacket());

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests the request content type is used if the request result is null.
        /// </summary>
        [Test]
        public void TestNulledCommandResult() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = null,
                Request = new HttpCommandRequest() {
                    Tags = new Dictionary<String, String>() {
                        { HttpRequestHeader.ContentType.ToString(), Mime.ApplicationJson }
                    }
                }
            }, new CommandServerPacket());

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests that supplying a nulled result and not supplying a header will result in ApplicationJson
        /// </summary>
        [Test]
        public void TestRequestAndResponseALlNullApplicationXml() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = null,
                Request = new HttpCommandRequest() {
                    Tags = new Dictionary<String, String>() {

                    }
                }
            }, new CommandServerPacket());

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }
    }
}
