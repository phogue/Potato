using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class RepositoryModel : CoreModel {

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<PackageModel> Packages { get; set; }

        /// <summary>
        /// The base url of the repository with trailing slash '/'
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// Short directory safe url
        /// </summary>
        public String UrlSlug { get; set; }

        /// <summary>
        /// When the repository was last checked for updates
        /// </summary>
        public DateTime LastQueryCompleted { get; set; }

        /// <summary>
        /// The name of this repository
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Username is only used for methods that require authentication
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// Password is only used for methods that require authentication
        /// </summary>
        public String Password { get; set; }

        public RepositoryModel() : base() {
            this.Packages = new List<PackageModel>();
        }
    }
}
