using System;
using NuGet;

namespace Procon.Service.Shared.Packages {
    /// <summary>
    /// Manages installing/uninstalling/updating packages for the service.
    /// </summary>
    public interface IServicePackageManager {
        /// <summary>
        /// Holds a reference to the local repository of packages.
        /// </summary>
        IPackageRepository LocalRepository { get; set; }

        /// <summary>
        /// Called when the repository is initialized
        /// </summary>
        Action BeforeRepositoryInitialize { get; set; }

        /// <summary>
        /// Called before the packages from the source repository are fetched.
        /// </summary>
        Action BeforeSourcePackageFetch { get; set; }

        /// <summary>
        /// Called before the packages from the local repository are fetched.
        /// </summary>
        Action BeforeLocalPackageFetch { get; set; }

        /// <summary>
        /// Called after packages have been fetched, checks have been completed and the
        /// action (installing/uninstalling) is 
        /// </summary>
        Action<String> PackageActionCanceled { get; set; }

        /// <summary>
        /// Called after fetching source and local and unable to find a package
        /// </summary>
        Action<String> PackageMissing { get; set; }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        Action<String, Exception> RepositoryException { get; set; }

        /// <summary>
        /// Called when a package is being installed by a package manager
        /// </summary>
        Action<Object, String, String> PackageInstalling { get; set; }

        /// <summary>
        /// Called when a package has been installed by a package manager
        /// </summary>
        Action<Object, String, String> PackageInstalled { get; set; }

        /// <summary>
        /// Called when a package is being uninstalled by a package manager
        /// </summary>
        Action<Object, String, String> PackageUninstalling { get; set; }

        /// <summary>
        /// Called when a package has been uninstalled by a package manager
        /// </summary>
        Action<Object, String, String> PackageUninstalled { get; set; }

        /// <summary>
        /// Installs or updates a package given a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        void MergePackage(String uri, String packageId);

        /// <summary>
        /// Uninstalls a package 
        /// </summary>
        /// <param name="packageId"></param>
        void UninstallPackage(String packageId);
    }
}
