using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// The basic interface of communication between Core and Net
    /// </summary>
    public interface IProtocol : IProtocolShared {
        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        event Action<IProtocol, IClientEventArgs> ClientEvent;
    }
}
