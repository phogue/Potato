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
        public RepositoryModel Repository { get; set; }

        public IList<IPackage> Packages { get; set; }

        public void Build() {
            foreach (var orphanedPackage in this.Packages) {
                PackageWrapperModel packageWrapper = this.Repository.Packages.FirstOrDefault(pack => pack.Id == orphanedPackage.Id);

                if (packageWrapper == null) {
                    this.Repository.Packages.Add(new PackageWrapperModel() {
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
