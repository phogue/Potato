using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Procon.Service {
    using Procon.Service.Shared;

    public class ServiceController {

        /// <summary>
        /// The domain to load Procon.Core into.
        /// </summary>
        protected AppDomain ServiceDomain { get; set; }

        /// <summary>
        /// The loader proxy to then load the procon core instance.
        /// </summary>
        protected ServiceLoaderProxy ServiceLoaderProxy { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public ServiceStatusType Status { get; set; }

        /// <summary>
        /// The arguments to start any instances with.
        /// </summary>
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Polling handler to ensure the appdomain is still functional.
        /// </summary>
        public Timer PollingTimer { get; set; }

        public ServiceController() {
            this.Status = ServiceStatusType.Stopped;
            
            // todo - does this suck up the cpu/memory?
            AppDomain.MonitoringIsEnabled = true;

            this.PollingTimer = new Timer(10000);
            this.PollingTimer.Elapsed += new ElapsedEventHandler(OnPollingTimerElapsed);
            this.PollingTimer.Start();
        }

        /// <summary>
        /// Fired every ten seconds to ensure the appdomain is still responding and does not have
        /// any additional messages for us to process.
        /// </summary>
        protected void OnPollingTimerElapsed(object sender, ElapsedEventArgs e) {
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
                else if (String.Compare(message.Name, "update", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Update");
                    this.Update();
                    Console.WriteLine("Signal: Update completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "statistics", StringComparison.OrdinalIgnoreCase) == 0 || String.Compare(message.Name, "stats", StringComparison.OrdinalIgnoreCase) == 0) {
                    Console.WriteLine("Signal: Statistics");
                    this.Statistics();
                    Console.WriteLine("Signal: Statistics completed in {0} seconds", (DateTime.Now - begin).TotalMilliseconds / 1000);
                }
                else if (String.Compare(message.Name, "ok", StringComparison.OrdinalIgnoreCase) == 0) {
                    // Do nothing, all is good.
                }
                else {
                    processed = false;
                }
            }
            else {
                processed = false;
            }

            return processed;
        }

        /// <summary>
        /// Updates Procon if it is not currently running, then attempts to start up the appdomain
        /// </summary>
        protected void Start() {
            // Update the server if it is currently stopped
            this.Update();

            if (this.Status == ServiceStatusType.Stopped) {
                try {
                    this.Status = ServiceStatusType.Starting;

                    this.ServiceDomain = AppDomain.CreateDomain("Procon.Instance");

                    this.ServiceLoaderProxy = (ServiceLoaderProxy)this.ServiceDomain.CreateInstanceAndUnwrap(typeof(ServiceLoaderProxy).Assembly.FullName, typeof(ServiceLoaderProxy).FullName);
                    this.ServiceLoaderProxy.Create();

                    this.ServiceLoaderProxy.ParseCommandLineArguments(this.Arguments);

                    this.ServiceLoaderProxy.Start();

                    this.Status = ServiceStatusType.Started;
                }
                catch {
                    this.Stop();
                }
            }
        }

        /// <summary>
        /// Outputs some usage statistics on the appdomain
        /// </summary>
        protected void Statistics() {
            if (this.Status == ServiceStatusType.Started) {
                Console.WriteLine("Service Controller");
                Console.WriteLine("=============");
                Console.WriteLine("MonitoringSurvivedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringSurvivedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalAllocatedMemorySize: {0:N0} K", AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalProcessorTime: {0}", AppDomain.CurrentDomain.MonitoringTotalProcessorTime);

                Console.WriteLine("");

                Console.WriteLine("Service Domain");
                Console.WriteLine("=============");
                Console.WriteLine("MonitoringSurvivedMemorySize: {0:N0} K", this.ServiceDomain.MonitoringSurvivedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalAllocatedMemorySize: {0:N0} K", this.ServiceDomain.MonitoringTotalAllocatedMemorySize / 1024);
                Console.WriteLine("MonitoringTotalProcessorTime: {0}", this.ServiceDomain.MonitoringTotalProcessorTime);

            }
        }

        /// <summary>
        /// Stops the service, updates it then starts it again.
        /// </summary>
        protected void Restart() {
            this.Stop();
            
            this.Update();

            this.Start();
        }

        /// <summary>
        /// Updates the procon instance, provided it is currently stopped.
        /// </summary>
        protected void Update() {
            if (this.Status == ServiceStatusType.Stopped) {
                new Updater().Execute().Shutdown();
            }
        }

        /// <summary>
        /// Saves the config, shuts down the instance and finally collapses the app domain.
        /// </summary>
        protected void Stop() {

            if (this.ServiceLoaderProxy != null || this.ServiceDomain != null) {
                this.Status = ServiceStatusType.Stopping;

                System.Console.WriteLine("Shutting down instance..");

                if (this.ServiceLoaderProxy != null) {
                    System.Console.Write("Writing config..");
                    this.ServiceLoaderProxy.WriteConfig();
                    System.Console.WriteLine(" Complete");

                    System.Console.Write("Cleaning up..");
                    this.ServiceLoaderProxy.Dispose();
                    System.Console.WriteLine(" Complete");

                    this.ServiceLoaderProxy = null;
                }

                if (this.ServiceDomain != null) {
                    System.Console.Write("Collapsing domain..");

                    try {
                        AppDomain.Unload(this.ServiceDomain);
                        this.ServiceDomain = null;

                        System.Console.WriteLine(" Complete");
                    }
                    catch {
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

    }
}
