using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// The current state of a package (installed, updatable or installed with no update)
    /// </summary>
    [Serializable]
    public enum PackageState {
        /// <summary>
        /// Package is not installed and is available on remote server
        /// </summary>
        NotInstalled,
        /// <summary>
        /// The package is installed and up to date
        /// </summary>
        Installed,
        /// <summary>
        /// Package installed, but version is out of date.
        /// </summary>
        UpdateAvailable
    }
}
