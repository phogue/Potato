using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class RepositoryModel : CoreModel {
        /// <summary>
        /// The base url of the repository with trailing slash '/'
        /// </summary>
        public String Uri { get; set; }

        /// <summary>
        /// Short directory safe url
        /// </summary>
        public String Slug { get; set; }

        /// <summary>
        /// When the repository was last checked for updates
        /// </summary>
        public DateTime LastQueryCompleted { get; set; }

        /// <summary>
        /// The name of this repository
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<PackageWrapperModel> Packages { get; set; }

        public RepositoryModel() : base() {
            this.Packages = new List<PackageWrapperModel>();
        }
    }
}
