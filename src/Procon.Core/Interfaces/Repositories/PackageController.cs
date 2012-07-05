using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;

    public abstract class PackageController : Executable<PackageController> {

        /// <summary>
        /// List of repositories external to procon that have packages to download
        /// </summary>
        public List<Repository> RemoteRepositories {
            get { return mRemoteRepositories; }
            protected set {
                if (mRemoteRepositories != value) {
                    mRemoteRepositories = value;
                    OnPropertyChanged(this, "RemoteRepositories");
                }
            }
        }
        private List<Repository> mRemoteRepositories;

        /// <summary>
        /// A list of repositories with any packages that have been installed
        /// </summary>
        public List<Repository> LocalInstalledRepositories {
            get { return mLocalInstalledRepositories; }
            protected set {
                if (mLocalInstalledRepositories != value) {
                    mLocalInstalledRepositories = value;
                    OnPropertyChanged(this, "LocalInstalledRepositories");
                }
            }
        }
        private List<Repository> mLocalInstalledRepositories;

        /// <summary>
        /// A list of repositories with any packages that have been downloaded, unzipped
        /// but are waiting for Procon to restart before they are installed.
        /// </summary>
        public List<Repository> LocalUpdatedRepositories {
            get { return mLocalUpdatedRepositories; }
            protected set {
                if (mLocalUpdatedRepositories != value) {
                    mLocalUpdatedRepositories = value;
                    OnPropertyChanged(this, "LocalUpdatedRepositories");
                }
            }
        }
        private List<Repository> mLocalUpdatedRepositories;

        /// <summary>
        /// List of flat packed packages that give a nice combined
        /// list of packages with potential updates.
        /// </summary>
        public List<FlatPackedPackage> Packages {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
                }
            }
        }
        private List<FlatPackedPackage> mPackages;

        // Default Initialization
        public PackageController() : base() {
            this.RemoteRepositories = new List<Repository>();
            this.LocalInstalledRepositories = new List<Repository>();
            this.LocalUpdatedRepositories = new List<Repository>();

            this.Packages = new List<FlatPackedPackage>();
        }

        #region events

        public delegate void PackagesRebuiltHandler(PackageController sender);
        public event PackagesRebuiltHandler PackagesRebuilt;

        #endregion

        protected FlatPackedPackage GetExistingFlatPackage(Repository repository, Package package) {
            return this.Packages.Where(x => x.Repository.UrlStub == repository.UrlStub && x.Uid == package.Uid).FirstOrDefault();
        }

        protected void AddOrUpdateFlatPackage(Repository repository, Package package, FlatPackedPackage flatPackage, PackageState defaultState = PackageState.NotInstalled) {
            FlatPackedPackage existingFlatPackage = this.GetExistingFlatPackage(repository, package);

            if (defaultState == PackageState.Installed) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy<Package>(flatPackage);
                }
                else {
                    flatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
            else if (defaultState == PackageState.UpdateInstalled) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetUpdatedVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy<Package>(flatPackage);
                }
                else {
                    flatPackage.SetUpdatedVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
            else if (defaultState == PackageState.UpdateAvailable) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetAvailableVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy<Package>(flatPackage);
                }
                else {
                    flatPackage.SetAvailableVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
        }

        protected void PopulateInstalledPackages() {
            foreach (Repository repository in this.LocalInstalledRepositories) {
                foreach (Package package in repository.Packages) {
                    FlatPackedPackage existingFlatPackage = this.GetExistingFlatPackage(repository, package);
                    
                    FlatPackedPackage flatPackage = (FlatPackedPackage)new FlatPackedPackage() {
                        Repository = repository
                    }.Copy<Package>(package);
                    flatPackage.SetInstalledVersion(flatPackage.LatestVersion);

                    if (existingFlatPackage == null) {
                        
                        this.Packages.Add(flatPackage);
                    }
                    else {
                        existingFlatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                        existingFlatPackage.Copy<Package>(flatPackage);
                    }
                }
            }
        }

        protected List<FlatPackedPackage> FlattenRepositories(List<Repository> repositories) {
            List<FlatPackedPackage> flattenedPackages = new List<FlatPackedPackage>();

            foreach (Repository repository in this.LocalInstalledRepositories) {
                foreach (Package package in repository.Packages) {
                }
            }

            return flattenedPackages;
        }

        protected void PopulatePackages(List<Repository> repositories, PackageState defaultState) {
            foreach (Repository repository in repositories) {
                foreach (Package package in repository.Packages) {

                    FlatPackedPackage flatPackage = new FlatPackedPackage() {
                        Repository = repository
                    };
                    flatPackage.Copy<Package>(package);

                    this.AddOrUpdateFlatPackage(repository, package, flatPackage, defaultState);
                }
            }
        }

        protected void BuildFlatPackedPackages() {

            // Add any flat packages that are installed
            this.PopulatePackages(this.LocalInstalledRepositories, PackageState.Installed);

            // Add/update any flat packages that are awaiting restart (updated)
            this.PopulatePackages(this.LocalUpdatedRepositories, PackageState.UpdateInstalled);

            // Add/update any flat packages from the remote repository
            this.PopulatePackages(this.RemoteRepositories, PackageState.UpdateAvailable);

            if (this.PackagesRebuilt != null) {
                this.PackagesRebuilt(this);
            }

        }
    }
}
