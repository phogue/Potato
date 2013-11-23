using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net {
    public class MarkManager {

        /// <summary>
        /// List of connection attempts used for a capped exponential backoff of reconnection attempts.
        /// </summary>
        protected List<DateTime> Marks { get; set; }

        /// <summary>
        /// The maxmimum number of seconds to hold connection attempts. The default is set to 600, 10 minutes.
        /// </summary>
        public uint MaximumMarkAge { get; set; }

        /// <summary>
        /// Lock used to control access to the Attempts list.
        /// </summary>
        protected readonly Object MarkListLock = new Object();

        public MarkManager() {
            this.Marks = new List<DateTime>();

            this.MaximumMarkAge = 600;
        }

        /// <summary>
        /// Marks the current connection attempt.
        /// </summary>
        public virtual MarkManager Mark() {
            lock (this.MarkListLock) {
                this.Marks.Add(DateTime.Now);
            }

            return this;
        }

        /// <summary>
        /// Removes all connection attempts that have expired.
        /// </summary>
        public virtual MarkManager RemoveExpiredMarks() {
            lock (this.MarkListLock) {
                this.Marks.RemoveAll(time => time < DateTime.Now.AddSeconds(this.MaximumMarkAge * -1));
            }

            return this;
        }

        /// <summary>
        /// Checks if a connection attempt is allowed, or if it should be ignored because we have attempted
        /// a connection too often.
        /// </summary>
        /// <returns>True if a connection attempt is valid, false if connection shouldn't be attempted</returns>
        public virtual bool IsValidMarkWindow() {
            bool valid = true;

            if (this.Marks.Count > 0) {
                DateTime recentAttempt;

                lock (this.MarkListLock) {
                    recentAttempt = this.Marks.OrderByDescending(time => time).First();
                }

                valid = recentAttempt < DateTime.Now.AddSeconds(Math.Pow(2, this.Marks.Count) * -1);

            }
            // else no connection attempts, allow.

            return valid;
        }
    }
}
