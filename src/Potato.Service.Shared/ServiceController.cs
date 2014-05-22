#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet;
using Potato.Service.Shared.Packages;

namespace Potato.Service.Shared {
    /// <summary>
    /// Manages an instance of Potato in a seperate AppDomain.
    /// </summary>
    public sealed class ServiceController : IDisposable {
        /// <summary>
        /// The domain to load Potato.Core into.
        /// </summary>
        public AppDomain ServiceDomain { get; set; }

        /// <summary>
        /// The loader proxy to then load the Potato core instance.
        /// </summary>
        public IServiceLoaderProxy ServiceLoaderProxy { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public IServiceObserver Observer { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public IServicePackageManager Packages { get; set; }

        /// <summary>
        /// The processed arguments to check/use any service side conditions
        /// </summary>
        public IServiceSettings Settings { get; set; }

        /// <summary>
        /// Our own wrapper for the currently running process.
        /// </summary>
        public IProcess Process { get; set; }

        /// <summary>
        /// The arguments to start any instances with. This is passed on to Potato, not actually used by the service.
        /// </summary>
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Polling handler to ensure the appdomain is still functional.
        /// </summary>
        public Timer Polling { get; set; }

        /// <summary>
        /// WriteConfig handler to periodically write the config to disk, just in case of unexpected outage.
        /// </summary>
        public Timer WriteConfig { get; set; }

        /// <summary>
        /// The type to load as a service proxy (must implement IServiceLoaderProxy)
        /// </summary>
        public Type ServiceLoaderProxyType { get; set; }

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
        /// Called when a help signal comes through.
        /// </summary>
        public Action<ServiceController> SignalHelp { get; set; }
        
        /// <summary>
        /// Called when a result signal is processed.
        /// </summary>
        public Action<ServiceController, ServiceMessage> SignalResult { get; set; }

        /// <summary>
        /// Called before requesting the service domain write it's config.
        /// </summary>
        public Action<ServiceController> WriteServiceConfigBegin { get; set; }

        /// <summary>
        /// Called when the config in the service domain has been written successfully
        /// </summary>
        public Action<ServiceController> WriteServiceConfigEnd { get; set; }

        /// <summary>
        /// Called before requesting the service domain dispose/shutdown.
        /// </summary>
        public Action<ServiceController> DisposeServiceBegin { get; set; }

        /// <summary>
        /// Called when the service domain has successfully been disposed/shutdown.
        /// </summary>
        public Action<ServiceController> DisposeServiceEnd { get; set; }

        /// <summary>
        /// Called before unloading the service domain
        /// </summary>
        public Action<ServiceController> UnloadServiceBegin { get; set; }

        /// <summary>
        /// Called when the service domain has been successfully unloaded
        /// </summary>
        public Action<ServiceController> UnloadServiceEnd { get; set; }

        /// <summary>
        /// Initiates the service controller with the default values
        /// </summary>
        public ServiceController() {
            AppDomain.MonitoringIsEnabled = true;

            this.Observer = new ServiceObserver() {
                Panic = this.Panic
            };

            this.Polling = new Timer(Polling_Tick, this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            this.WriteConfig = new Timer(WriteConfig_Tick, this, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));

            this.ServiceLoaderProxyType = typeof(ServiceLoaderProxy);

            this.Process = new Process();

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
        /// Called when a help signal comes through
        /// </summary>
        private void OnHelp() {
            var handler = this.SignalHelp;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called when a result signal is processed
        /// </summary>
        /// <param name="message">The service message containing further details about the result</param>
        private void OnResult(ServiceMessage message) {
            var handler = this.SignalResult;

            if (handler != null) {
                handler(this, message);
            }
        }

        /// <summary>
        /// Called just before requestin the service to write its configs
        /// </summary>
        private void OnWriteServiceConfigBegin() {
            var handler = this.WriteServiceConfigBegin;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called after the service config has been successfully written.
        /// </summary>
        private void OnWriteServiceConfigEnd() {
            var handler = this.WriteServiceConfigEnd;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called just before requesting the service to dispose and cleanup after itself
        /// </summary>
        private void OnDisposeServiceBegin() {
            var handler = this.DisposeServiceBegin;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called after the service has disposed itself.
        /// </summary>
        private void OnDisposeServiceEnd() {
            var handler = this.DisposeServiceEnd;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called before collapsing/unloading the service domain.
        /// </summary>
        private void OnUnloadServiceBegin() {
            var handler = this.UnloadServiceBegin;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called when the service domain has been successfully unloaded.
        /// </summary>
        private void OnUnloadServiceEnd() {
            var handler = this.UnloadServiceEnd;

            if (handler != null) {
                handler(this);
            }
        }

        /// <summary>
        /// Called when the observer enters a panic (Potato has been stopped for 15 minutes)
        /// </summary>
        public void Panic() {
            // Set to full stop. A panic can be called if we are currently "stopping" or "starting"
            this.Observer.Status = ServiceStatusType.Stopped;
            
            // Attempt to start Potato
            this.Restart();

            // If we didn't start AND core updates are turned off
            if (this.Observer.Status != ServiceStatusType.Started && this.Settings.ServiceUpdateCore == false) {
                // Force a core update check. Something is obviously very wrong with Potato at the moment
                // and no doubt the forums are flooded with people telling me about it :)
                this.Packages.MergePackage(this.Settings.PackagesDefaultSourceRepositoryUri, Defines.PackageMyrconPotatoCore);

                // Try starting Potato again.. if not we'll do this again in 20 minutes.
                this.Restart();
            }
        }

        /// <summary>
        /// Fired every ten seconds to ensure the appdomain is still responding and does not have
        /// any additional messages for us to process.
        /// </summary>
        public void Polling_Tick(object state) {
            if (this.Observer.Status == ServiceStatusType.Started && this.ServiceLoaderProxy != null) {
                AutoResetEvent pollingTimeoutEvent = new AutoResetEvent(false);
                ServiceMessage message = null;

                Task.Factory.StartNew(() => {
                    try {
                        message = this.ServiceLoaderProxy.PollService();
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.Polling_Tick", e);

                        message = null;
                    }
                    finally {
                        pollingTimeoutEvent.Set();
                    }
                });

                // If we don't get a response or the response wasn't processed properly.
                if (pollingTimeoutEvent.WaitOne(this.Settings.ServicePollingTimeout) == false || this.SignalMessage(message) == false) {
                    this.Restart();
                }
            }
        }

        /// <summary>
        /// Fired every 10 minutes to write the current config to disk.
        /// </summary>
        public void WriteConfig_Tick(object state) {
            if (this.Observer.Status == ServiceStatusType.Started && this.ServiceLoaderProxy != null) {
                this.SignalMessage(new ServiceMessage() {
                    Name = "write"
                });
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
                        this.OnHelp();
                    }
                    else if (String.Compare(message.Name, "result", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.OnResult(message);
                    }
                    else if (String.Compare(message.Name, "write", StringComparison.OrdinalIgnoreCase) == 0) {
                        this.WriteServiceConfig();
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
        /// Updates Potato if it is not currently running, then attempts to start up the appdomain
        /// </summary>
        public void Start() {
            // Update the server if it is currently stopped
            this.UpdateCore();

            if (this.Observer.Status == ServiceStatusType.Stopped) {
                try {
                    this.Observer.Status = ServiceStatusType.Starting;

                    this.ServiceDomain = AppDomain.CreateDomain("Potato.Instance", null, new AppDomainSetup() {
                        ApplicationBase = Defines.BaseDirectory.FullName,
                        PrivateBinPath = String.Join(";", new[] {
                            Defines.PackageMyrconPotatoCoreLibNet40.FullName,
                            Defines.PackageMyrconPotatoSharedLibNet40.FullName
                        })
                    });

                    this.ServiceLoaderProxy = (IServiceLoaderProxy)this.ServiceDomain.CreateInstanceAndUnwrap(this.ServiceLoaderProxyType.Assembly.FullName, this.ServiceLoaderProxyType.FullName);
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
        /// Stops the service, updates it then starts it again.
        /// </summary>
        public void Restart() {
            this.Stop();
            
            this.Start();
        }

        /// <summary>
        /// Updates the Potato instance, provided it is currently stopped.
        /// </summary>
        public void UpdateCore() {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (this.Settings.ServiceUpdateCore == true) {
                if (this.Observer.Status == ServiceStatusType.Stopped) {
                    this.Packages.MergePackage(this.Settings.PackagesDefaultSourceRepositoryUri, Defines.PackageMyrconPotatoCore);
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
        /// Runs a command on the instance, returning a signal message. The signal message is
        /// processed the same as if it was found while polling the instance.
        /// </summary>
        /// <param name="message">The message to send to the instance.</param>
        public void ExecuteMessage(ServiceMessage message) {
            this.Start();

            if (this.Observer.Status == ServiceStatusType.Started) {
                this.SignalMessage(this.ServiceLoaderProxy.ExecuteMessage(message));
            }
        }

        /// <summary>
        /// Writes the service config
        /// </summary>
        public void WriteServiceConfig() {
            if (this.ServiceLoaderProxy != null) {
                AutoResetEvent writeServiceConfigWait = new AutoResetEvent(false);

                this.OnWriteServiceConfigBegin();

                Task.Factory.StartNew(() => {
                    try {
                        this.ServiceLoaderProxy.WriteConfig();

                        this.OnWriteServiceConfigEnd();
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.WriteServiceConfig", e);
                    }
                    finally {
                        writeServiceConfigWait.Set();
                    }
                });

                if (writeServiceConfigWait.WaitOne(this.Settings.WriteServiceConfigTimeout) == false) {
                    ServiceControllerHelpers.LogUnhandledException("ServiceController.WriteServiceConfig", new Exception("Timeout waiting for service to write config."));
                }
            }
        }

        /// <summary>
        /// Disposes the service
        /// </summary>
        public void DisposeService() {
            if (this.ServiceLoaderProxy != null) {
                AutoResetEvent disposeServerWait = new AutoResetEvent(false);

                this.OnDisposeServiceBegin();

                Task.Factory.StartNew(() => {
                    try {
                        this.ServiceLoaderProxy.Dispose();

                        this.OnDisposeServiceEnd();
                    }
                    catch (Exception e) {
                        ServiceControllerHelpers.LogUnhandledException("ServiceController.DisposeService", e);
                    }
                    finally {
                        disposeServerWait.Set();
                    }
                });

                if (disposeServerWait.WaitOne(this.Settings.DisposeServiceTimeout) == false) {
                    ServiceControllerHelpers.LogUnhandledException("ServiceController.DisposeService", new Exception("Timeout waiting for service to dispose."));
                }

                this.ServiceLoaderProxy = null;
            }
        }

        /// <summary>
        /// Unloads the service domain
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         We have no time out for this process. If this fails then when we start it will behave strangely anyway.
        ///         We'll be binding to identical ports, unable to perform updates etc.
        ///     </para>
        ///     <para>
        ///         If unloading the domain fails then the process will be terminated.
        ///     </para>
        /// </remarks>
        public void UnloadService() {
            if (this.ServiceDomain != null) {
                try {
                    this.OnUnloadServiceBegin();

                    AppDomain.Unload(this.ServiceDomain);

                    this.OnUnloadServiceEnd();
                }
                catch (Exception e) {
                    ServiceControllerHelpers.LogUnhandledException("ServiceController.UnloadService.General", e);

                    // The AppDomain is still running, though it may be in serious trouble or an unknown state.
                    // We terminate the current process instead of continuing, having reached a critical state.
                    this.Process.Kill();
                }
                finally {
                    this.ServiceDomain = null;
                }
            }
        }

        /// <summary>
        /// Saves the config, shuts down the instance and finally collapses the app domain.
        /// </summary>
        public void Stop() {
            if (this.ServiceLoaderProxy != null || this.ServiceDomain != null) {
                this.Observer.Status = ServiceStatusType.Stopping;

                this.WriteServiceConfig();

                this.DisposeService();

                this.UnloadService();
            }

            // After running through the above, provided both are set to null then shutting down was successful.
            if (this.ServiceLoaderProxy == null && this.ServiceDomain == null) {
                this.Observer.Status = ServiceStatusType.Stopped;
            }
        }

        public void Dispose() {
            this.Stop();

            this.Polling.Dispose();
            this.WriteConfig.Dispose();
        }
    }
}
