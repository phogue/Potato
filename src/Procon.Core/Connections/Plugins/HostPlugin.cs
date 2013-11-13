using System;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Events;
    using Procon.Net;
    using Procon.Core.Utils;

    /// <summary>
    /// This is the Procon side class to handle the proxy to the app domain, as well as the plugins
    /// cleanup.
    /// </summary>
    public sealed class HostPlugin : Executable, IHostPlugin {

        /// <summary>
        /// The name of the plugin, also used as it's namespace
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The loaded plugin GUID
        /// </summary>
        public Guid PluginGuid {
            get {
                return this.Proxy != null ? this.Proxy.PluginGuid : Guid.Empty;
            }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        [XmlIgnore, JsonIgnore]
        public String Path { get; set; }

        /// <summary>
        /// Reference to the plugin loader proxy
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IPluginLoaderProxy PluginFactory { get; set; }

        /// <summary>
        /// Ultimately the owner of this plugin.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Connection Connection { get; set; }

        /// <summary>
        /// Reference to the plugin loaded in the AppDomain for remoting calls.
        /// </summary>
        private IRemotePlugin Proxy { get; set; }

        public override ExecutableBase Execute() {
            if (File.Exists(this.Path) == true) {
                this.Name = new FileInfo(this.Path).Name.Replace(".dll", "");

                this.Proxy = this.PluginFactory.Create(this.Path, this.Name + ".Program");

                if (this.Proxy != null) {

                    // Tell the plugin we are about the setup the callbacks
                    this.Proxy.GenericEvent(new GenericEventArgs() {
                        GenericEventType = GenericEventType.PluginsRegisteringCallbacks
                    });

                    this.Proxy.PluginCallback = this;

                    // register game specific call backs. Connection can be null during unit testing.
                    if (this.Connection != null && this.Connection.Game != null) {

                        this.Proxy.ConnectionGuid = this.Connection.ConnectionGuid;

                        this.Connection.Game.ClientEvent += new Game.ClientEventHandler(Connection_ClientEvent);
                        this.Connection.Game.GameEvent += new Game.GameEventHandler(Connection_GameEvent);
                    }

                    // Tell the plugin that callback have been registered and it may start sending out commands.
                    this.Proxy.GenericEvent(new GenericEventArgs() {
                        GenericEventType = GenericEventType.PluginsCallbacksRegistered
                    });

                    // Connection and Game could be null if we're unit testing.
                    if (this.Connection != null && this.Connection.Game != null) {

                        // check the plugin's config directory
                        this.Proxy.ConfigDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.ConfigsDirectory, System.IO.Path.Combine(PathValidator.Valdiate(String.Format("{0}_{1}", this.Connection.Hostname, this.Connection.Port)), PathValidator.Valdiate(this.Name))));
                        this.Proxy.ConfigDirectoryInfo.Create();

                        // check the plugin's log directory
                        this.Proxy.LogDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.LogsDirectory, System.IO.Path.Combine(PathValidator.Valdiate(String.Format("{0}_{1}", this.Connection.Hostname, this.Connection.Port)), PathValidator.Valdiate(this.Name))));

                        if (!this.Proxy.LogDirectoryInfo.Exists) {
                            this.Proxy.LogDirectoryInfo.Create();
                        }
                    }

                    // Tell the plugin that everything is setup and ready for it to start loading
                    // its config.
                    this.Proxy.GenericEvent(new GenericEventArgs() {
                        GenericEventType = GenericEventType.ConfigSetup
                    });
                }
            }

            return base.Execute();
        }

        private void Connection_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.Proxy != null && e.EventType != ClientEventType.ClientPacketReceived && e.EventType != ClientEventType.ClientPacketSent) {
                this.Proxy.ClientEvent(e);
            }
        }

        private void Connection_GameEvent(Game sender, GameEventArgs e) {
            if (this.Proxy != null) {
                this.Proxy.GameEvent(e);
            }
        }

        /// <summary>
        /// Executes a command in the scope of connection or the entire instance of procon.
        /// </summary>
        /// <remarks><para>This is a proxy called from the plugins appdomain.</para></remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public CommandResultArgs ProxyExecute(Command command) {
            CommandResultArgs result = null;

            command.Origin = CommandOrigin.Plugin;

            // We check for null's on these in case of unit testing.
            if (this.Connection != null && this.Connection.Instance != null) {
                if (command.Scope != null && command.Scope.ConnectionGuid != Guid.Empty) {
                    command.Scope.ConnectionGuid = this.Connection.ConnectionGuid;

                    // Optimization to bypass Instance (and other connections), but passing this to Instance would have the same effect.
                    result = this.Connection.Execute(command);
                }
                else {
                    result = this.Connection.Instance.Execute(command);
                }
            }

            return result;
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

            if (this.Proxy != null) {
                this.Proxy.Dispose();
            }

            // Disposed of in the plugin controller.
            this.PluginFactory = null;

            this.Connection = null;

            base.Dispose();
        }
    }
}
