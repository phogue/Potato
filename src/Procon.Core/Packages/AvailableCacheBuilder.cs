using System.Collections.Generic;
using System.Linq;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    /// <summary>
    /// Grabs packages from source repositories
    /// </summary>
    public class AvailableCacheBuilder : ICacheBuilder {
        public RepositoryModel Repository { get; set; }

        public IList<IPackage> Packages { get; set; }

        public void Build() {
            foreach (var availablePackage in this.Packages) {
                PackageWrapperModel packageWrapper = this.Repository.Packages.FirstOrDefault(pack => pack.Id == availablePackage.Id);

                if (packageWrapper == null) {
                    this.Repository.Packages.Add(new PackageWrapperModel() {
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
