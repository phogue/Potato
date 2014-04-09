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
#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Serialization;
using Procon.Net.Shared.Utils.HTTP;

#endregion

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestPushEventsEndPoint {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that an event can be pushed onot the stream.
        /// </summary>
        [Test]
        public void TestPushEventsAppendSuccess() {
            var pushEndPoint = new PushEventsEndPoint();

            pushEndPoint.Append(new GenericEvent() {
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

            var genericEventArgs = new GenericEvent() {
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

            pushEndPoint.Append(new GenericEvent() {
                Message = "What up?"
            });

            pushEndPoint.PushCompleted += sender => requestWait.Set();

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
                Events = new List<IGenericEvent>() {
                    new GenericEvent() {
                        Message = "What up?"
                    }
                }
            });

            var deserialized = JsonSerialization.Minimal.Deserialize<PushEventsRequest>(new JsonTextReader(new StringReader(builder.ToString())));

            Assert.AreEqual("What up?", deserialized.Events.First().Message);
        }
    }
}