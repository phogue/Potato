using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    internal class Program {
        
        public static List<String> Wordify(String data) {
            List<String> list = new List<String>();

            String word = String.Empty;
            int stack = 0;
            bool escaped = false;

            foreach (char input in data) {

                if (input == ' ') {
                    if (stack == 0) {
                        list.Add(word);
                        word = String.Empty;
                    }
                    else {
                        word += ' ';
                    }
                }
                else if (input == 'n' && escaped == true) {
                    word += '\n';
                    escaped = false;
                }
                else if (input == 'r' && escaped == true) {
                    word += '\r';
                    escaped = false;
                }
                else if (input == 't' && escaped == true) {
                    word += '\t';
                    escaped = false;
                }
                else if (input == '"') {
                    if (escaped == false) {
                        if (stack == 0) {
                            stack++;
                        }
                        else {
                            stack--;
                        }
                    }
                    else {
                        word += '"';
                    }
                }
                else if (input == '\\') {
                    if (escaped == true) {
                        word += '\\';
                        escaped = false;
                    }
                    else {
                        escaped = true;
                    }
                }
                else {
                    word += input;
                    escaped = false;
                }
            }

            list.Add(word);

            return list;
        }
        
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
                },
                SignalHelp = controller => Console.WriteLine(Resources.ConsoleHelp),
                WriteServiceConfigBegin = controller => Console.Write(@"Writing config.. "),
                WriteServiceConfigEnd = controller => Console.WriteLine(@"Complete"),
                DisposeServiceBegin = controller => Console.Write(@"Disposing service.. "),
                DisposeServiceEnd = controller => Console.WriteLine(@"Complete"),
                UnloadServiceBegin = controller => Console.Write(@"Unloading service domain.. "),
                UnloadServiceEnd = controller => Console.WriteLine(@"Complete"),
                Observer = {
                    StatusChange = (observer, type) => Console.WriteLine(@"Status: {0}", type.ToString())
                }
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            service.SignalMessage(new ServiceMessage() {
                Name = "start"
            });

            AutoResetEvent exitWait = new AutoResetEvent(false);

            Task.Factory.StartNew(() => {
                var input = String.Empty;

                do {
                    input = Console.ReadLine();

                    if (input != null) {
                        var words = Program.Wordify(input);

                        service.SignalMessage(new ServiceMessage() {
                            Name = words.FirstOrDefault(),
                            Arguments = ArgumentHelper.ToArguments(words.Skip(1).ToList())
                        });
                    }

                } while (String.Compare(input, "exit", StringComparison.OrdinalIgnoreCase) != 0);

                exitWait.Set();
            });

            // Wait until exit() is called.
            exitWait.WaitOne();

            service.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });

            service.Dispose();

            System.Console.WriteLine(@"Closing..");
            Thread.Sleep(1000);
        }
    }
}