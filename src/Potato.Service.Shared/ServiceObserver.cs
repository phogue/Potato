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
using System.Threading;

namespace Potato.Service.Shared {
    /// <summary>
    /// Observer implementation of the 
    /// </summary>
    public class ServiceObserver : IServiceObserver {
        private ServiceStatusType _status;

        /// <summary>
        /// Delegate called when the status is modified
        /// </summary>
        public Action<IServiceObserver, ServiceStatusType> StatusChange { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public ServiceStatusType Status {
            get {
                return _status;
            }
            set {
                if (_status != value) {
                    _status = value;

                    switch (this._status) {
                        case ServiceStatusType.Stopped:
                            this.StopTime = DateTime.Now;
                            break;
                        case ServiceStatusType.Started:
                            this.StartTime = DateTime.Now;
                            break;
                    }

                    this.OnStatusChange(this._status);
                }
            }
        }

        /// <summary>
        /// When the service was started
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// When the service was stopped
        /// </summary>
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// Handler to call when the observer has a panic about being stopped for so long.
        /// </summary>
        public Action Panic { get; set; }

        /// <summary>
        /// Self polling handler to ensure downtime has not exceeded fifteen minutes. If it has
        /// then it will call a panic handler.
        /// </summary>
        public Timer PanicTask { get; set; }

        /// <summary>
        /// Initializes the observer with the default values.
        /// </summary>
        public ServiceObserver() {
            this.Status = ServiceStatusType.Stopped;

            this.PanicTask = new Timer(PanicTask_Tick, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromMinutes(20));
        }

        /// <summary>
        /// Fired every twenty minutes to ensure Potato has not been down stopped for 15 minutes
        /// </summary>
        public void PanicTask_Tick(object state) {
            TimeSpan? downtime = this.Downtime();
            var handler = this.Panic;

            if (downtime.HasValue == true && downtime.Value > TimeSpan.FromMinutes(15) && handler != null) {
                // Time to panic.
                handler();
            }
        }

        /// <summary>
        /// Fetch the current uptime of the service
        /// </summary>
        /// <returns>How long the service has been started</returns>
        public TimeSpan? Uptime() {
            TimeSpan time = new TimeSpan(0);

            if (this.Status == ServiceStatusType.Started && this.StartTime.HasValue == true) {
                time = DateTime.Now - this.StartTime.Value;
            }

            return time;
        }

        /// <summary>
        /// Fetch the current downtime of the service
        /// </summary>
        /// <returns>How long the service has been stopped</returns>
        public TimeSpan? Downtime() {
            TimeSpan time = new TimeSpan(0);

            // We check here for any state that isn't completely started. Everything else
            // is assumed down.
            if (this.Status != ServiceStatusType.Started && this.StopTime.HasValue == true) {
                time = DateTime.Now - this.StopTime.Value;
            }

            return time;
        }

        /// <summary>
        /// Called when the status is modified
        /// </summary>
        /// <param name="status">The new status</param>
        protected void OnStatusChange(ServiceStatusType status) {
            var handler = this.StatusChange;

            if (handler != null) {
                handler(this, status);
            }
        }
    }
}
