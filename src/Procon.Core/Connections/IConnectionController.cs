using System;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;

namespace Procon.Core.Connections {
    /// <summary>
    /// Handles connections, plugins and text commands for a single game server.
    /// </summary>
    public interface IConnectionController : ICoreController {
        /// <summary>
        /// Data about the protocol connection
        /// </summary>
        ConnectionModel ConnectionModel { get; set; }

        /// <summary>
        /// Fired when a protocol event is recieved from the protocol appdomain.
        /// </summary>
        event Action<IProtocolShared, IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when a client event is recieved from the protocol appdomain.
        /// </summary>
        event Action<IProtocolShared, IClientEventArgs> ClientEvent;

        /// <summary>
        /// Proxy to the active protocol state
        /// </summary>
        IProtocolState ProtocolState { get; }
    }
}
