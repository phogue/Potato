namespace Procon.Service {
    public enum ServiceStatusType {
        /// <summary>
        /// Service is currently stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// Service is in the process of stopping
        /// </summary>
        Stopping,
        /// <summary>
        /// Service is in the process of starting
        /// </summary>
        Starting,
        /// <summary>
        /// Service is currently running.
        /// </summary>
        Started
    }
}
