using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class PackageModel : CoreModel {

        /// <summary>
        /// The path to save xml packages to
        /// </summary>
        public String PackagesUpdatesPath { get; set; }

        /// <summary>
        /// What repository this package belongs to
        /// </summary>
        public RepositoryModel Repository { get; set; }

        /// <summary>
        /// What state this package is in. Is it installed or not,
        /// updateable or has the update been downloaded and is pending
        /// an install on restart?
        /// </summary>
        public PackageState State { get; set; }

        /// <summary>
        /// The currently installed (or awaiting restart for update)
        /// version of the package.
        /// 
        /// See this.State for more information about which it is,
        /// but for the most part I see this variable as "What
        /// have we got right now?"
        /// </summary>
        public PackageVersionModel InstalledVersion { get; set; }

        /// <summary>
        /// The updated version of the package.
        /// 
        /// This is null if the the package is at its latest version
        /// (meaning there is no other available version, unless you
        /// wanted to go backwards, which is just silly... stop it.)
        /// </summary>
        public PackageVersionModel AvailableVersion { get; set; }

    }
}
