using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Core.Utils;
    using Procon.Net.Utils.HTTP;

    public class Repository : Executable<Repository> {

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<Package> Packages {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
                }
            }
        }
        private List<Package> mPackages;

        /// <summary>
        /// The base url of the repository with trailing slash '/'
        /// </summary>
        public String Url {
            get { return mUrl; }
            set {
                if (mUrl != value) {
                    mUrl = value;
                    this.UrlStub = this.GetUrlStub();

                    OnPropertyChanged(this, "Url");
                }
            }
        }
        private String mUrl;

        /// <summary>
        /// Short directory safe url
        /// </summary>
        public String UrlStub {
            get; set;
        }

        /// <summary>
        /// When the repository was last checked for updates
        /// </summary>
        public DateTime LastUpdate {
            get { return mLastUpdate; }
            protected set {
                if (mLastUpdate != value) {
                    mLastUpdate = value;
                    OnPropertyChanged(this, "LastUpdate");
                }
            }
        }
        private DateTime mLastUpdate;

        /// <summary>
        /// The name of this repository
        /// </summary>
        public String Name {
            get { return mName; }
            set {
                if (mName != value) {
                    mName = value;
                    OnPropertyChanged(this, "Name");
                }
            }
        }
        private String mName;

        protected Request mRequest;

        #region events

        public delegate void RepositoryLoadedHandler(Repository repository);
        public event RepositoryLoadedHandler RepositoryLoaded;

        #endregion

        // Default Initialization
        public Repository() : base() {
            this.Packages = new List<Package>();
        }

        protected String GetUrlStub() {
            Uri uri = new Uri(this.Url);

            String stub = uri.Host + uri.PathAndQuery;

            stub = Regex.Replace(stub, "[/]+", "_").Trim('_');
            stub = Regex.Replace(stub, "[^\\w]+", "");

            return stub;
        }

        protected void CancelUpdate() {
            if (this.mRequest != null) {
                this.mRequest.EndRequest();
                this.mRequest = null;
            }
        }

        /// <summary>
        /// Looks in the specified directory for package information and adds it to the packages list.
        /// </summary>
        public void ReadDirectory(String repositoryPath) {

            if (Directory.Exists(repositoryPath)) {
                foreach (String packagePath in Directory.GetFiles(repositoryPath, "*.xml")) {
                    try {
                        AddOrCopyPackage(new Package().Parse(XElement.Load(packagePath)));
                    }
                    catch (Exception) { }
                }
            }
        }

        private void AddOrCopyPackage(Package package) {

            Package currentPackage = null;
            if ((currentPackage = this.Packages.Where(x => x.Uid == package.Uid).FirstOrDefault() as Package) == null) {
                this.Packages.Add(package);
                // @todo add this back in once were set for events. OnPackageAdded(this, package);
            }
            else {
                currentPackage.Copy(package);
            }
        }

        public void BeginLoading() {
            if (this.Url.Length > 0) {
                this.CancelUpdate();

                this.mRequest = new Request(this.Url + "1/query/repository/format/xml");

                this.mRequest.RequestComplete += new Request.RequestEventDelegate(mRequest_RequestComplete);

                this.mRequest.BeginRequest();
            }
        }

        private void ProcessRepository(XElement element) {
            foreach (XElement packageElement in element.Descendants("package")) {
                this.AddOrCopyPackage(new Package().Parse(packageElement));
            }
        }

        private void mRequest_RequestComplete(Request sender) {
            String data = sender.GetResponseContent();

            try {
                this.ProcessRepository(XElement.Parse(data));

                if (this.RepositoryLoaded != null) {
                    this.RepositoryLoaded(this);
                }
                // Fire Updated Event.
            }
            catch (Exception) {
                // More than likely syntax error in the xml.
            }
        }

    }
}
