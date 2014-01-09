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
