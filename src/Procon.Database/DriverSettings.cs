using System;

namespace Procon.Database {
    /// <summary>
    /// The connection details for a driver to use when connecting/authenticating
    /// </summary>
    public class DriverSettings : IDriverSettings {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        public String Hostname { get; set; }

        /// <summary>
        /// The port to connect over.
        /// </summary>
        public uint? Port { get; set; }

        /// <summary>
        /// The username for authentication.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// The name of the database to select.
        /// </summary>
        public String Database { get; set; }

        /// <summary>
        /// If the database should exists in memory only, not on a file system.
        /// </summary>
        public bool Memory { get; set; }
    }
}
