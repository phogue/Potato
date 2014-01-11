using System.Net;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestResponseContentType {
        /// <summary>
        /// Tests that the result content type is returned if supplied.
        /// </summary>
        [Test]
        public void TestUseCommandResultContentType() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = new CommandResultArgs() {
                    ContentType = Mime.ApplicationJson
                }
            });

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests the request content type is used if the request result content type isn't specified.
        /// </summary>
        [Test]
        public void TestNulledCommandResultContentType() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = new CommandResultArgs() {
                    ContentType = null
                },
                RemoteRequest = new CommandServerPacket() {
                    Headers = new WebHeaderCollection() {
                        { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                    }
                }
            });

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests the request content type is used if the request result is null.
        /// </summary>
        [Test]
        public void TestNulledCommandResult() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = null,
                RemoteRequest = new CommandServerPacket() {
                    Headers = new WebHeaderCollection() {
                        { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                    }
                }
            });

            Assert.AreEqual(Mime.ApplicationJson, contentType);
        }

        /// <summary>
        /// Tests that supplying a nulled result and not supplying a header will result in ApplicationXml
        /// </summary>
        [Test]
        public void TestRequestAndResponseALlNullApplicationXml() {
            var contentType = CommandServerSerializer.ResponseContentType(new Command() {
                Result = null,
                RemoteRequest = new CommandServerPacket() {
                    Headers = new WebHeaderCollection() {
                        
                    }
                }
            });

            Assert.AreEqual(Mime.ApplicationXml, contentType);
        }
    }
}
