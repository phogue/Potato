#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Concurrent;
using System.Linq;
using NuGet;

namespace Potato.Service.Shared.Packages {
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
        public ConcurrentDictionary<string, IPackageRepository> SourceRepositories { get; set; }

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
        public Action<string> PackageActionCanceled { get; set; }

        /// <summary>
        /// Called after fetching source and local and unable to find a package
        /// </summary>
        public Action<string> PackageMissing { get; set; }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        public Action<string, Exception> RepositoryException { get; set; }

        /// <summary>
        /// Called when a package is being installed by a package manager
        /// </summary>
        public Action<object, string, string> PackageInstalling { get; set; }

        /// <summary>
        /// Called when a package has been installed by a package manager
        /// </summary>
        public Action<object, string, string> PackageInstalled { get; set; }

        /// <summary>
        /// Called when a package is being uninstalled by a package manager
        /// </summary>
        public Action<object, string, string> PackageUninstalling { get; set; }

        /// <summary>
        /// Called when a package has been uninstalled by a package manager
        /// </summary>
        public Action<object, string, string> PackageUninstalled { get; set; }

        /// <summary>
        /// Initializes the service package manager with the default values.
        /// </summary>
        public ServicePackageManager() {
            SourceRepositories = new ConcurrentDictionary<string, IPackageRepository>();
            PackageManagerDispatch = new PackageManagerDispatch();
        }

        /// <summary>
        /// Fetches a cached source repository based on the uri
        /// </summary>
        /// <param name="uri">The uri to search for a cached package repository</param>
        /// <returns>The cached or newly cached repository</returns>
        public IPackageRepository GetCachedSourceRepository(string uri) {
            IPackageRepository repository = null;

            if (SourceRepositories.TryGetValue(uri, out repository) == false) {
                repository = PackageRepositoryFactory.Default.CreateRepository(uri);

                SourceRepositories.TryAdd(uri, repository);
            }

            return repository;
        }

        /// <summary>
        /// Called when the repository is initialized, just before 
        /// </summary>
        protected void OnBeforeRepositoryInitialize() {
            var handler = BeforeRepositoryInitialize;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called before the source packages are fetched.
        /// </summary>
        protected void OnBeforeSourcePackageFetch() {
            var handler = BeforeSourcePackageFetch;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called before the source packages are fetched.
        /// </summary>
        protected void OnBeforeLocalPackageFetch() {
            var handler = BeforeLocalPackageFetch;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        protected void OnRepositoryException(string hint, Exception e) {
            var handler = RepositoryException;
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
            var handler = PackageInstalling;
            
            if (handler != null) {
                handler(this, e.Package.Id, e.Package.Version.ToString());
            }
        }

        /// <summary>
        /// Called when a package has been completely installed by a package manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageInstalled(object sender, PackageOperationEventArgs e) {
            var handler = PackageInstalled;
            if (handler != null) {
                handler(this, e.Package.Id, e.Package.Version.ToString());
            }
        }

        /// <summary>
        /// Called when a package is being uninstalled during a package manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageUninstalling(object sender, PackageOperationEventArgs e) {
            var handler = PackageUninstalling;
            if (handler != null) {
                handler(this, e.Package.Id, e.Package.Version.ToString());
            }
        }

        /// <summary>
        /// Called when a package has been completely uninstalled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPackageUninstalled(object sender, PackageOperationEventArgs e) {
            var handler = PackageUninstalled;
            if (handler != null) {
                handler(this, e.Package.Id, e.Package.Version.ToString());
            }
        }

        /// <summary>
        /// Called when the action is canceled
        /// </summary>
        /// <param name="packageId"></param>
        protected void OnPackageActionCanceled(string packageId) {
            var handler = PackageActionCanceled;
            if (handler != null) {
                handler(packageId);
            }
        }

        /// <summary>
        /// Called when a packae with an id cannot be found
        /// </summary>
        /// <param name="packageId"></param>
        protected void OnPackageMissing(string packageId) {
            var handler = PackageMissing;
            if (handler != null) {
                handler(packageId);
            }
        }

        /// <summary>
        /// Attaches the installing/installed/uninstalling/uninstalled events to the manager
        /// </summary>
        /// <param name="manager">The manager to attach events to</param>
        protected void AttachManagerEvents(IPackageManager manager) {
            manager.PackageInstalling += OnPackageInstalling;
            manager.PackageInstalled += OnPackageInstalled;
            manager.PackageUninstalling += OnPackageUninstalling;
            manager.PackageUninstalled += OnPackageUninstalled;
        }

        /// <summary>
        /// Detaches all events from the manager to handlers for this object
        /// </summary>
        /// <param name="manager">The manager to detach events from</param>
        protected void DetachManagerEvents(IPackageManager manager) {
            manager.PackageInstalling -= OnPackageInstalling;
            manager.PackageInstalled -= OnPackageInstalled;
            manager.PackageUninstalling -= OnPackageUninstalling;
            manager.PackageUninstalled -= OnPackageUninstalled;
        }

        /// <summary>
        /// Installs or updates a package given a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        public void MergePackage(string uri, string packageId) {
            OnBeforeRepositoryInitialize(); 

            try {
                var manager = new PackageManager(GetCachedSourceRepository(uri), new DefaultPackagePathResolver(LocalRepository.Source), new PhysicalFileSystem(LocalRepository.Source), LocalRepository);

                AttachManagerEvents(manager);

                OnBeforeSourcePackageFetch();

                var latest = manager.SourceRepository.GetPackages().Where(package => package.Id == packageId && package.IsLatestVersion == true).ToList();

                OnBeforeLocalPackageFetch();

                var installed = manager.LocalRepository.GetPackages().Where(package => package.Id == packageId).ToList();

                if (latest.Any() == true) {
                    if (installed.Any() == true) {
                        if (installed.First().Version.CompareTo(latest.First().Version) < 0) {
                            try {
                                PackageManagerDispatch.UpdatePackage(manager, latest.First());
                            }
                            catch (Exception e) {
                                OnRepositoryException("ServicePackages.MergePackage.UpdatePackage", e);
                            }
                        }
                        else {
                            OnPackageActionCanceled(packageId);
                        }
                    }
                    else {
                        try {
                            PackageManagerDispatch.InstallPackage(manager, latest.First());
                        }
                        catch (Exception e) {
                            OnRepositoryException("ServicePackages.MergePackage.InstallPackage", e);
                        }
                    }
                }
                else {
                    OnPackageMissing(packageId);
                }

                DetachManagerEvents(manager);
            }
            catch (Exception e) {
                OnRepositoryException("ServicePackages.MergePackage.GeneralCatch", e);
            }
        }

        /// <summary>
        /// Uninstalls a package 
        /// </summary>
        /// <param name="packageId"></param>
        public void UninstallPackage(string packageId) {
            OnBeforeRepositoryInitialize();

            try {
                var manager = new PackageManager(LocalRepository, new DefaultPackagePathResolver(LocalRepository.Source), new PhysicalFileSystem(LocalRepository.Source), LocalRepository);

                AttachManagerEvents(manager);

                OnBeforeLocalPackageFetch();

                var installed = manager.LocalRepository.GetPackages()
                    .Where(package => package.Id == packageId)
                    .ToList();

                if (installed.Any() == true) {
                    try {
                        PackageManagerDispatch.UninstallPackage(manager, installed.First());
                    }
                    catch (Exception e) {
                        OnRepositoryException("ServicePackages.UninstallPackage.UninstallPackage", e);
                    }
                }
                else {
                    OnPackageMissing(packageId);
                }

                DetachManagerEvents(manager);
            }
            catch (Exception e) {
                OnRepositoryException("ServicePackages.UninstallPackage.GeneralCatch", e);
            }
        }
    }
}
