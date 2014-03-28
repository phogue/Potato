namespace Procon.Service.Shared {
    /// <summary>
    /// Handles functionality affecting the running process.
    /// </summary>
    public interface IProcess {
        /// <summary>
        /// Kill the current process.
        /// </summary>
        void Kill();
    }
}
