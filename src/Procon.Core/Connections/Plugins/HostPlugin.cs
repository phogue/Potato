using System;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Service.Shared;

namespace Procon.Core.Connections.Plugins {

    /// <summary>
    /// This is the Procon side class to handle the proxy to the app domain, as well as the plugins
    /// cleanup.
    /// </summary>
    public sealed class HostPlugin : Executable, IRenewableLease {

        /// <summary>
        /// The name of the plugin, also used as it's namespace
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The loaded plugin GUID
        /// todo This remotes once, which isn't very expensive if used in moderation, but perhaps we should cache this result if this ever becomes a hot spot?
        /// </summary>
        public Guid PluginGuid {
            get {
                return this.Proxy != null ? this.Proxy.PluginGuid : Guid.Empty;
            }
            // Commented so this object can be serialized.
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// If this plugin is enabled or not.
        /// todo This remotes twice, which isn't very expensive if used in moderation, but perhaps we should cache this result if this ever becomes a hot spot?
        /// </summary>
        public bool IsEnabled {
            get {
                return this.PluginFactory != null && this.PluginFactory.IsPluginEnabled(this.PluginGuid);
            }
            // Commented so this object can be serialized.
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// The path to the dll file
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String Path { get; set; }

        /// <summary>
        /// Reference to the plugin loader proxy
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IRemotePluginController PluginFactory { get; set; }

        /// <summary>
        /// The owner of this plugin.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// Reference to the plugin loaded in the AppDomain for remoting calls.
        /// </summary>
        private IRemotePlugin Proxy { get; set; }

        public override ExecutableBase Execute() {
            if (File.Exists(this.Path) == true) {
                this.Name = new FileInfo(this.Path).Name.Replace(".dll", "");

                this.Proxy = this.PluginFactory.Create(this.Path, this.Name + ".Program");

                if (this.Proxy != null) {

                    // register game specific call backs.
                    this.Proxy.ConnectionGuid = this.ConnectionGuid;

                    // check the plugin's config directory
                    this.Proxy.ConfigDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.ConfigsDirectory,this.ConnectionGuid.ToString(), this.PluginGuid.ToString()));
                    this.Proxy.ConfigDirectoryInfo.Create();
                        
                    // check the plugin's log directory
                    this.Proxy.LogDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.LogsDirectory, this.ConnectionGuid.ToString(), this.PluginGuid.ToString()));
                    this.Proxy.LogDirectoryInfo.Create();

                    // Tell the plugin it's ready to begin, everything is setup and ready 
                    // for it to start loading its config.
                    this.Proxy.GenericEvent(new GenericEventArgs() {
                        GenericEventType = GenericEventType.PluginsPluginLoaded
                    });
                }
            }

            return base.Execute();
        }

        /// <summary>
        /// Renews the lease on the proxy to the appdomain hosted pluin.
        /// </summary>
        public void RenewLease() {
            ILease lease = ((MarshalByRefObject) this.Proxy).GetLifetimeService() as ILease;

            if (lease != null) {
                lease.Renew(lease.InitialLeaseTime);
            }
        }

        public override void Dispose() {

            if (this.Proxy != null) this.Proxy.Dispose();
            this.Proxy = null;

            // Disposed of in the plugin controller.
            this.PluginFactory = null;

            base.Dispose();
        }
    }
}
