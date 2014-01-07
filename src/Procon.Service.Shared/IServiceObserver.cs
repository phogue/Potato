using System;

namespace Procon.Service.Shared {
    /// <summary>
    /// Used to track the state of the service
    /// </summary>
    public interface IServiceObserver {
        /// <summary>
        /// The current status of the service.
        /// </summary>
        ServiceStatusType Status { get; set; }

        /// <summary>
        /// When the service was started
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// When the service was stopped
        /// </summary>
        DateTime? StopTime { get; }

        /// <summary>
        /// Fetch the current uptime of the service
        /// </summary>
        /// <returns>How long the service has been started</returns>
        TimeSpan? Uptime();

        /// <summary>
        /// Fetch the current downtime of the service
        /// </summary>
        /// <returns>How long the service has been stopped</returns>
        TimeSpan? Downtime();
    }
}
