using NuGet;
using Procon.Service.Shared.Packages;

namespace Procon.Service.Shared.Test.TestServicePackages.Mocks {
    public class MockPackageManagerDispatch : IPackageManagerDispatch {
        public bool DispatchedInstallPackage { get; set; }
        public bool DispatchedUpdatePackage { get; set; }
        public bool DispatchedUninstallPackage { get; set; }

        public MockPackageManagerDispatch() {
            this.DispatchedInstallPackage = false;
            this.DispatchedUpdatePackage = false;
            this.DispatchedUninstallPackage = false;
        }

        public void InstallPackage(IPackageManager manager, IPackage package) {
            this.DispatchedInstallPackage = true;
        }

        public void UpdatePackage(IPackageManager manager, IPackage package) {
            this.DispatchedUpdatePackage = true;
        }

        public void UninstallPackage(IPackageManager manager, IPackage package) {
            this.DispatchedUninstallPackage = true;
        }
    }
}
