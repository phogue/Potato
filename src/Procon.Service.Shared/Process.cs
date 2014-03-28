namespace Procon.Service.Shared {
    /// <summary>
    /// Handles functionality affecting the running process, but more mockable.
    /// </summary>
    public class Process : IProcess {
        public void Kill() {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
