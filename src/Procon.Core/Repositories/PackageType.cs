using System;

namespace Procon.Core.Repositories {
    [Serializable]
    public enum PackageType {
        /// <summary>
        /// Package has no designation
        /// </summary>
        None,
        /// <summary>
        /// The package contains a plugin
        /// </summary>
        Plugin,
        /// <summary>
        /// The package contains a language pack
        /// </summary>
        Language,
        /// <summary>
        /// The package contains an application
        /// This might be restricted to official repositories only.
        /// </summary>
        Application
    }
}
