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
        ///  The actual game object
        /// </summary>
        IProtocolShared Protocol { get; set; }

        /// <summary>
        /// Proxy to the active protocol state
        /// </summary>
        IProtocolState ProtocolState { get; }
    }
}
