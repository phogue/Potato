using System;
using System.Collections.Concurrent;
using System.Linq;
using NuGet;

namespace Procon.Service.Shared.Packages {
    /// <summary>
    /// Manages installing/uninstalling/updating packages for the service.
    /// </summary>
    public class ServicePackageManager : IServicePackageManager {
        /// <summary>
        /// The controller to dispatch manager requests to.
        /// </summary>
        public IPackageManagerDispatch PackageManagerDispatch { get; set; }

        /// <summary>
        /// Holds a reference to the local repository of packages.
        /// </summary>
        public IPackageRepository LocalRepository { get; set; }

        /// <summary>
        /// A dictionary of source repositories from the list of repositories. Used as a cache.
        /// </summary>
        public ConcurrentDictionary<String, IPackageRepository> SourceRepositories { get; set; }

        /// <summary>
        /// Called when the repository is initialized
        /// </summary>
        public Action BeforeRepositoryInitialize { get; set; }

        /// <summary>
        /// Called before the packages from the source repository are fetched.
        /// </summary>
        public Action BeforeSourcePackageFetch { get; set; }

        /// <summary>
        /// Called before the packages from the local repository are fetched.
        /// </summary>
        public Action BeforeLocalPackageFetch { get; set; }

        /// <summary>
        /// Called after packages have been fetched, checks have been completed and the
        /// action (installing/uninstalling) is 
        /// </summary>
        public Action<String> PackageActionCanceled { get; set; }

        /// <summary>
        /// Called after fetching source and local and unable to find a package
        /// </summary>
        public Action<String> PackageMissing { get; set; }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        public Action<String, Exception> RepositoryException { get; set; }

        /// <summary>
        /// Called when a package is being installed by a package manager
        /// </summary>
        public Action<Object, PackageOperationEventArgs> PackageInstalling { get; set; }

        /// <summary>
        /// Called when a package has been installed by a package manager
        /// </summary>
        public Action<Object, PackageOperationEventArgs> PackageInstalled { get; set; }

        /// <summary>
        /// Called when a package is being uninstalled by a package manager
        /// </summary>
        public Action<Object, PackageOperationEventArgs> PackageUninstalling { get; set; }

        /// <summary>
        /// Called when a package has been uninstalled by a package manager
        /// </summary>
        public Action<Object, PackageOperationEventArgs> PackageUninstalled { get; set; }

        /// <summary>
        /// Initializes the service package manager with the default values.
        /// </summary>
        public ServicePackageManager() {
            this.SourceRepositories = new ConcurrentDictionary<String, IPackageRepository>();
            this.PackageManagerDispatch = new PackageManagerDispatch();
        }

        /// <summary>
        /// Fetches a cached source repository based on the uri
        /// </summary>
        /// <param name="uri">The uri to search for a cached package repository</param>
        /// <returns>The cached or newly cached repository</returns>
        public IPackageRepository GetCachedSourceRepository(String uri) {
            IPackageRepository repository = null;

            if (this.SourceRepositories.TryGetValue(uri, out repository) == false) {
                repository = PackageRepositoryFactory.Default.CreateRepository(uri);

                this.SourceRepositories.TryAdd(uri, repository);
            }

            return repository;
        }

        /// <summary>
        /// Called when the repository is initialized, just before 
        /// </summary>
        protected void OnBeforeRepositoryInitialize() {
            var handler = this.BeforeRepositoryInitialize;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called before the source packages are fetched.
        /// </summary>
        protected void OnBeforeSourcePackageFetch() {
            var handler = this.BeforeSourcePackageFetch;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called before the source packages are fetched.
        /// </summary>
        protected void OnBeforeLocalPackageFetch() {
            var handler = this.BeforeLocalPackageFetch;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        protected void OnRepositoryException(String hint, Exception e) {
            var handler = this.RepositoryException;
            if (handler != null) {
                handler(hint, e);
            }
        }
        
        /// <summary>
        /// Called when a package is being installed by a manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageInstalling(object sender, PackageOperationEventArgs e) {
            var handler = this.PackageInstalling;
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when a package has been completely installed by a package manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageInstalled(object sender, PackageOperationEventArgs e) {
            var handler = this.PackageInstalled;
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when a package is being uninstalled during a package manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageUninstalling(object sender, PackageOperationEventArgs e) {
            var handler = this.PackageUninstalling;
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when a package has been completely uninstalled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageUninstalled(object sender, PackageOperationEventArgs e) {
            var handler = this.PackageUninstalled;
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when the action is canceled
        /// </summary>
        /// <param name="packageId"></param>
        protected void OnPackageActionCanceled(String packageId) {
            var handler = this.PackageActionCanceled;
            if (handler != null) {
                handler(packageId);
            }
        }

        /// <summary>
        /// Called when a packae with an id cannot be found
        /// </summary>
        /// <param name="packageId"></param>
        protected void OnPackageMissing(String packageId) {
            var handler = this.PackageMissing;
            if (handler != null) {
                handler(packageId);
            }
        }

        /// <summary>
        /// Attaches the installing/installed/uninstalling/uninstalled events to the manager
        /// </summary>
        /// <param name="manager">The manager to attach events to</param>
        protected void AttachManagerEvents(IPackageManager manager) {
            manager.PackageInstalling += this.OnPackageInstalling;
            manager.PackageInstalled += this.OnPackageInstalled;
            manager.PackageUninstalling += this.OnPackageUninstalling;
            manager.PackageUninstalled += this.OnPackageUninstalled;
        }

        /// <summary>
        /// Detaches all events from the manager to handlers for this object
        /// </summary>
        /// <param name="manager">The manager to detach events from</param>
        protected void DetachManagerEvents(IPackageManager manager) {
            manager.PackageInstalling -= this.OnPackageInstalling;
            manager.PackageInstalled -= this.OnPackageInstalled;
            manager.PackageUninstalling -= this.OnPackageUninstalling;
            manager.PackageUninstalled -= this.OnPackageUninstalled;
        }

        /// <summary>
        /// Installs or updates a package given a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        public void MergePackage(String uri, String packageId) {
            this.OnBeforeRepositoryInitialize(); 

            try {
                var manager = new PackageManager(this.GetCachedSourceRepository(uri), new DefaultPackagePathResolver(this.LocalRepository.Source), new PhysicalFileSystem(this.LocalRepository.Source), this.LocalRepository);

                this.AttachManagerEvents(manager);

                this.OnBeforeSourcePackageFetch();

                var latest = manager.SourceRepository.GetPackages()
                    .Where(package => package.Id == packageId)
                    .OrderByDescending(package => package.Version)
                    .Take(1)
                    .ToList();

                this.OnBeforeLocalPackageFetch();

                var installed = manager.LocalRepository.GetPackages().Where(package => package.Id == packageId).ToList();

                if (latest.Any() == true) {
                    if (installed.Any() == true) {
                        if (installed.First().Version.CompareTo(latest.First().Version) < 0) {
                            try {
                                this.PackageManagerDispatch.UpdatePackage(manager, latest.First());
                            }
                            catch (Exception e) {
                                this.OnRepositoryException("ServicePackages.MergePackage.UpdatePackage", e);
                            }
                        }
                        else {
                            this.OnPackageActionCanceled(packageId);
                        }
                    }
                    else {
                        try {
                            this.PackageManagerDispatch.InstallPackage(manager, latest.First());
                        }
                        catch (Exception e) {
                            this.OnRepositoryException("ServicePackages.MergePackage.InstallPackage", e);
                        }
                    }
                }
                else {
                    this.OnPackageMissing(packageId);
                }

                this.DetachManagerEvents(manager);
            }
            catch (Exception e) {
                this.OnRepositoryException("ServicePackages.MergePackage.GeneralCatch", e);
            }
        }

        /// <summary>
        /// Uninstalls a package 
        /// </summary>
        /// <param name="packageId"></param>
        public void UninstallPackage(String packageId) {
            this.OnBeforeRepositoryInitialize();

            try {
                var manager = new PackageManager(this.LocalRepository, new DefaultPackagePathResolver(this.LocalRepository.Source), new PhysicalFileSystem(this.LocalRepository.Source), this.LocalRepository);

                this.AttachManagerEvents(manager);

                this.OnBeforeLocalPackageFetch();

                var installed = manager.LocalRepository.GetPackages()
                    .Where(package => package.Id == packageId)
                    .ToList();

                if (installed.Any() == true) {
                    try {
                        this.PackageManagerDispatch.UninstallPackage(manager, installed.First());
                    }
                    catch (Exception e) {
                        this.OnRepositoryException("ServicePackages.UninstallPackage.UninstallPackage", e);
                    }
                }
                else {
                    this.OnPackageMissing(packageId);
                }

                this.DetachManagerEvents(manager);
            }
            catch (Exception e) {
                this.OnRepositoryException("ServicePackages.UninstallPackage.GeneralCatch", e);
            }
        }
    }
}
