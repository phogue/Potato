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
using System.Collections.Generic;
using System.Linq;
using NuGet;
using Potato.Core.Shared.Models;

namespace Potato.Core.Packages {
    /// <summary>
    /// Similar to the installed cache builder, but regardless of previously knowing the package or not
    /// this builder will assume the repository wants to take charge of the packages.
    /// </summary>
    public class OrphanedCacheBuilder : ICacheBuilder {
        public IList<PackageWrapperModel> Cache { get; set; }

        public IList<IPackage> Source { get; set; }

        public void Build() {
            foreach (var orphanedPackage in Source) {
                var packageWrapper = Cache.FirstOrDefault(pack => pack.Id == orphanedPackage.Id);

                if (packageWrapper == null) {
                    Cache.Add(new PackageWrapperModel() {
                        State = PackageState.Installed,
                        Installed = PackageFactory.CreatePackageModelFromNugetPackage(orphanedPackage)
                    });
                }
                else {
                    packageWrapper.Installed = PackageFactory.CreatePackageModelFromNugetPackage(orphanedPackage);
                    packageWrapper.State = PackageState.Installed;
                }
            }
        }
    }
}
