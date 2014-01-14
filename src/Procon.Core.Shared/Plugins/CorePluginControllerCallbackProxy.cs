namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// A simple executable proxy known by Plugins and Core for callbacks
    /// from a plugin to the core.
    /// </summary>
    public class CorePluginControllerCallbackProxy : CoreController {
        public override object InitializeLifetimeService() {
            return null;
        }
    }
}
