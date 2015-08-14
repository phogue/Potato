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
using NuGet;

namespace Potato.Service.Shared.Packages {
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
        Action<string> PackageActionCanceled { get; set; }

        /// <summary>
        /// Called after fetching source and local and unable to find a package
        /// </summary>
        Action<string> PackageMissing { get; set; }

        /// <summary>
        /// Called when an exception occurs during a package operation
        /// </summary>
        Action<string, Exception> RepositoryException { get; set; }

        /// <summary>
        /// Called when a package is being installed by a package manager
        /// </summary>
        Action<object, string, string> PackageInstalling { get; set; }

        /// <summary>
        /// Called when a package has been installed by a package manager
        /// </summary>
        Action<object, string, string> PackageInstalled { get; set; }

        /// <summary>
        /// Called when a package is being uninstalled by a package manager
        /// </summary>
        Action<object, string, string> PackageUninstalling { get; set; }

        /// <summary>
        /// Called when a package has been uninstalled by a package manager
        /// </summary>
        Action<object, string, string> PackageUninstalled { get; set; }

        /// <summary>
        /// Installs or updates a package given a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        void MergePackage(string uri, string packageId);

        /// <summary>
        /// Uninstalls a package 
        /// </summary>
        /// <param name="packageId"></param>
        void UninstallPackage(string packageId);
    }
}
