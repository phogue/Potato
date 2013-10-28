using System;
using System.Collections.Generic;
using System.Timers;

namespace Procon.Core.Scheduler {

    /// <summary>
    /// I should really catch up on 4.0's security changes so we can use a BlockingCollection here.
    /// </summary>
    [Serializable]
    public class TaskController : List<Task>, IDisposable {

        /// <summary>
        /// The timer to tick every second, checking against the Tasks
        /// </summary>
        protected Timer Ticker { get; set; }

        /// <summary>
        /// Simple boolean used in start/stop as a flag to determine if tasks should be
        /// processed or not. We don't rely on the enabled propery o the ticker to allow
        /// for unit testing.
        /// </summary>
        public bool Enabled { get; set; }

        public TaskController(double interval = 1000) {
            this.Ticker = new Timer(interval);
            this.Ticker.Elapsed += new ElapsedEventHandler(Ticker_Elapsed);
        }

        public void Dispose() {
            lock (this) {
                this.Stop();

                foreach (Task task in this) {
                    task.Dispose();
                }

                this.Clear();
            }
        }

        private void Ticker_Elapsed(object sender, ElapsedEventArgs e) {
            this.Tick();
        }

        public void Tick(DateTime? now = null) {
            if (this.Enabled == true) {
                if (now.HasValue == false) {
                    now = DateTime.Now;
                }

                lock (this) {
                    // Clones the enumerator to avoid threading problems when altering the list
                    foreach (Task task in this) {
                        task.Check(now.Value);
                    }

                    // Dispose any tasks that have expired.
                    this.ForEach(task => {
                        if (task.IsExpired(now.Value) == true) {
                            task.Dispose();
                        }
                    });

                    // Removes all tasks that have expired
                    this.RemoveAll(task => task.IsExpired(now.Value) == true);

                }
            }
        }

        /// <summary>
        /// Adds a Task to the TaskController and returns the item
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>The added item</returns>
        public new Task Add(Task item) {
            lock (this) {
                base.Add(item);
            }

            return item;
        }

        public new TaskController Clear() {
            lock (this) {
                foreach (Task task in this) {
                    task.Dispose();
                }

                base.Clear();
            }

            return this;
        }

        /// <summary>
        /// Starts the TaskController ticking and checking if its tasks need
        /// their events fired
        /// </summary>
        /// <returns>Itself</returns>
        public TaskController Start() {
            this.Ticker.Enabled = true;
            this.Ticker.Start();

            this.Enabled = true;

            return this;
        }

        /// <summary>
        /// Stops the TaskController ticking
        /// </summary>
        /// <returns>Itself</returns>
        public TaskController Stop() {
            this.Ticker.Enabled = false;
            this.Ticker.Stop();

            this.Enabled = false;

            return this;
        }
    }
}
