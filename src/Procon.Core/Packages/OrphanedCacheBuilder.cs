using System.Collections.Generic;
using System.Linq;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    /// <summary>
    /// Similar to the installed cache builder, but regardless of previously knowing the package or not
    /// this builder will assume the repository wants to take charge of the packages.
    /// </summary>
    public class OrphanedCacheBuilder : ICacheBuilder {
        public IList<PackageWrapperModel> Cache { get; set; }

        public IList<IPackage> Source { get; set; }

        public void Build() {
            foreach (var orphanedPackage in this.Source) {
                PackageWrapperModel packageWrapper = this.Cache.FirstOrDefault(pack => pack.Id == orphanedPackage.Id);

                if (packageWrapper == null) {
                    this.Cache.Add(new PackageWrapperModel() {
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
