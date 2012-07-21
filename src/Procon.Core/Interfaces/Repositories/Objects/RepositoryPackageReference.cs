using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Net.Utils;

    public class RepositoryPackageReference {

        /// <summary>
        /// The repository url stub. This is used as a unique
        /// id for the repository
        /// </summary>
        public String RepositoryUrlStub {
            get { return this.mRepositoryUrlStub; }
            set {
                this.mRepositoryUrlStub = value.UrlStub();
            }
        }
        private String mRepositoryUrlStub;

        /// <summary>
        /// The sanitized uid of the package
        /// </summary>
        public String PackageUid {
            get { return this.mPackageUid; }
            set {
                this.mPackageUid = value.SanitizeDirectory();
            }
        }
        private String mPackageUid;

    }
}
