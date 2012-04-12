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
using System.Timers;

namespace Procon.Core.Scheduler {

    [Serializable]
    public class TaskController : List<Task>, ICloneable {

        /// <summary>
        /// The timer to tick every second, checking against
        /// the Tasks
        /// </summary>
        private Timer Ticker;

        public TaskController() {
            this.Ticker = new Timer(1000);
            this.Ticker.Elapsed += new ElapsedEventHandler(Ticker_Elapsed);
        }

        private void Ticker_Elapsed(object sender, ElapsedEventArgs e) {

            DateTime now = DateTime.Now;

            // Clones the enumerator to avoid threading problems when altering the list
            foreach (Task task in this.Clone() as TaskController) {
                task.Check(now);
            }

            // Removes all tasks that have expired
            this.RemoveAll(x => x.IsExpired(now) == true);
        }

        /// <summary>
        /// Adds a Task to the TaskController and returns the item
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>The added item</returns>
        public new Task Add(Task item) {
            base.Add(item);

            return item;
        }

        /// <summary>
        /// Starts the TaskController ticking and checking if its tasks need
        /// their events fired
        /// </summary>
        /// <returns>Itself</returns>
        public TaskController Start() {
            this.Ticker.Enabled = true;
            this.Ticker.Start();

            return this;
        }

        /// <summary>
        /// Stops the TaskController ticking
        /// </summary>
        /// <returns>Itself</returns>
        public TaskController Stop() {
            this.Ticker.Enabled = false;
            this.Ticker.Stop();

            return this;
        }

        /// <summary>
        /// Clones the current object
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
