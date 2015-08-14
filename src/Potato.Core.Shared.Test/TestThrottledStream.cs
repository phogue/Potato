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
using System.Threading;
using NUnit.Framework;

namespace Potato.Core.Shared.Test {
    [TestFixture]
    public class TestThrottledStream {
        /// <summary>
        /// Tests that running start will set the stream to running and create the interval timer
        /// </summary>
        [Test]
        public void TestStart() {
            var stream = new ThrottledStream<int>();

            stream.Start();

            Assert.IsTrue(stream.Running);
            Assert.IsNotNull(stream.IntervalTick);
        }

        /// <summary>
        /// Tests that calling stop will ultimately set the running property to false and null the interval tick.
        /// </summary>
        [Test]
        public void TestStop() {
            var stream = new ThrottledStream<int>();

            stream.Stop();

            Assert.IsFalse(stream.Running);
            Assert.IsNull(stream.IntervalTick);
            Assert.IsNull(stream.FlushTo);
        }

        /// <summary>
        /// Tests that calling stop will ultimately set the running property to false and null the interval tick.
        /// </summary>
        [Test]
        public void TestStopWhileRunning() {
            var stream = new ThrottledStream<int>() {
                FlushTo = items => {
                    
                }
            };

            stream.Start();

            stream.Stop();

            Assert.IsFalse(stream.Running);
            Assert.IsNull(stream.IntervalTick);
            Assert.IsNull(stream.FlushTo);
        }

        /// <summary>
        /// Tests that a short interval will result in the flush to method being called when 1 item is on the list.
        /// </summary>
        [Test]
        public void TestFlushToIsCalled() {
            var reset = new AutoResetEvent(false);
            var isFlushToCalled = false;

            var stream = new ThrottledStream<int>() {
                Interval = new TimeSpan(0, 0, 0, 0, 10),
                FlushTo = items => {
                    isFlushToCalled = true;
                    reset.Set();
                }
            };

            stream.Start();

            stream.Call(1);

            Assert.IsTrue(reset.WaitOne(500));

            Assert.IsTrue(isFlushToCalled);
        }

        /// <summary>
        /// Tests that items will come out of the flush to in the same order they went in as
        /// </summary>
        [Test]
        public void TestFlushToCorrectOrder() {
            var reset = new AutoResetEvent(false);
            var flushedItems = new List<int>();

            var stream = new ThrottledStream<int>() {
                Interval = new TimeSpan(0, 0, 0, 0, 100),
                FlushTo = items => {
                    flushedItems = items;
                    reset.Set();
                }
            };

            stream.Start();

            stream.Call(1);
            stream.Call(2);
            stream.Call(3);

            Assert.IsTrue(reset.WaitOne(500));

            Assert.AreEqual(new List<int>() { 1, 2, 3 }, flushedItems);
        }

        /// <summary>
        /// Tests that an item will not be enqueued if the stream is not marked as running.
        /// </summary>
        [Test]
        public void TestIgnoredWhenNotRunning() {
            var stream = new ThrottledStream<int>();

            stream.Call(1);

            Assert.IsEmpty(stream.Items);
        }

        /// <summary>
        /// Tests that an item will be pushed if the stream is marked as running
        /// </summary>
        [Test]
        public void TestPushedWhenRunning() {
            var stream = new ThrottledStream<int>() {
                Running = true
            };

            stream.Call(1);

            Assert.IsNotEmpty(stream.Items);
        }
    }
}
