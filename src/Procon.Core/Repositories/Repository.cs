using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Net;
using Procon.Core.Utils;
using Procon.Net.Utils;

namespace Procon.Core.Repositories {
    using Procon.Net.Utils.HTTP;

    public class Repository : Executable {

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<Package> Packages { get; set; }

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

        protected Request QueryRequest;

        protected Request AuthenticationTestRequest;

        protected Request RebuildCacheRequest;

        protected Request PublishRequest;

        #region events

        public delegate void RepositoryEventHandler(Repository repository);

        /// <summary>
        /// Fired once the repository has been loaded. This means
        /// the repository has been loaded from file (installed/updates)
        /// or has been loaded from the remote repository.
        /// </summary>
        public event RepositoryEventHandler RepositoryLoaded;

        protected void OnRepositoryLoaded() {
            var handler = this.RepositoryLoaded;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Fired when a basic authentication test succeeds.
        /// </summary>
        public event RepositoryEventHandler AuthenticationSuccess;

        protected void OnAuthenticationSuccess() {
            var handler = this.AuthenticationSuccess;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Fired when a basic authentication test fails.
        /// Failure occurs when the url, username or password
        /// are incorrect.
        /// </summary>
        public event RepositoryEventHandler AuthenticationFailed;

        protected void OnAuthenticationFailed() {
            var handler = this.AuthenticationFailed;

            if (handler != null) {
                handler(this);
            }
        }

        public event RepositoryEventHandler RebuildCacheSuccess;

        protected void OnRebuildCacheSuccess() {
            var handler = this.RebuildCacheSuccess;

            if (handler != null) {
                handler(this);
            }
        }

        public event RepositoryEventHandler PublishSuccess;

        protected void OnPublishSuccess() {
            var handler = this.PublishSuccess;

            if (handler != null) {
                handler(this);
            }
        }
        
        #endregion

        // Default Initialization
        public Repository() : base() {
            this.Packages = new List<Package>();
        }

        /// <summary>
        /// Looks in the specified directory for package information and adds it to the packages list.
        /// </summary>
        /// <param name="repositoryPath">The path to look for xml files to load</param>
        /// <returns>A list of loaded packages from the directory</returns>
        public List<Package> ReadDirectory(String repositoryPath) {
            List<Package> loadedPackages = new List<Package>();

            if (Directory.Exists(repositoryPath)) {
                foreach (String packagePath in Directory.GetFiles(repositoryPath, "*.xml")) {
                    Package loadedPackage = null;

                    try {
                        loadedPackage = XElement.Load(packagePath).FromXElement<Package>();
                    }
                    catch {
                        loadedPackage = null;
                    }

                    if (loadedPackage != null) {
                        this.AddOrUpdatePackage(loadedPackage);

                        loadedPackages.Add(loadedPackage);
                    }
                }
            }

            return loadedPackages;
        }

        /// <summary>
        /// Copies the data of a package into the current repositories list of packages
        /// or if the package already exists then it will be updated with the passed in package.
        /// </summary>
        /// <param name="package">The source package to add or update.</param>
        public void AddOrUpdatePackage(Package package) {
            Package existingPackage = this.Packages.FirstOrDefault(p => p.Uid == package.Uid);

            if (existingPackage == null) {
                this.Packages.Add(package);

                // @todo add this back in once were set for events. OnPackageAdded(this, package);
            }
            else {
                existingPackage.Copy(package);
            }
        }

        /// <summary>
        /// Cancels an existing query request, unregisters the events and nulls the objects.
        /// </summary>
        protected void CloseQueryRequest() {
            if (this.QueryRequest != null) {
                this.QueryRequest.RequestComplete -= new Request.RequestEventDelegate(QueryRequest_RequestComplete);

                this.QueryRequest.EndRequest();
                this.QueryRequest = null;
            }
        }

        /// <summary>
        /// Begin asynchronous loading of the remote repository. 
        /// </summary>
        public void BeginQueryRequest() {
            if (this.Url.Length > 0) {
                this.CloseQueryRequest();

                this.QueryRequest = new Request(this.Url + "1/query/repository/format/xml");

                this.QueryRequest.RequestComplete += new Request.RequestEventDelegate(QueryRequest_RequestComplete);

                this.QueryRequest.BeginRequest();
            }
        }

        /// <summary>
        /// Takes a valid xelement and deserializes it over this repository.
        /// </summary>
        /// <param name="element"></param>
        private void ProcessRepository(XElement element) {
            foreach (XElement packageElement in element.Descendants("package")) {
                Package loadedPackage = packageElement.FromXElement<Package>();

                this.AddOrUpdatePackage(loadedPackage);
            }
        }

        private void QueryRequest_RequestComplete(Request sender) {
            String data = sender.GetResponseContent();

            XElement element = null;

            if (XElementValidator.TryParse(data, out element) == true) {
                this.ProcessRepository(element);

                this.LastQueryCompleted = DateTime.Now;
            }

            // Fire Updated Event.
            this.OnRepositoryLoaded();

            this.CloseQueryRequest();
        }

        protected void CloseAuthenticationTest() {
            if (this.AuthenticationTestRequest != null) {
                this.AuthenticationTestRequest.RequestError -= new Request.RequestEventDelegate(AuthenticationTestRequest_RequestError);
                this.AuthenticationTestRequest.RequestComplete -= new Request.RequestEventDelegate(AuthenticationTestRequest_RequestComplete);

                this.AuthenticationTestRequest.EndRequest();
                this.AuthenticationTestRequest = null;
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
                this.CloseAuthenticationTest();

                Uri uri = new Uri(this.Url + "1/publish/authentication_test/format/xml");

                this.AuthenticationTestRequest = new Request(uri.OriginalString);

                this.AuthenticationTestRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username, 
                        this.Password
                    )
                );

                this.AuthenticationTestRequest.RequestError += new Request.RequestEventDelegate(AuthenticationTestRequest_RequestError);
                this.AuthenticationTestRequest.RequestComplete += new Request.RequestEventDelegate(AuthenticationTestRequest_RequestComplete);

                this.AuthenticationTestRequest.BeginRequest();
            }
        }

        private void AuthenticationTestRequest_RequestComplete(Request sender) {
            this.OnAuthenticationSuccess();

            this.CloseAuthenticationTest();
        }

        private void AuthenticationTestRequest_RequestError(Request sender) {
            this.OnAuthenticationFailed();

            this.CloseAuthenticationTest();
        }

        protected void CloseRebuildCache() {
            if (this.RebuildCacheRequest != null) {
                this.RebuildCacheRequest.RequestComplete -= new Request.RequestEventDelegate(RebuildCacheRequest_RequestComplete);

                this.RebuildCacheRequest.EndRequest();
                this.RebuildCacheRequest = null;
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
                this.CloseRebuildCache();

                Uri uri = new Uri(this.Url + "1/publish/authentication_test/format/xml");

                this.RebuildCacheRequest = new Request(uri.OriginalString);

                this.RebuildCacheRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username,
                        this.Password
                    )
                );

                this.RebuildCacheRequest.RequestComplete += new Request.RequestEventDelegate(RebuildCacheRequest_RequestComplete);

                this.RebuildCacheRequest.BeginRequest();
            }
        }

        private void RebuildCacheRequest_RequestComplete(Request sender) {
            this.OnRebuildCacheSuccess();

            this.CloseRebuildCache();
        }

        protected void ClosePublishRequest() {
            if (this.PublishRequest != null) {
                this.PublishRequest.RequestComplete -= new Request.RequestEventDelegate(PublishRequest_RequestComplete);

                this.PublishRequest.EndRequest();
                this.PublishRequest = null;
            }
        }

        public void BeginPublish(Package package, Version version, MemoryStream zippedPackage) {
            if (this.Url.Length > 0) {
                this.CloseAuthenticationTest();

                Uri uri = new Uri(this.Url + "1/publish/submit/format/xml");

                this.PublishRequest = new Request(uri.OriginalString);

                this.PublishRequest.PostContent.Params.Add(new PostParameter("uid", package.Uid, PostParameterType.Field));
                this.PublishRequest.PostContent.Params.Add(new PostParameter("name", package.Name, PostParameterType.Field));
                this.PublishRequest.PostContent.Params.Add(new PostParameter("version", version.ToString(), PostParameterType.Field));

                this.PublishRequest.PostContent.Params.Add(new PostParameter("package", String.Format("{0}_{1}.zip", package.Uid, version), zippedPackage, "application/x-zip-compressed", PostParameterType.File));

                this.PublishRequest.CredentialCache.Add(
                    new Uri(uri.GetLeftPart(UriPartial.Authority)),
                    "Digest",
                    new NetworkCredential(
                        this.Username,
                        this.Password
                    )
                );

                this.PublishRequest.RequestComplete += new Request.RequestEventDelegate(PublishRequest_RequestComplete);

                this.PublishRequest.BeginRequest();
            }
        }

        private void PublishRequest_RequestComplete(Request sender) {
            this.OnPublishSuccess();

            this.ClosePublishRequest();
        }
    }
}
