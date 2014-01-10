using NuGet;

namespace Procon.Service.Shared.Packages {
    /// <summary>
    /// Dispatches simple requests to the manager to complete actions.
    /// </summary>
    public class PackageManagerDispatch : IPackageManagerDispatch {
        public void InstallPackage(IPackageManager manager, IPackage package) {
            manager.InstallPackage(package, false, false);
        }

        public void UpdatePackage(IPackageManager manager, IPackage package) {
            manager.UpdatePackage(package, true, false);
        }

        public void UninstallPackage(IPackageManager manager, IPackage package) {
            manager.UninstallPackage(package, false, true);
        }
    }
}
