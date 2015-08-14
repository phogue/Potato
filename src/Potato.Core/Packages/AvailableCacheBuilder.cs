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
using System.Collections.Generic;
using System.Linq;
using NuGet;
using Potato.Core.Shared.Models;

namespace Potato.Core.Packages {
    /// <summary>
    /// Grabs packages from source repositories
    /// </summary>
    public class AvailableCacheBuilder : ICacheBuilder {
        public IList<PackageWrapperModel> Cache { get; set; }

        public IList<IPackage> Source { get; set; }

        public void Build() {
            foreach (var availablePackage in Source) {
                var packageWrapper = Cache.FirstOrDefault(pack => pack.Id == availablePackage.Id);

                if (packageWrapper == null) {
                    Cache.Add(new PackageWrapperModel() {
                        State = PackageState.NotInstalled,
                        Available = PackageFactory.CreatePackageModelFromNugetPackage(availablePackage)
                    });
                }
                else {
                    // Note that we preserve the state in its current form because we don't know here if an update is available.
                    packageWrapper.State = availablePackage.Version.CompareTo(new SemanticVersion(packageWrapper.Installed.Version)) > 0 ? PackageState.UpdateAvailable : PackageState.Installed;

                    packageWrapper.Available = PackageFactory.CreatePackageModelFromNugetPackage(availablePackage);
                }
            }
        }
    }
}
