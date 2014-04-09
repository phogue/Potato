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
using NuGet;

namespace Procon.Service.Shared.Packages {
    /// <summary>
    /// Dispatches requests on a package manager
    /// </summary>
    public interface IPackageManagerDispatch {
        /// <summary>
        /// Dispatches an install request on a package manager to install the passed in package
        /// </summary>
        /// <param name="manager">The build package manager with the source/local queried, checked and completed</param>
        /// <param name="package">The package to install</param>
        void InstallPackage(IPackageManager manager, IPackage package);

        /// <summary>
        /// Dispatches an update request on a package manager to install the passed in package
        /// </summary>
        /// <param name="manager">The build package manager with the source/local queried, checked and completed</param>
        /// <param name="package">The package to install</param>
        void UpdatePackage(IPackageManager manager, IPackage package);

        /// <summary>
        /// Dispatches an uninstall request on a package manager to install the passed in package
        /// </summary>
        /// <param name="manager">The build package manager with the source/local queried, checked and completed</param>
        /// <param name="package">The package to install</param>
        void UninstallPackage(IPackageManager manager, IPackage package);
    }
}
