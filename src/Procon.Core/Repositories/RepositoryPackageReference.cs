using System;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Repositories {
    using Procon.Net.Utils;

    public class RepositoryPackageReference {

        /// <summary>
        /// The repository url stub. This is used as a unique
        /// id for the repository
        /// </summary>
        public String RepositoryUrlSlug {
            get { return this._repositoryUrlSlug; }
            set {
                this._repositoryUrlSlug = value.UrlSlug();
            }
        }
        private String _repositoryUrlSlug;

        /// <summary>
        /// The sanitized uid of the package
        /// </summary>
        public String PackageUid {
            get { return this._packageUid; }
            set {
                this._packageUid = value.SanitizeDirectory();
            }
        }
        private String _packageUid;

    }
}
