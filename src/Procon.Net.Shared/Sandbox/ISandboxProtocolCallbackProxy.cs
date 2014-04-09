using System;

namespace Procon.Net.Shared.Sandbox {
    /// <summary>
    /// Callback for to be initialized in the host domain, set and passed into
    /// the sandboxed domain allowing for events to be pushed through.
    /// </summary>
    public interface ISandboxProtocolCallbackProxy {
        /// <summary>
        /// Fires a protocol event back across the appdomain
        /// </summary>
        void FireProtocolEvent(IProtocolEventArgs args);

        /// <summary>
        /// Fires a client event back across the appdomain
        /// </summary>
        void FireClientEvent(IClientEventArgs args);
    }
}
