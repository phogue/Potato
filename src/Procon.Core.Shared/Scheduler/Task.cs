using System;

namespace Procon.Core.Shared.Scheduler {

    [Serializable]
    public sealed class Task : IDisposable {

        /// <summary>
        /// Fires every time the current date time is after the delay
        /// and it returns true from the conditions predicates
        /// </summary>
        public event TickHandler Tick;
        public delegate void TickHandler(Object sender, TickEventArgs e);

        /// <summary>
        /// The name of this task.  This could be used to later remove any tasks
        /// with the current name.  This is not unique to the task controller.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A custom object to tag along with this task.  When fired the registered
        /// parties could cast and use this object.  This property is not used by
        /// the Scheduler itself.
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// A list of predicates that will have the current DateTime passed to them
        /// to be checked.
        /// </summary>
        public Temporal Condition { get; set; }

        /// <summary>
        /// The delay until the conditions are checked.  This is just
        /// like delaying all execution until at least DateTime X.
        /// 
        /// If null the tasks conditions will be checked immediately.  This is the same
        /// as setting the Delay to the current date time.
        /// </summary>
        public DateTime? Delay { get; set; }

        /// <summary>
        /// The DateTime this task is to be removed at (or after)
        /// 
        /// If null the task will never be terminated
        /// </summary>
        public DateTime? Termination { get; set; }

        public Task() {
            this.Delay = null;
            this.Termination = null;
        }

        private void OnTickEvent(DateTime date) {
            var handler = this.Tick;

            if (handler != null) {
                handler(this, new TickEventArgs() {
                    TickedAt = date
                });
            }
        }

        /// <summary>
        /// Checks if the passed DateTime is after the delay and meets the conditions.
        /// 
        /// Fires the Tick event when the task meets the conditions
        /// </summary>
        /// <param name="date">The date and time to check against</param>
        public void Check(DateTime date) {

            // If we're beyond the delay point
            if (this.Delay.HasValue == false || date >= this.Delay.Value) {

                // If it has additional conditions on the task
                if (this.Condition != null) {

                    // Make sure the additional conditions are met, then fire if they are.
                    if (this.Condition.Check(date, this) == true) {
                        this.OnTickEvent(date);
                    }
                }
                // No additional conditions, fire the event if registered
                else {
                    this.OnTickEvent(date);
                }
            }
        }

        /// <summary>
        /// Checks if the task is up for removing (after the termination date time)
        /// </summary>
        /// <param name="dt">The date time to check against</param>
        /// <returns>True, the task has expired and is ready to be removed.  False otherwise.</returns>
        public bool IsExpired(DateTime dt) {
            return (this.Termination.HasValue == true && this.Termination.Value.Ticks > 0 && dt >= this.Termination.Value);
        }

        public void Dispose() {
            this.Name = null;
            this.Tag = null;
            this.Condition = null;
            this.Delay = null;
            this.Termination = null;
            this.Tick = null;
        }
    }
}
