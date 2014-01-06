using System;
using System.Collections.Generic;
using System.Threading;

namespace Procon.Service.Shared {
    /// <summary>
    /// Manages an instance of Procon in a seperate AppDomain.
    /// </summary>
    public sealed class ServiceController : IDisposable {

        /// <summary>
        /// The domain to load Procon.Core into.
        /// </summary>
        private AppDomain ServiceDomain { get; set; }

        /// <summary>
        /// The loader proxy to then load the procon core instance.
        /// </summary>
        private ServiceLoaderProxy ServiceLoaderProxy { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public ServiceStatusType Status { get; set; }

        /// <summary>
        /// The arguments to start any instances with.
        /// </summary>
        public List<String> Arguments { get; set; }

        /// <summary>
        /// The processed arguments to check/use any service side conditions
        /// </summary>
        public IServiceSettings Settings { get; set; } 

        /// <summary>
        /// Polling handler to ensure the appdomain is still functional.
        /// </summary>
        public Timer PollingTask { get; set; }

        /// <summary>
        /// Initiates the service controller with the default values
        /// </summary>
        public ServiceController() {
            this.Status = ServiceStatusType.Stopped;

            this.PollingTask = new Timer(PollingTask_Tick, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(10));

            this.Arguments = new List<String>();
            this.Settings = new ServiceSettings();
        }

        /// <summary>
        /// Fired every ten seconds to ensure the appdomain is still responding and does not have
        /// any additional messages for us to process.
        /// </summary>
        private void PollingTask_Tick(object state) {
            if (this.Status == ServiceStatusType.Started && this.ServiceLoaderProxy != null) {
                AutoResetEvent pollingTimeoutEvent = new AutoResetEvent(false);
                ServiceMessage message = null;

                ThreadPool.QueueUserWorkItem(delegate {
                    message = this.ServiceLoaderProxy.PollService();

                    pollingTimeoutEvent.Set();
                });

                // If we don't get a response or the response wasn't processed properly.
                if (pollingTimeoutEvent.WaitOne(5000) == false || this.SignalMessage(message) == false) {
                    this.Restart();
                }
            }
        }

        /// <summary>
        /// Parse a basic message to the service controller
        /// </summary>
        /// <param name="message">The message to dispatch</param>
        /// <returns>True if the message was processed correctly, false otherwise</returns>
        public bool SignalMessage(ServiceMessage message) {
            bool processed = true;

            if (message != null) {
                // Record the current time for statistics output.
                DateTime begin = DateTime.Now;

                if (String.Compare(message.Name, "start", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Start");
                    this.Start();
                    Console.WriteLine("Signal: Start completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "stop", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Stop");
                    this.Stop();
                    Console.WriteLine("Signal: Stop completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "restart", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Restart");
                    this.Restart();
                    Console.WriteLine("Signal: Restart completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "merge-package", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Merge Package");

                    if (message.Arguments.ContainsKey("uri") == true && message.Arguments.ContainsKey("packageid") == true) {
                        this.MergePackage(message.Arguments["uri"], message.Arguments["packageid"]);
                    }
                    else {
                        Console.WriteLine("Missing argument uri or packageId");
                    }

                    Console.WriteLine("Signal: Merge Package completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "uninstall-package", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Uninstall Package");

                    if (message.Arguments.ContainsKey("packageid") == true) {
                        this.UninstallPackage(message.Arguments["packageid"]);
                    }
                    else {
                        Console.WriteLine("Missing argument packageId");
                    }

                    Console.WriteLine("Signal: Uninstall Package completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "statistics", StringComparison.OrdinalIgnoreCase) == 0 || String.Compare(message.Name, "stats", StringComparison.OrdinalIgnoreCase) == 0) {
                    this.Statistics();
                }
                else if (String.Compare(message.Name, "help", StringComparison.OrdinalIgnoreCase) == 0) {
                    this.Help();
                }
                else if (String.Compare(message.Name, "ok", StringComparison.OrdinalIgnoreCase) == 0) {
                    // Do nothing, all is good.
                }
                else {
                    processed = false;
                }

                message.Dispose();
            }
            else {
                processed = false;
            }

            return processed;
        }

        /// <summary>
        /// Updates Procon if it is not currently running, then attempts to start up the appdomain
        /// </summary>
        private void Start() {
            // Update the server if it is currently stopped
            this.UpdateCore();

            if (this.Status == ServiceStatusType.Stopped) {
                try {
                    this.Status = ServiceStatusType.Starting;

                    this.ServiceDomain = AppDomain.CreateDomain("Procon.Instance", null, new AppDomainSetup() {
                        PrivateBinPath = String.Join(";", new[] {
                            Defines.PackageMyrconProconCoreLibNet40,
                            Defines.PackageMyrconProconSharedLibNet40
                        })
                    });

                    this.ServiceLoaderProxy = (ServiceLoaderProxy)this.ServiceDomain.CreateInstanceAndUnwrap(typeof(ServiceLoaderProxy).Assembly.FullName, typeof(ServiceLoaderProxy).FullName);
                    this.ServiceLoaderProxy.Create();

                    this.ServiceLoaderProxy.ParseCommandLineArguments(this.Arguments);

                    this.ServiceLoaderProxy.Start();

                    this.Status = ServiceStatusType.Started;
                }
                catch (Exception e) {
                    ServiceControllerHelpers.LogUnhandledException("ServiceController.Start", e);

                    this.Stop();
                }
            }
        }

        /// <summary>
        /// Outputs some usage statistics on the appdomain
        /// </summary>
        private void Statistics() {
            if (this.Status == ServiceStatusType.Started) {

                // todo - does this suck up the cpu/memory?
                AppDomain.MonitoringIsEnabled = true;

                Console.WriteLine("Service Controller");
                Console.WriteLine("+--------------------------------------------------------+");
                Console.WriteLine("MonitoringSurvivedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringSurvivedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalAllocatedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalProcessorTime: {0}", AppDomain.CurrentDomain.MonitoringTotalProcessorTime);

                Console.WriteLine("");

                Console.WriteLine("Service Domain");
                Console.WriteLine("+--------------------------------------------------------+");
                Console.WriteLine("MonitoringSurvivedMemorySize: {0:N0} K", this.ServiceDomain.MonitoringSurvivedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalAllocatedMemorySize: {0:N0} K", this.ServiceDomain.MonitoringTotalAllocatedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalProcessorTime: {0}", this.ServiceDomain.MonitoringTotalProcessorTime);
            }
        }

        /// <summary>
        /// Outputs some useful commands to enter.
        /// </summary>
        private void Help() {

            Console.WriteLine("Instance Control");
            Console.WriteLine("+-------------------+------+----------+--------+-------+-----------+");
            Console.WriteLine("| Command           | Save | Shutdown | Update | Start | Terminate |");
            Console.WriteLine("+-------------------+------+----------+--------+-------+-----------+");
            Console.WriteLine("| start             |  -   |    -     |   x    |   x   |     -     |");
            Console.WriteLine("| restart           |  x   |    x     |   x    |   x   |     -     |");
            Console.WriteLine("| merge-package     |  x   |    x     |   x    |   x   |     -     |");
            Console.WriteLine("| uninstall-package |  x   |    x     |   x    |   x   |     -     |");
            Console.WriteLine("| stop              |  x   |    x     |   -    |   -   |     -     |");
            Console.WriteLine("| exit              |  x   |    x     |   -    |   -   |     x     |");
            Console.WriteLine("+-------------------+------+----------+--------+-------+-----------+");
            Console.WriteLine("");
            Console.WriteLine("Information");
            Console.WriteLine("+-------------------+----------------------------------------------+");
            Console.WriteLine("| Command           | Description                                  |");
            Console.WriteLine("+-------------------+----------------------------------------------+");
            Console.WriteLine("| stats             | Statistics running on the current instance.  |");
            Console.WriteLine("| help              | This display.                                |");
            Console.WriteLine("| merge-package     | Installs/Updates a package to latest version.|");
            Console.WriteLine("| uninstall-package | Removes the package and unused dependencies. |");
            Console.WriteLine("+-------------------+----------------------------------------------+");
            Console.WriteLine("");
            Console.WriteLine("merge-package -uri [repository-uri] -packageid [package-id]");
            Console.WriteLine("uninstall-package -packageid [package-id]");
        }

        /// <summary>
        /// Stops the service, updates it then starts it again.
        /// </summary>
        private void Restart() {
            this.Stop();
            
            this.UpdateCore();

            this.Start();
        }

        /// <summary>
        /// Updates the procon instance, provided it is currently stopped.
        /// </summary>
        private void UpdateCore() {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (this.Settings.ServiceUpdateCore == true) {
                if (this.Status == ServiceStatusType.Stopped) {
                    ServiceControllerHelpers.InstallOrUpdatePackage(this.Settings.PackagesDefaultSourceRepositoryUri, Defines.PackageMyrconProconCore);
                }
            }
        }

        /// <summary>
        /// Installs or updates a specific package with id from a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        private void MergePackage(String uri, String packageId) {
            this.Stop();

            if (this.Status == ServiceStatusType.Stopped) {
                ServiceControllerHelpers.InstallOrUpdatePackage(uri, packageId);
            }

            this.Start();
        }

        /// <summary>
        /// Uninstalls a package from the local repository
        /// </summary>
        /// <param name="packageId">The package id to search for and install/update</param>
        private void UninstallPackage(String packageId) {
            this.Stop();

            if (this.Status == ServiceStatusType.Stopped) {
                ServiceControllerHelpers.UninstallPackage(packageId);
            }

            this.Start();
        }

        /// <summary>
        /// Saves the config, shuts down the instance and finally collapses the app domain.
        /// </summary>
        private void Stop() {

            if (this.ServiceLoaderProxy != null || this.ServiceDomain != null) {
                this.Status = ServiceStatusType.Stopping;

                System.Console.WriteLine("Shutting down instance..");

                if (this.ServiceLoaderProxy != null) {
                    try {
                        System.Console.Write("Writing config..");
                        this.ServiceLoaderProxy.WriteConfig();
                        System.Console.WriteLine(" Complete");
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.Stop.WriteConfig", e);
                    }

                    try {
                        System.Console.Write("Cleaning up..");
                        this.ServiceLoaderProxy.Dispose();
                        System.Console.WriteLine(" Complete");
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.Stop.ServiceLoaderProxy.Dispose", e);
                    }

                    this.ServiceLoaderProxy = null;
                }

                if (this.ServiceDomain != null) {
                    System.Console.Write("Collapsing domain..");

                    try {
                        AppDomain.Unload(this.ServiceDomain);
                        this.ServiceDomain = null;

                        System.Console.WriteLine(" Complete");
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.Stop.ServiceDomain.Unload", e);

                        System.Console.WriteLine(" Error");
                        Console.WriteLine("\tUnable to unload domain.");
                    }
                }
            }

            // After running through the above, provided both are set to null then shutting down was successful.
            if (this.ServiceLoaderProxy == null && this.ServiceDomain == null) {
                this.Status = ServiceStatusType.Stopped;
            }
        }

        public void Dispose() {
            this.Stop();

            this.PollingTask.Dispose();
        }
    }
}
