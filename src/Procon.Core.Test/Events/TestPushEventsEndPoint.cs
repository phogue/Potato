#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Core.Shared.Events;
using Procon.Net.Shared.Utils.HTTP;

#endregion

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestPushEventsEndPoint {
        /// <summary>
        ///     Tests that an event can be pushed onot the stream.
        /// </summary>
        [Test]
        public void TestPushEventsAppendSuccess() {
            var pushEndPoint = new PushEventsEndPoint();

            pushEndPoint.Append(new GenericEventArgs() {
                Message = "What up?"
            });

            Assert.AreEqual(1, pushEndPoint.EventsStream.Count);
        }

        /// <summary>
        ///     Tests that an event that is disposed will also be removed from the
        ///     stream. Even if the stream has not been pushed yet.
        /// </summary>
        [Test]
        public void TestPushEventsDisposedEventIsRemoved() {
            var pushEndPoint = new PushEventsEndPoint();

            var genericEventArgs = new GenericEventArgs() {
                Message = "What up?"
            };

            pushEndPoint.Append(genericEventArgs);

            genericEventArgs.Dispose();

            Assert.AreEqual(0, pushEndPoint.EventsStream.Count);
        }

        // @todo push events request success.. or at least validate the outgoing data?

        /// <summary>
        ///     Tests that even when a request fails it will still remove the
        ///     events from the stream.
        /// </summary>
        [Test]
        public void TestPushEventsRequestFailCleanupOccurs() {
            var requestWait = new AutoResetEvent(false);
            var pushEndPoint = new PushEventsEndPoint();

            pushEndPoint.Append(new GenericEventArgs() {
                Message = "What up?"
            });

            pushEndPoint.PushCompleted += (sender, args) => requestWait.Set();

            pushEndPoint.Push();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(0, pushEndPoint.EventsStream.Count);
        }

        /// <summary>
        ///     Simply tests if the ApplicationJson mime type is passed in we will recieve json back from the function.
        /// </summary>
        [Test]
        public void TestPushEventsRequestJsonSerialization() {
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);

            PushEventsEndPoint.WriteSerializedEventsRequest(writer, Mime.ApplicationJson, new PushEventsRequest() {
                Events = new List<GenericEventArgs>() {
                    new GenericEventArgs() {
                        Message = "What up?"
                    }
                }
            });

            var deserialized = JsonConvert.DeserializeObject<PushEventsRequest>(builder.ToString());

            Assert.AreEqual("What up?", deserialized.Events.First().Message);
        }

        /// <summary>
        ///     Simply tests if the ApplicationXml mime type is passed in we will recieve xml back from the function.
        /// </summary>
        [Test]
        public void TestPushEventsRequestXmlSerialization() {
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);

            PushEventsEndPoint.WriteSerializedEventsRequest(writer, Mime.ApplicationXml, new PushEventsRequest() {
                Events = new List<GenericEventArgs>() {
                    new GenericEventArgs() {
                        Message = "What up?"
                    }
                }
            });

            // Can this in turn be parsed by xml?
            XElement element = XElement.Parse(builder.ToString());

            Assert.AreEqual("What up?", element.Descendants("Message").First().Value);
        }
    }
}