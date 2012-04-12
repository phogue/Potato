// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Scheduler {

    [Serializable]
    public class Task {

        #region Events

        public delegate void TickHandler(Task sender, DateTime dt);

        /// <summary>
        /// Fires every time the current date time is after the delay
        /// and it returns true from the conditions predicates
        /// </summary>
        public event TickHandler Tick;

        #endregion

        /// <summary>
        /// The name of this task.  This could be used to later remove any tasks
        /// with the current name.  This is not unique to the task controller.
        /// </summary>
        public string Name { get; set; }

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
        public Temporal Conditions { get; set; }

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

        /// <summary>
        /// Checks if the passed DateTime is after the delay and meets the conditions.
        /// 
        /// Fires the Tick event when the task meets the conditions
        /// </summary>
        /// <param name="dt">The date and time to check against</param>
        public void Check(DateTime dt) {

            // If we're beyond the delay point
            if (this.Delay.HasValue == false || dt >= this.Delay.Value) {

                // If it has additional conditions on the task
                if (this.Conditions != null) {

                    // Make sure the additional conditions are met, then fire if they are.
                    if (this.Conditions.Check(dt) == true) {
                        if (this.Tick != null) {
                            this.Tick(this, dt);
                        }
                    }
                }
                // No additional conditions, fire the evet if registered
                else if (this.Tick != null) {
                    this.Tick(this, dt);
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
    }
}
