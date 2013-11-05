using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Net.Utils.HTTP;

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestPushEventsEndPoint {

        /// <summary>
        /// Tests that an event can be pushed onot the stream.
        /// </summary>
        [Test]
        public void TestPushEventsAppendSuccess() {
            PushEventsEndPoint pushEndPoint = new PushEventsEndPoint();

            pushEndPoint.Append(new GenericEventArgs() {
                Message = "What up?"
            });

            Assert.AreEqual(1, pushEndPoint.EventsStream.Count);
        }

        /// <summary>
        /// Tests that an event that is disposed will also be removed from the
        /// stream. Even if the stream has not been pushed yet.
        /// </summary>
        [Test]
        public void TestPushEventsDisposedEventIsRemoved() {
            PushEventsEndPoint pushEndPoint = new PushEventsEndPoint();

            GenericEventArgs genericEventArgs = new GenericEventArgs() {
                Message = "What up?"
            };

            pushEndPoint.Append(genericEventArgs);

            genericEventArgs.Dispose();

            Assert.AreEqual(0, pushEndPoint.EventsStream.Count);
        }

        // @todo push events request success.. or at least validate the outgoing data?

        /// <summary>
        /// Tests that even when a request fails it will still remove the
        /// events from the stream.
        /// </summary>
        [Test]
        public void TestPushEventsRequestFailCleanupOccurs() {
            AutoResetEvent requestWait = new AutoResetEvent(false);
            PushEventsEndPoint pushEndPoint = new PushEventsEndPoint();

            pushEndPoint.Append(new GenericEventArgs() {
                Message = "What up?"
            });

            pushEndPoint.PushCompleted += (sender, args) => requestWait.Set();

            pushEndPoint.Push();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(0, pushEndPoint.EventsStream.Count);
        }

        /// <summary>
        /// Simply tests if the ApplicationXml mime type is passed in we will recieve xml back from the function.
        /// </summary>
        [Test]
        public void TestPushEventsRequestXmlSerialization() {
            String serialized = PushEventsEndPoint.SerializeEventsRequest(Mime.ApplicationXml, new PushEventsRequest() {
                Events = new List<GenericEventArgs>() {
                    new GenericEventArgs() {
                        Message = "What up?"
                    }
                }
            });

            // Can this in turn be parsed by xml?
            XElement element = XElement.Parse(serialized);

            Assert.AreEqual("What up?", element.Descendants("Message").First().Value);
        }

        /// <summary>
        /// Simply tests if the ApplicationJson mime type is passed in we will recieve json back from the function.
        /// </summary>
        [Test]
        public void TestPushEventsRequestJsonSerialization() {
            String serialized = PushEventsEndPoint.SerializeEventsRequest(Mime.ApplicationJson, new PushEventsRequest() {
                Events = new List<GenericEventArgs>() {
                    new GenericEventArgs() {
                        Message = "What up?"
                    }
                }
            });

            PushEventsRequest deserialized = JsonConvert.DeserializeObject<PushEventsRequest>(serialized);

            Assert.AreEqual("What up?", deserialized.Events.First().Message);
        }
    }
}
