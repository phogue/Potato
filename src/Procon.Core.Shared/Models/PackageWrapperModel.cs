using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class PackageWrapperModel : CoreModel {
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
        /// The updatable version of the package.
        /// </summary>
        public PackageModel Available { get; set; }
    }
}
