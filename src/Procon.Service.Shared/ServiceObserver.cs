using System;
using System.Threading;

namespace Procon.Service.Shared {
    /// <summary>
    /// Observer implementation of the 
    /// </summary>
    public class ServiceObserver : IServiceObserver {
        private ServiceStatusType _status;

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
        /// Fired every twenty minutes to ensure Procon has not been down stopped for 15 minutes
        /// </summary>
        private void PanicTask_Tick(object state) {
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
    }
}
