using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Procon.Service.Shared {
    /// <summary>
    /// The proxy to be loaded in the service appdomain
    /// </summary>
    public sealed class ServiceLoaderProxy : MarshalByRefObject, IServiceLoaderProxy {
        /// <summary>
        /// The proxy to the Procon.Core.Instance object.
        /// </summary>
        public IService Service { get; set; }

        public override object InitializeLifetimeService() {
            return null;
        }

        /// <summary>
        /// Creates the procon instance in the procon instance appdomain
        /// </summary>
        public void Create() {
            this.Service = (IService)Activator.CreateInstanceFrom(
                Defines.SearchPaths(Defines.ProconCoreDll, new List<String> {
                    Defines.BaseDirectory.FullName,
                    Defines.PackageMyrconProconCoreLibNet40.FullName,
                    Defines.PackageMyrconProconSharedLibNet40.FullName
                }).First(),
                Defines.TypeProconCoreInstanceController,
                false,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                null,
                null,
                null
            ).Unwrap();
        }

        public void Start() {
            if (this.Service != null) this.Service.Start();
        }

        public void WriteConfig() {
            if (this.Service != null) this.Service.WriteConfig();
        }

        public void Dispose() {
            if (this.Service != null) this.Service.Dispose();
        }

        public void ParseCommandLineArguments(List<string> arguments) {
            if (this.Service != null) this.Service.ParseCommandLineArguments(arguments);
        }

        public ServiceMessage PollService() {
            ServiceMessage message = null;

            if (this.Service != null) {
                var polled = this.Service.PollService();

                // Clone the message so we have no proxy to the other side.
                message = new ServiceMessage() {
                    Name = polled.Name,
                    Arguments = polled.Arguments,
                    Stamp = polled.Stamp
                };
            }

            return message;
        }

        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            ServiceMessage result = null;

            if (this.Service != null) {
                var polled = this.Service.ExecuteMessage(message);

                // Clone the message so we have no proxy to the other side.
                result = new ServiceMessage() {
                    Name = polled.Name,
                    Arguments = polled.Arguments,
                    Stamp = polled.Stamp
                };
            }

            return result;
        }
    }
}