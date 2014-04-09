using System;

namespace Procon.Net.Shared.Sandbox {
    /// <summary>
    /// Callback for to be initialized in the host domain, set and passed into
    /// the sandboxed domain allowing for events to be pushed through.
    /// </summary>
    public interface ISandboxProtocolCallbackProxy {
        /// <summary>
        /// Called when ever a dispatched game event occurs.
        /// </summary>
        Action<IProtocolShared, IProtocolEventArgs> ProtocolEvent { get; set; }

        /// <summary>
        /// Called when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        Action<IProtocolShared, IClientEventArgs> ClientEvent { get; set; }
    }
}
