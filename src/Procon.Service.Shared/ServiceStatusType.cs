namespace Procon.Service.Shared {
    /// <summary>
    /// The current state of the procon server running.
    /// </summary>
    public enum ServiceStatusType {
        /// <summary>
        /// Default value, nothing has been said about the status yet.
        /// </summary>
        None,
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
