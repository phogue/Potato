#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Service.Shared.Packages;

namespace Potato.Service.Shared.Test.TestServiceController.Mocks {
    public class MockServicePackageManager : IServicePackageManager {
        public IPackageRepository LocalRepository { get; set; }
        public Action BeforeRepositoryInitialize { get; set; }
        public Action BeforeSourcePackageFetch { get; set; }
        public Action BeforeLocalPackageFetch { get; set; }
        public Action<string> PackageActionCanceled { get; set; }
        public Action<string> PackageMissing { get; set; }
        public Action<string, Exception> RepositoryException { get; set; }
        public Action<object, string, string> PackageInstalling { get; set; }
        public Action<object, string, string> PackageInstalled { get; set; }
        public Action<object, string, string> PackageUninstalling { get; set; }
        public Action<object, string, string> PackageUninstalled { get; set; }

        public void MergePackage(string uri, string packageId) {
            if (PackageInstalled != null) {
                PackageInstalled(this, packageId, "1.0.0");
            }
        }

        public void UninstallPackage(string packageId) {
            if (PackageUninstalled != null) {
                PackageUninstalled(this, packageId, "1.0.0");
            }
        }
    }
}
