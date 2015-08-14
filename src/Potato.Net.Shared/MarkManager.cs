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
using System.Linq;

namespace Potato.Net.Shared {
    public class MarkManager {

        /// <summary>
        /// List of connection attempts used for a capped exponential backoff of reconnection attempts.
        /// </summary>
        public List<DateTime> Marks { get; set; }

        /// <summary>
        /// The maxmimum number of seconds to hold connection attempts. The default is set to 600, 10 minutes.
        /// </summary>
        public uint MaximumMarkAge { get; set; }

        /// <summary>
        /// Lock used to control access to the Attempts list.
        /// </summary>
        protected readonly object MarkListLock = new object();

        public MarkManager() {
            Marks = new List<DateTime>();

            MaximumMarkAge = 600;
        }

        /// <summary>
        /// Marks the current connection attempt.
        /// </summary>
        public virtual MarkManager Mark(DateTime? time = null) {
            time = time ?? DateTime.Now;

            lock (MarkListLock) {
                Marks.Add(time.Value);
            }

            return this;
        }

        /// <summary>
        /// Removes all connection attempts that have expired.
        /// </summary>
        public virtual MarkManager RemoveExpiredMarks() {
            lock (MarkListLock) {
                Marks.RemoveAll(time => time < DateTime.Now.AddSeconds(MaximumMarkAge * -1));
            }

            return this;
        }

        /// <summary>
        /// Checks if a connection attempt is allowed, or if it should be ignored because we have attempted
        /// a connection too often.
        /// </summary>
        /// <returns>True if a connection attempt is valid, false if connection shouldn't be attempted</returns>
        public virtual bool IsValidMarkWindow() {
            var valid = true;

            if (Marks.Count > 0) {
                DateTime recentAttempt;

                lock (MarkListLock) {
                    recentAttempt = Marks.OrderByDescending(time => time).First();
                }

                valid = recentAttempt < DateTime.Now.AddSeconds(Math.Pow(2, Marks.Count) * -1);

            }
            // else no connection attempts, allow.

            return valid;
        }
    }
}
