using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    internal class Program {

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main(string[] args) {

            Console.WriteLine(Resources.ConsoleHeader);
            
            if (args.Length > 0) {
                // Support for --help command?

                System.Console.WriteLine(@"Starting with arguments: {0}", String.Join(" ", args));
            }

            ServiceController service = new ServiceController {
                Arguments = new List<String>(args),
                Settings = new ServiceSettings(new List<String>(args))
            };

            service.Packages.BeforeRepositoryInitialize = () => Console.WriteLine(@"Initializing package repository..");
            service.Packages.BeforeSourcePackageFetch = () => Console.WriteLine(@"Checking source repositories..");
            service.Packages.BeforeLocalPackageFetch = () => Console.WriteLine(@"Checking local repository..");
            service.Packages.PackageInstalling = (sender, packageId, version) => Console.WriteLine(@"Installing {0} version {1}..", packageId, version);
            service.Packages.PackageInstalled = (sender, packageId, version) => Console.WriteLine(@"Installed {0} version {1}", packageId, version);
            service.Packages.PackageUninstalling = (sender, packageId, version) => Console.WriteLine(@"Uninstalling {0} version {1}..", packageId, version);
            service.Packages.PackageUninstalled = (sender, packageId, version) => Console.WriteLine(@"Uninstalled {0} version {1}..", packageId, version);
            service.Packages.PackageMissing = packageId => Console.WriteLine(@"Couldn't find package {0}.", packageId);
            service.Packages.PackageActionCanceled = packageId => Console.WriteLine(@"Package {0} is up to date.", packageId);
            service.Packages.RepositoryException = (hint, exception) => {
                if (exception is UnauthorizedAccessException) {
                    Console.WriteLine(@"Unable to access path {0}", Defines.PackagesDirectory);
                    Console.WriteLine(@"Ensure all applications and open folders using the packages folder are closed and try again.");
                }

                ServiceControllerHelpers.LogUnhandledException(hint, exception);
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            service.SignalMessage(new ServiceMessage() {
                Name = "start"
            });

            var input = String.Empty;

            do {
                input = Console.ReadLine();
                
                if (input != null) {
                    var words = Regex.Matches(input, @"([^\s]*""[^""]+""[^\s]*)|[^""]?\w+[^""]?")
                        .Cast<Match>()
                        .Select(match => match.Captures[0].Value.Trim('"', ' '))
                        .ToList();

                    service.SignalMessage(new ServiceMessage() {
                        Name = words.FirstOrDefault(),
                        Arguments = ArgumentHelper.ToArguments(words.Skip(1).ToList())
                    });
                }

            } while (String.Compare(input, "exit", StringComparison.OrdinalIgnoreCase) != 0);

            service.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });

            service.Dispose();

            System.Console.WriteLine(@"Closing..");
            Thread.Sleep(1000);
        }
    }
}