using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Core.Utils;
    using Procon.Net.Utils;
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
                    this.UrlStub = this.Url.UrlStub();

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

        /// <summary>
        /// Username is only used for methods that require authentication
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// Password is only used for methods that require authentication
        /// </summary>
        public String Password { get; set; }

        protected Request mQueryRequest;

        protected Request mAuthenticationTestRequest;

        protected Request mRebuildCacheRequest;

        protected Request mPublishRequest;


        #region events

        public delegate void RepositoryEventHandler(Repository repository);

        /// <summary>
        /// Fired once the repository has been loaded. This means
        /// the repository has been loaded from file (installed/updates)
        /// or has been loaded from the remote repository.
        /// </summary>
        public event RepositoryEventHandler RepositoryLoaded;

        /// <summary>
        /// Fired when a basic authentication test succeeds.
        /// </summary>
        public event RepositoryEventHandler AuthenticationSuccess;

        /// <summary>
        /// Fired when a basic authentication test fails.
        /// Failure occurs when the url, username or password
        /// are incorrect.
        /// </summary>
        public event RepositoryEventHandler AuthenticationFailed;

        public event RepositoryEventHandler RebuildCacheSuccess;
        public event RepositoryEventHandler RebuildCacheFailed;

        public event RepositoryEventHandler PublishSuccess;
        public event RepositoryEventHandler PublishFailed;
        
        #endregion

        // Default Initialization
        public Repository() : base() {
            this.Packages = new List<Package>();
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
                currentPackage.Copy<Package>(package);
            }
        }

        protected void CancelLoading() {
            if (this.mQueryRequest != null) {
                this.mQueryRequest.EndRequest();
                this.mQueryRequest = null;
            }
        }

        /// <summary>
        /// Begin asynchronous loading of the remote repository. 
        /// </summary>
        public void BeginLoading() {
            if (this.Url.Length > 0) {
                this.CancelLoading();

                this.mQueryRequest = new Request(this.Url + "1/query/repository/format/xml");

                this.mQueryRequest.RequestComplete += new Request.RequestEventDelegate(mQueryRequest_RequestComplete);

                this.mQueryRequest.BeginRequest();
            }
        }

        private void ProcessRepository(XElement element) {
            foreach (XElement packageElement in element.Descendants("package")) {
                this.AddOrCopyPackage(new Package().Parse(packageElement));
            }
        }

        private void mQueryRequest_RequestComplete(Request sender) {
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

        protected void CancelAuthenticationTest() {
            if (this.mAuthenticationTestRequest != null) {
                this.mAuthenticationTestRequest.EndRequest();
                this.mAuthenticationTestRequest = null;
            }
        }

        /// <summary>
        /// Begin asynchronous authentication test. This does not essentially do anything
        /// but test that the user has setup their repository properly.
        /// 
        /// This method is only ever relevant for publishing updates to a repository.
        /// </summary>
        public void BeginAuthenticationTest() {
            if (this.Url.Length > 0) {
                this.CancelAuthenticationTest();

                Uri uri = new Uri(this.Url + "1/publish/authentication_test/format/xml");

                this.mAuthenticationTestRequest = new Request(uri.OriginalString);

                this.mAuthenticationTestRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username, 
                        this.Password
                    )
                );

                this.mAuthenticationTestRequest.RequestError += new Request.RequestEventDelegate(mAuthenticationTestRequest_RequestError);
                this.mAuthenticationTestRequest.RequestComplete += new Request.RequestEventDelegate(mAuthenticationTestRequest_RequestComplete);

                this.mAuthenticationTestRequest.BeginRequest();
            }
        }

        private void mAuthenticationTestRequest_RequestError(Request sender) {
            if (this.AuthenticationFailed != null) {
                this.AuthenticationFailed(this);
            }
        }

        private void mAuthenticationTestRequest_RequestComplete(Request sender) {
            if (this.AuthenticationSuccess != null) {
                this.AuthenticationSuccess(this);
            }
        }









        protected void CancelRebuildCache() {
            if (this.mRebuildCacheRequest != null) {
                this.mRebuildCacheRequest.EndRequest();
                this.mRebuildCacheRequest = null;
            }
        }

        /// <summary>
        /// Begin asynchronous authentication test. This does not essentially do anything
        /// but test that the user has setup their repository properly.
        /// 
        /// This method is only ever relevant for publishing updates to a repository.
        /// </summary>
        public void BeginRebuildCache() {
            if (this.Url.Length > 0) {
                this.CancelRebuildCache();

                Uri uri = new Uri(this.Url + "1/publish/authentication_test/format/xml");

                this.mRebuildCacheRequest = new Request(uri.OriginalString);

                this.mRebuildCacheRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username,
                        this.Password
                    )
                );

                this.mRebuildCacheRequest.RequestError += new Request.RequestEventDelegate(mRebuildCacheRequest_RequestError);
                this.mRebuildCacheRequest.RequestComplete += new Request.RequestEventDelegate(mRebuildCacheRequest_RequestComplete);

                this.mRebuildCacheRequest.BeginRequest();
            }
        }

        private void mRebuildCacheRequest_RequestError(Request sender) {
            if (this.RebuildCacheFailed != null) {
                this.RebuildCacheFailed(this);
            }
        }

        private void mRebuildCacheRequest_RequestComplete(Request sender) {
            if (this.RebuildCacheSuccess != null) {
                this.RebuildCacheSuccess(this);
            }
        }











        protected void CancelPublish() {
            if (this.mPublishRequest != null) {
                this.mPublishRequest.EndRequest();
                this.mPublishRequest = null;
            }
        }

        public void BeginPublish(Package package, Version version, MemoryStream zippedPackage) {
            if (this.Url.Length > 0) {
                this.CancelAuthenticationTest();

                Uri uri = new Uri(this.Url + "1/publish/submit/format/xml");

                this.mPublishRequest = new Request(uri.OriginalString);

                this.mPublishRequest.PostContent.Params.Add(new PostParameter("uid", package.Uid, PostParameterType.Field));
                this.mPublishRequest.PostContent.Params.Add(new PostParameter("name", package.Name, PostParameterType.Field));
                this.mPublishRequest.PostContent.Params.Add(new PostParameter("version", version.ToString(), PostParameterType.Field));

                this.mPublishRequest.PostContent.Params.Add(new PostParameter("package", String.Format("{0}_{1}.zip", package.Uid, version.ToString()), zippedPackage, "application/x-zip-compressed", PostParameterType.File));

                this.mPublishRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username,
                        this.Password
                    )
                );

                this.mPublishRequest.RequestError += new Request.RequestEventDelegate(mPublishRequest_RequestError);
                this.mPublishRequest.RequestComplete += new Request.RequestEventDelegate(mPublishRequest_RequestComplete);

                this.mPublishRequest.BeginRequest();
            }
        }

        private void mPublishRequest_RequestComplete(Request sender) {
            if (this.PublishSuccess != null) {
                this.PublishSuccess(this);
            }
        }

        private void mPublishRequest_RequestError(Request sender) {
            if (this.PublishFailed != null) {
                this.PublishFailed(this);
            }
        }
    }
}
