using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace Procon.Service.Shared {
    /// <summary>
    /// A series of static methods to use within the ServiceController, but it 
    /// neatens the code up a little having some of the functionality here.
    /// </summary>
    public static class ServiceControllerHelpers {

        /// <summary>
        /// Self enclosed exception log, opens a file, writes the exception and flushes/closes the file.
        /// </summary>
        /// <param name="hint">A hint for where the exception occured</param>
        /// <param name="e">The exception to log</param>
        public static void LogUnhandledException(String hint, Exception e) {
            Directory.CreateDirectory(Defines.ErrorsLogsDirectory);

            var lines = new List<String>() {
                String.Format("Hint: {0}", hint),
                String.Format("Exception: {0}", e)
            };

            File.WriteAllLines(Path.Combine(Defines.ErrorsLogsDirectory, DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss-fffffff")), lines);

            lines.ForEach(line => Console.WriteLine("Error: {0}", line));
        }

        /// <summary>
        /// Installs or updates a package given a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        public static void InstallOrUpdatePackage(String uri, String packageId) {
            Console.WriteLine("Initializing package repository..");

            try {
                var repository = PackageRepositoryFactory.Default.CreateRepository(uri);

                var manager = new PackageManager(repository, Defines.PackagesDirectory);

                manager.PackageInstalling += (sender, args) => Console.WriteLine("Installing {0} version {1}..", args.Package.Id, args.Package.Version);
                manager.PackageInstalled += (sender, args) => Console.WriteLine("Installed {0} version {1}", args.Package.Id, args.Package.Version);
                manager.PackageUninstalling += (sender, args) => Console.WriteLine("Uninstalling {0} version {1}..", args.Package.Id, args.Package.Version);
                manager.PackageUninstalled += (sender, args) => Console.WriteLine("Uninstalled {0} version {1}..", args.Package.Id, args.Package.Version);

                Console.WriteLine("Checking source repositories..");

                var latest = manager.SourceRepository.GetPackages()
                    .Where(package => package.Id == packageId)
                    .OrderByDescending(package => package.Version)
                    // todo uncomment. It's commented so we initially install the first Myrcon.Procon.Core to then test updating.
                    //.Take(1)
                    .ToList();

                Console.WriteLine("Checking local repositories..");

                var installed = manager.LocalRepository.GetPackages()
                    .Where(package => package.Id == packageId)
                    .ToList();

                if (latest.Any() == true) {
                    if (installed.Any() == true) {
                        if (installed.First().Version.CompareTo(latest.First().Version) < 0) {
                            Console.WriteLine("Initiating update {0}..", packageId);

                            try {
                                manager.UpdatePackage(latest.First(), true, false);
                            }
                            catch (UnauthorizedAccessException e) {
                                Console.WriteLine("Unable to access path {0}", Defines.PackagesDirectory);
                                Console.WriteLine("Ensure all applications and open folders using the packages folder are closed and try again.");

                                ServiceControllerHelpers.LogUnhandledException("ServiceController.InstallOrUpdatePackage.UpdatePackage", e);
                            }
                        }
                        else {
                            Console.WriteLine("Package {0} is up to date.", packageId);
                        }
                    }
                    else {
                        try {
                            // todo changed to .First() though it wouldn't make much difference really.
                            manager.InstallPackage(latest.Last(), true, false);
                        }
                        catch (UnauthorizedAccessException e) {
                            Console.WriteLine("Unable to access path {0}", Defines.PackagesDirectory);
                            Console.WriteLine("Ensure all applications and open folders using the packages folder are closed and try again.");

                            ServiceControllerHelpers.LogUnhandledException("ServiceController.InstallOrUpdatePackage.InstallPackage", e);
                        }
                    }
                }
                else {
                    Console.WriteLine("Couldn't find package {0}.", packageId);
                }
            }
            catch (Exception e) {
                ServiceControllerHelpers.LogUnhandledException("ServiceController.InstallOrUpdatePackage.GeneralCatch", e);
            }
        }

        /// <summary>
        /// Uninstalls a package 
        /// </summary>
        /// <param name="packageId"></param>
        public static void UninstallPackage(String packageId) {
            Console.WriteLine("Initializing package repository..");

            var repository = PackageRepositoryFactory.Default.CreateRepository(Defines.PackagesDirectory);

            var manager = new PackageManager(repository, Defines.PackagesDirectory);

            manager.PackageUninstalling += (sender, args) => Console.WriteLine("Uninstalling {0} version {1}..", args.Package.Id, args.Package.Version);
            manager.PackageUninstalled += (sender, args) => Console.WriteLine("Uninstalled {0} version {1}..", args.Package.Id, args.Package.Version);

            Console.WriteLine("Checking local repositories..");

            var installed = manager.LocalRepository.GetPackages()
                .Where(package => package.Id == packageId)
                .ToList();

            if (installed.Any() == true) {
                try {
                    manager.UninstallPackage(installed.First(), false, true);
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine("Unable to access path {0}", Defines.PackagesDirectory);
                    Console.WriteLine("Ensure all applications and open folders using the packages folder are closed and try again.");

                    ServiceControllerHelpers.LogUnhandledException("ServiceController.InstallOrUpdatePackage.UpdatePackage", e);
                }
            }
            else {
                Console.WriteLine("Couldn't find package {0}.", packageId);
            }
        }
    }
}
