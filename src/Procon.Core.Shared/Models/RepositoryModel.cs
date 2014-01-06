using System;
using System.Collections.Generic;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A nuget repository source known to Procon.core
    /// </summary>
    [Serializable]
    public class RepositoryModel : CoreModel {
        /// <summary>
        /// The base url of the repository
        /// </summary>
        public String Uri { get; set; }

        /// <summary>
        /// Short directory safe url
        /// </summary>
        public String Slug { get; set; }

        /// <summary>
        /// The name of this repository
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<PackageWrapperModel> Packages { get; set; }

        /// <summary>
        /// Initializes a repository model with the default values.
        /// </summary>
        public RepositoryModel() : base() {
            this.Packages = new List<PackageWrapperModel>();
        }
    }
}
