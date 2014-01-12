using System;
using System.Collections.Generic;
using System.Threading;
using NuGet;
using Procon.Service.Shared.Packages;

namespace Procon.Service.Shared {
    /// <summary>
    /// Manages an instance of Procon in a seperate AppDomain.
    /// </summary>
    public sealed class ServiceController : IDisposable {
        /// <summary>
        /// The domain to load Procon.Core into.
        /// </summary>
        public AppDomain ServiceDomain { get; set; }

        /// <summary>
        /// The loader proxy to then load the procon core instance.
        /// </summary>
        public ServiceLoaderProxy ServiceLoaderProxy { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public IServiceObserver Observer { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public IServicePackageManager Packages { get; set; }

        /// <summary>
        /// The arguments to start any instances with. This is passed on to Procon, not actually used by the service.
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
        /// Called when a signal message is starting
        /// </summary>
        public Action<ServiceController, ServiceMessage> SignalBegin { get; set; }

        /// <summary>
        /// Called when a signal message has completed
        /// </summary>
        public Action<ServiceController, ServiceMessage, Double> SignalEnd { get; set; }

        /// <summary>
        /// Called when a signal demands parameters but the parameters are in an incorrect format or missing.
        /// </summary>
        public Action<ServiceController, List<String>> SignalParameterError { get; set; }

        /// <summary>
        /// Called when a statistics signal comes through
        /// </summary>
        public Action<ServiceController, AppDomain> SignalStatistics { get; set; }

        /// <summary>
        /// Initiates the service controller with the default values
        /// </summary>
        public ServiceController() {
            AppDomain.MonitoringIsEnabled = true;

            this.Observer = new ServiceObserver() {
                Panic = this.Panic
            };

            this.PollingTask = new Timer(PollingTask_Tick, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(10));

            this.Arguments = new List<String>();
            this.Settings = new ServiceSettings();
            
            this.Packages = new ServicePackageManager() {
                LocalRepository = PackageRepositoryFactory.Default.CreateRepository(Defines.PackagesDirectory.FullName)
            };
        }

        /// <summary>
        /// Called before processing a signal begins
        /// </summary>
        /// <param name="message">The message being processed</param>
        private void OnSignalBegin(ServiceMessage message) {
            var handler = this.SignalBegin;

            if (handler != null) {
                handler(this, message);
            }
        }

        /// <summary>
        /// Called when a message processing is completed.
        /// </summary>
        /// <param name="message">The message being processed</param>
        /// <param name="seconds">The time in seconds it took to process the signal</param>
        private void OnSignalEnd(ServiceMessage message, Double seconds) {
            var handler = this.SignalEnd;

            if (handler != null) {
                handler(this, message, seconds);
            }
        }

        /// <summary>
        /// Called when a signal requires parameters, but the parameters are missing or invalid.
        /// </summary>
        /// <param name="parameters">The list of parameters that are missing or invalid.</param>
        private void OnSignalParameterError(List<String> parameters) {
            var handler = this.SignalParameterError;

            if (handler != null) {
                handler(this, parameters);
            }
        }

        /// <summary>
        /// Called when a statistics signal comes through
        /// </summary>
        private void OnStatistics() {
            var handler = this.SignalStatistics;

            if (handler != null) {
                handler(this, this.ServiceDomain);
            }
        }

        /// <summary>
        /// Called when the observer enters a panic (Procon has been stopped for 15 minutes)
        /// </summary>
        private void Panic() {
            // Set to full stop. A panic can be called if we are currently "stopping" or "starting"
            this.Observer.Status = ServiceStatusType.Stopped;
            
            // Attempt to start Procon
            this.Start();

            // If we didn't start AND core updates are turned off
            if (this.Observer.Status != ServiceStatusType.Started && this.Settings.ServiceUpdateCore == false) {
                // Force a core update check. Something is obviously very wrong with Procon at the moment
                // and no doubt the forums are flooded with people telling me about it :)
                this.Packages.MergePackage(this.Settings.PackagesDefaultSourceRepositoryUri, Defines.PackageMyrconProconCore);

                // Try starting Procon again.. if not we'll do this again in 20 minutes.
                this.Start();
            }
        }

        /// <summary>
        /// Fired every ten seconds to ensure the appdomain is still responding and does not have
        /// any additional messages for us to process.
        /// </summary>
        private void PollingTask_Tick(object state) {
            if (this.Observer.Status == ServiceStatusType.Started && this.ServiceLoaderProxy != null) {
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

                // Ignore "nop" messages
                if (String.Compare(message.Name, "nop", StringComparison.OrdinalIgnoreCase) != 0) {
                    this.OnSignalBegin(message);

                    if (String.Compare(message.Name, "start", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.Start();
                    }
                    else if (String.Compare(message.Name, "stop", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.Stop();
                    }
                    else if (String.Compare(message.Name, "restart", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.Restart();
                    }
                    else if (String.Compare(message.Name, "merge", StringComparison.OrdinalIgnoreCase) == 0) {
                        if (message.Arguments.ContainsKey("uri") == true && message.Arguments.ContainsKey("packageid") == true) {
                            this.MergePackage(message.Arguments["uri"], message.Arguments["packageid"]);
                        }
                        else {
                            this.OnSignalParameterError(new List<String>() {
                                "uri",
                                "packageId"
                            });
                        }
                    }
                    else if (String.Compare(message.Name, "uninstall", StringComparison.OrdinalIgnoreCase) == 0) {
                        if (message.Arguments.ContainsKey("packageid") == true) {
                            this.UninstallPackage(message.Arguments["packageid"]);
                        }
                        else {
                            this.OnSignalParameterError(new List<String>() {
                                "packageId"
                            });
                        }
                    }
                    else if (String.Compare(message.Name, "statistics", StringComparison.OrdinalIgnoreCase) == 0 || String.Compare(message.Name, "stats", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.OnStatistics();
                    }
                    else if (String.Compare(message.Name, "help", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.Help();
                    }
                    else {
                        processed = false;
                    }

                    this.OnSignalEnd(message, (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                // else do nothing for nop messages, the message was to do nothing.

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
        public void Start() {
            // Update the server if it is currently stopped
            this.UpdateCore();

            if (this.Observer.Status == ServiceStatusType.Stopped) {
                try {
                    this.Observer.Status = ServiceStatusType.Starting;

                    this.ServiceDomain = AppDomain.CreateDomain("Procon.Instance", null, new AppDomainSetup() {
                        PrivateBinPath = String.Join(";", new[] {
                            Defines.PackageMyrconProconCoreLibNet40.FullName,
                            Defines.PackageMyrconProconSharedLibNet40.FullName
                        })
                    });

                    this.ServiceLoaderProxy = (ServiceLoaderProxy)this.ServiceDomain.CreateInstanceAndUnwrap(typeof(ServiceLoaderProxy).Assembly.FullName, typeof(ServiceLoaderProxy).FullName);
                    this.ServiceLoaderProxy.Create();

                    this.ServiceLoaderProxy.ParseCommandLineArguments(this.Arguments);

                    this.ServiceLoaderProxy.Start();

                    this.Observer.Status = ServiceStatusType.Started;
                }
                catch (Exception e) {
                    ServiceControllerHelpers.LogUnhandledException("ServiceController.Start", e);

                    this.Stop();
                }
            }
        }

        /// <summary>
        /// Outputs some useful commands to enter.
        /// </summary>
        public void Help() {

            Console.WriteLine("Instance Control");
            Console.WriteLine("+-------------------+------+----------+--------+-------+-----------+");
            Console.WriteLine("| Command           | Save | Shutdown | Update | Start | Terminate |");
            Console.WriteLine("+-------------------+------+----------+--------+-------+-----------+");
            Console.WriteLine("| start             |  -   |    -     |   x    |   x   |     -     |");
            Console.WriteLine("| restart           |  x   |    x     |   x    |   x   |     -     |");
            Console.WriteLine("| merge             |  x   |    x     |   x    |   x   |     -     |");
            Console.WriteLine("| uninstall         |  x   |    x     |   x    |   x   |     -     |");
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
            Console.WriteLine("| merge             | Installs/Updates a package to latest version.|");
            Console.WriteLine("| uninstall         | Removes the package and unused dependencies. |");
            Console.WriteLine("+-------------------+----------------------------------------------+");
            Console.WriteLine("");
            Console.WriteLine("merge-package -uri [repository-uri] -packageid [package-id]");
            Console.WriteLine("uninstall-package -packageid [package-id]");
        }

        /// <summary>
        /// Stops the service, updates it then starts it again.
        /// </summary>
        public void Restart() {
            this.Stop();
            
            this.Start();
        }

        /// <summary>
        /// Updates the procon instance, provided it is currently stopped.
        /// </summary>
        public void UpdateCore() {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (this.Settings.ServiceUpdateCore == true) {
                if (this.Observer.Status == ServiceStatusType.Stopped) {
                    this.Packages.MergePackage(this.Settings.PackagesDefaultSourceRepositoryUri, Defines.PackageMyrconProconCore);
                }
            }
        }

        /// <summary>
        /// Installs or updates a specific package with id from a repository
        /// </summary>
        /// <param name="uri">The source repository of the package</param>
        /// <param name="packageId">The package id to search for and install/update</param>
        public void MergePackage(String uri, String packageId) {
            this.Stop();

            if (this.Observer.Status == ServiceStatusType.Stopped) {
                this.Packages.MergePackage(uri, packageId);
            }

            this.Start();
        }

        /// <summary>
        /// Uninstalls a package from the local repository
        /// </summary>
        /// <param name="packageId">The package id to search for and install/update</param>
        public void UninstallPackage(String packageId) {
            this.Stop();

            if (this.Observer.Status == ServiceStatusType.Stopped) {
                this.Packages.UninstallPackage(packageId);
            }

            this.Start();
        }

        /// <summary>
        /// Saves the config, shuts down the instance and finally collapses the app domain.
        /// </summary>
        public void Stop() {

            if (this.ServiceLoaderProxy != null || this.ServiceDomain != null) {
                this.Observer.Status = ServiceStatusType.Stopping;

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
                this.Observer.Status = ServiceStatusType.Stopped;
            }
        }

        public void Dispose() {
            this.Stop();

            this.PollingTask.Dispose();
        }
    }
}
