using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// Setup variables used when creating a new client
    /// </summary>
    public interface IClientSetup {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        String Hostname { get; }

        /// <summary>
        /// The port to connect on.
        /// </summary>
        ushort Port { get; }
    }
}
