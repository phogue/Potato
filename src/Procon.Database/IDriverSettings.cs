using System;

namespace Procon.Database {
    /// <summary>
    /// The connection details for a driver to use when connecting/authenticating
    /// </summary>
    public interface IDriverSettings {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        String Hostname { get; set; }

        /// <summary>
        /// The port to connect over.
        /// </summary>
        uint? Port { get; set; }

        /// <summary>
        /// The username for authentication.
        /// </summary>
        String Username { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        String Password { get; set; }

        /// <summary>
        /// The name of the database to select.
        /// </summary>
        String Database { get; set; }

        /// <summary>
        /// If the database should exists in memory only, not on a file system.
        /// </summary>
        bool Memory { get; set; }
    }
}
