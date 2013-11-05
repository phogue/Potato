using System;
using System.Threading;
using NUnit.Framework;

namespace Procon.Core.Test.Scheduler {
    using Procon.Core.Scheduler;

    [TestFixture]
    public class TestScheduler {

        /// <summary>
        /// Test that any date time we pass will fire an event if no condition is added.
        /// </summary>
        [Test]
        public void TestTaskEvent() {
            int eventCount = 0;
            Random random = new Random();

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            tasks.Add(
                new Task()
            ).Tick += new Task.TickHandler((sender, e) => eventCount++);

            DateTime now = new DateTime(2000, 1, 1, 1, 1, 1, 0);
            tasks.Tick(now);

            // The time does not matter as no condition is put on the task. It'll fire at all.
            now = now.AddMilliseconds(random.Next(100));
            tasks.Tick(now);

            now = now.AddMilliseconds(random.Next(100));
            tasks.Tick(now);

            Assert.AreEqual(3, eventCount);
        }

        /// <summary>
        /// Tests that the event is fired at least once in 100 milliseconds with a 10 ms tick.
        /// </summary>
        [Test]
        public void TestTaskInternalEvent() {
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            // Add a task with zero conditions.
            tasks.Add(
                new Task()
            ).Tick += new Task.TickHandler((sender, e) => autoEvent.Set());

            tasks.Start();

            Assert.IsTrue(autoEvent.WaitOne(100));
        }

        /// <summary>
        /// Tests that disposing the controller removes all tasks and disposes them as well.
        /// </summary>
        [Test]
        public void TestTaskDispose() {
            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            Task testTask = new Task() {
                Condition = new Temporal() {
                    (date, task) => date.Millisecond % 10 == 0
                },
                Delay = DateTime.Now,
                Termination = DateTime.Now,
                Name = "Dispose",
                Tag = "Tag"
            };

            tasks.Add(testTask);

            tasks.Dispose();

            Assert.AreEqual(0, tasks.Count);
            Assert.IsNull(testTask.Condition);
            Assert.IsNull(testTask.Delay);
            Assert.IsNull(testTask.Termination);
            Assert.IsNull(testTask.Name);
            Assert.IsNull(testTask.Tag);
        }

        /// <summary>
        /// Test ticking several times to make sure our event is fired when the condition is met.
        /// </summary>
        [Test]
        public void TestTaskConditionalEvent() {
            int eventCount = 0;

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Millisecond % 10 == 0
                    }
                }
            ).Tick += new Task.TickHandler((sender, e) => eventCount++);

            DateTime now = new DateTime(2000, 1, 1, 1, 1, 1, 0);
            tasks.Tick(now);

            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            Assert.AreEqual(3, eventCount);
        }

        /// <summary>
        /// Checks that a task is terminated once the termination date/time has elapsed.
        /// </summary>
        [Test]
        public void TestTaskConditionalEventWithTermination() {
            int eventCount = 0;

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Millisecond % 10 == 0
                    },
                    Termination = new DateTime(2000, 1, 1, 1, 1, 1, 10)
                }
            ).Tick += new Task.TickHandler((sender, e) => eventCount++);

            DateTime now = new DateTime(2000, 1, 1, 1, 1, 1, 0);
            tasks.Tick(now);

            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            // The task should have terminated on the last call so the event
            // will never be fired.
            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            Assert.AreEqual(2, eventCount);
        }

        /// <summary>
        /// Test that delays events will not fire if the task is delayed.
        /// </summary>
        [Test]
        public void TestTaskDelayEvent() {
            int eventCount = 0;

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            tasks.Add(
                new Task() {
                    Delay = new DateTime(2000, 1, 1, 1, 1, 1, 10)
                }
            ).Tick += new Task.TickHandler((sender, e) => eventCount++);

            // This will not fire the event because it has ticked before the delay.
            DateTime now = new DateTime(2000, 1, 1, 1, 1, 1, 0);
            tasks.Tick(now);

            // This ticked on the delay, so this will be fired.
            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            now = now.AddMilliseconds(10);
            tasks.Tick(now);

            Assert.AreEqual(2, eventCount);
        }

        /// <summary>
        /// Test that any date time we pass will fire an event if no condition is added.
        /// </summary>
        [Test]
        public void TestTaskStoppedEvent() {
            int eventCount = 0;
            Random random = new Random();

            TaskController tasks = new TaskController(10) {
                Enabled = true
            };

            tasks.Add(
                new Task()
            ).Tick += new Task.TickHandler((sender, e) => eventCount++);

            DateTime now = new DateTime(2000, 1, 1, 1, 1, 1, 0);
            tasks.Tick(now);

            // The time does not matter as no condition is put on the task. It'll fire at all.
            now = now.AddMilliseconds(random.Next(100));
            tasks.Tick(now);

            // Now kill the task controller.
            tasks.Stop();

            // The task controller is not enabled 
            now = now.AddMilliseconds(random.Next(100));
            tasks.Tick(now);

            Assert.AreEqual(2, eventCount);
        }
    }
}
