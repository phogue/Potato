using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net {
    public class ConnectionAttemptManager {

        /// <summary>
        /// List of connection attempts used for a capped exponential backoff of reconnection attempts.
        /// </summary>
        protected List<DateTime> Attempts { get; set; }

        /// <summary>
        /// The maxmimum number of seconds to hold connection attempts. The default is set to 600, 10 minutes.
        /// </summary>
        public uint MaximumAttemptAge { get; set; }

        /// <summary>
        /// Lock used to control access to the Attempts list.
        /// </summary>
        protected readonly Object AttemptsLock = new Object();

        public ConnectionAttemptManager() {
            this.Attempts = new List<DateTime>();

            this.MaximumAttemptAge = 600;
        }

        /// <summary>
        /// Marks the current connection attempt.
        /// </summary>
        public virtual ConnectionAttemptManager MarkAttempt() {
            lock (this.AttemptsLock) {
                this.Attempts.Add(DateTime.Now);
            }

            return this;
        }

        /// <summary>
        /// Removes all connection attempts that have expired.
        /// </summary>
        public virtual ConnectionAttemptManager RemoveExpiredAttempts() {
            lock (this.AttemptsLock) {
                this.Attempts.RemoveAll(time => time < DateTime.Now.AddSeconds(this.MaximumAttemptAge * -1));
            }

            return this;
        }

        /// <summary>
        /// Checks if a connection attempt is allowed, or if it should be ignored because we have attempted
        /// a connection too often.
        /// </summary>
        /// <returns>True if a connection attempt is valid, false if connection shouldn't be attempted</returns>
        public virtual bool IsAttemptAllowed() {
            bool isAllowed = true;

            if (this.Attempts.Count > 0) {
                DateTime recentAttempt;

                lock (this.AttemptsLock) {
                    recentAttempt = this.Attempts.OrderByDescending(time => time).First();
                }

                isAllowed = recentAttempt < DateTime.Now.AddSeconds(Math.Pow(2, this.Attempts.Count) * -1);

            }
            // else no connection attempts, allow.

            return isAllowed;
        }
    }
}
