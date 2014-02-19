using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Procon.Core.Shared.Test {
    [TestFixture]
    public class TestThrottledStream {
        /// <summary>
        /// Tests that running start will set the stream to running and create the interval timer
        /// </summary>
        [Test]
        public void TestStart() {
            ThrottledStream<int> stream = new ThrottledStream<int>();

            stream.Start();

            Assert.IsTrue(stream.Running);
            Assert.IsNotNull(stream.IntervalTick);
        }

        /// <summary>
        /// Tests that calling stop will ultimately set the running property to false and null the interval tick.
        /// </summary>
        [Test]
        public void TestStop() {
            ThrottledStream<int> stream = new ThrottledStream<int>();

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
            ThrottledStream<int> stream = new ThrottledStream<int>() {
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
            AutoResetEvent reset = new AutoResetEvent(false);
            bool isFlushToCalled = false;

            ThrottledStream<int> stream = new ThrottledStream<int>() {
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
            AutoResetEvent reset = new AutoResetEvent(false);
            List<int> flushedItems = new List<int>();

            ThrottledStream<int> stream = new ThrottledStream<int>() {
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
            ThrottledStream<int> stream = new ThrottledStream<int>();

            stream.Call(1);

            Assert.IsEmpty(stream.Items);
        }

        /// <summary>
        /// Tests that an item will be pushed if the stream is marked as running
        /// </summary>
        [Test]
        public void TestPushedWhenRunning() {
            ThrottledStream<int> stream = new ThrottledStream<int>() {
                Running = true
            };

            stream.Call(1);

            Assert.IsNotEmpty(stream.Items);
        }
    }
}
