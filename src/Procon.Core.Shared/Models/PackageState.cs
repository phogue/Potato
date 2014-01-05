﻿using System;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public enum PackageState {
        /// <summary>
        /// Package is not installed and is available on remote server
        /// </summary>
        NotInstalled,
        /// <summary>
        /// Package installed, but version is out of date.
        /// </summary>
        UpdateAvailable,
        /// <summary>
        /// The package is installed and up to date
        /// </summary>
        Installed,
    }
}
