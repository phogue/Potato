using System;
using NuGet;
using Procon.Service.Shared.Packages;

namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
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
            if (this.PackageInstalled != null) {
                this.PackageInstalled(this, packageId, "1.0.0");
            }
        }

        public void UninstallPackage(string packageId) {
            if (this.PackageUninstalled != null) {
                this.PackageUninstalled(this, packageId, "1.0.0");
            }
        }
    }
}
