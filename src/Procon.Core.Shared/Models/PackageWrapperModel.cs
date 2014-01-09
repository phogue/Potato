using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Wraps a package with what is known about the state of the package
    /// </summary>
    [Serializable]
    public class PackageWrapperModel : CoreModel {
        /// <summary>
        /// The id found in this models Installed or Available properties.
        /// </summary>
        public String Id {
            get {
                return this.Available.Id ?? this.Installed.Id;
            }
        }

        /// <summary>
        /// What state this package is in. Is it installed or not,
        /// updateable or is just installed.
        /// </summary>
        public PackageState State { get; set; }

        /// <summary>
        /// The currently installed version of the package.
        /// </summary>
        public PackageModel Installed { get; set; }

        /// <summary>
        /// The available version of the package form the source.
        /// </summary>
        public PackageModel Available { get; set; }

        /// <summary>
        /// Initializes the wrapper with the default values.
        /// </summary>
        public PackageWrapperModel() {
            this.Installed = new PackageModel();
            this.Available = new PackageModel();
        }
    }
}
