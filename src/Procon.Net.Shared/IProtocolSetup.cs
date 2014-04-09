using System;
using System.Collections.Generic;

namespace Procon.Net.Shared {
    /// <summary>
    /// Setup variables used when creating a new protocol
    /// </summary>
    public interface IProtocolSetup {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        String Hostname { get; }

        /// <summary>
        /// The port to connect on.
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// The password used to authenticate with the server.
        /// </summary>
        String Password { get; }

        /// <summary>
        /// A list of generic variables to us 
        /// </summary>
        IDictionary<String, String> Arguments { get; }

        /// <summary>
        /// Convert the variables dictionary to a simple string
        /// </summary>
        String ArgumentsString();

        /// <summary>
        /// The path for the protocol to look for configs at
        /// </summary>
        String ConfigDirectory { get; set; }
    }
}
