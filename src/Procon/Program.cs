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
                Settings = new ServiceSettings(new List<String>(args)),
                Packages = {
                    BeforeRepositoryInitialize = () => Console.WriteLine(@"Initializing package repository.."),
                    BeforeSourcePackageFetch = () => Console.WriteLine(@"Checking source repositories.."),
                    BeforeLocalPackageFetch = () => Console.WriteLine(@"Checking local repository.."),
                    PackageInstalling = (sender, packageId, version) => Console.WriteLine(@"Installing {0} version {1}..", packageId, version),
                    PackageInstalled = (sender, packageId, version) => Console.WriteLine(@"Installed {0} version {1}", packageId, version),
                    PackageUninstalling = (sender, packageId, version) => Console.WriteLine(@"Uninstalling {0} version {1}..", packageId, version),
                    PackageUninstalled = (sender, packageId, version) => Console.WriteLine(@"Uninstalled {0} version {1}..", packageId, version),
                    PackageMissing = packageId => Console.WriteLine(@"Couldn't find package {0}.", packageId),
                    PackageActionCanceled = packageId => Console.WriteLine(@"Package {0} is up to date.", packageId),
                    RepositoryException = (hint, exception) => {
                        if (exception is UnauthorizedAccessException) {
                            Console.WriteLine(@"Unable to access path {0}", Defines.PackagesDirectory);
                            Console.WriteLine(@"Ensure all applications and open folders using the packages folder are closed and try again.");
                        }

                        ServiceControllerHelpers.LogUnhandledException(hint, exception);
                    }
                },
                SignalBegin = (controller, message) => Console.WriteLine(@"Signal: {0}", message.Name),
                SignalEnd = (controller, message, seconds) => Console.WriteLine(@"Signal: {0} completed in {1} seconds", message.Name, seconds),
                SignalParameterError = (controller, list) => Console.WriteLine(@"Missing or valid parameters: {0}", String.Join(", ", list)),
                SignalStatistics = (controller, domain) => {
                    Console.WriteLine(@"Service Controller");
                    Console.WriteLine(@"+--------------------------------------------------------+");
                    Console.WriteLine(@"MonitoringSurvivedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringSurvivedMemorySize / 1024);
                    Console.WriteLine(@"MonitoringTotalAllocatedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024);
                    Console.WriteLine(@"MonitoringTotalProcessorTime: {0}", AppDomain.CurrentDomain.MonitoringTotalProcessorTime);

                    Console.WriteLine("");

                    Console.WriteLine(@"Service Domain");
                    Console.WriteLine(@"+--------------------------------------------------------+");
                    Console.WriteLine(@"MonitoringSurvivedMemorySize: {0:N0} K", domain.MonitoringSurvivedMemorySize / 1024);
                    Console.WriteLine(@"MonitoringTotalAllocatedMemorySize: {0:N0} K", domain.MonitoringTotalAllocatedMemorySize / 1024);
                    Console.WriteLine(@"MonitoringTotalProcessorTime: {0}", domain.MonitoringTotalProcessorTime);
                }
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