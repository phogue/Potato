// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Utils;

    public class LocalPluginController : PluginController
    {
        // Public Properties
        private AppDomain           AppDomainSandbox         { get; set; }
        private PluginLoaderFactory PluginFactory            { get; set; }
        private PermissionSet       PluginSandboxPermissions { get; set; }

        [JsonIgnore]
        public  SecurityController Security
        {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        [JsonIgnore]
        public  VariableController Variables
        {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
                }
            }
        }
        private VariableController mVariables;

        // Default Initialization
        public LocalPluginController() : base() {
            PluginSandboxPermissions = new PermissionSet(PermissionState.Unrestricted);
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override PluginController Execute()
        {
            AssignEvents();

            // Make sure the plugins directory exists and set it up.
            if (!Directory.Exists(Defines.PLUGINS_DIRECTORY))
                Directory.CreateDirectory(Defines.PLUGINS_DIRECTORY);
            PreparePluginDirectory(new DirectoryInfo(Defines.PLUGINS_DIRECTORY));

            // [XpKiller] - Mono workaround.
            AppDomainSetup appDomain = new AppDomainSetup();
            Type           monoType  = Type.GetType("Mono.Runtime");
            if (monoType != null) appDomain.PrivateBinPath  = AppDomain.CurrentDomain.BaseDirectory;
            else                  appDomain.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            // TODO: - The previous two lines used to use the constant: Defines.PLUGINS_DIRECTORY.
            // However, when I (Imisnew2) was doing core changes, I fubared up the config loading, causing
            // the plugins to load "Debug\Plugins\Localization" instead of "Debug\Localizations" due to the
            // directory being a compilation of CurrentDomain + BaseDirectory.  To counter this, we set the
            // app domains directory to this app domains directory.  Must set permissions or get phogue to
            // remember stuff later.

            // Use the same evidence as MyComputer.
            Evidence hostEvidence = new Evidence();
            hostEvidence.AddHost(new Zone(SecurityZone.MyComputer));

            // Create the app domain and the plugin factory in the new domain.
            AppDomainSandbox = AppDomain.CreateDomain("ProconPlugin", hostEvidence, appDomain, PluginSandboxPermissions);
            PluginFactory    = (PluginLoaderFactory)AppDomainSandbox.CreateInstance("Procon.Core", "Procon.Core.Interfaces.Connections.Plugins.PluginLoaderFactory").Unwrap();

            // Load all the plugins.
            LoadPlugins(new DirectoryInfo(Defines.PLUGINS_DIRECTORY));

            // Return the base execution.
            return base.Execute();
        }

        /// <summary>
        /// Simply calls the base dispose.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
            PluginAdded             += Plugins_PluginAdded;
            PluginRemoved           += Plugins_PluginRemoved;
        }

        /// <summary>
        /// Executes a command based on events sent across the layer.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters)
        {
            if (context.ContextType == ContextType.Connection && Layer.ServerContext(Connection.Hostname, Connection.Port).CompareTo(context) == 0) {
                Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username      = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event   = @event
                    },
                    parameters
                );
            }
        }

        /// <summary>
        /// Lets the layer know a plugin was added.
        /// </summary>
        private void Plugins_PluginAdded(PluginController parent, Plugin item)
        {
            Layer.Request(
                new Layer.Objects.Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.PluginAdded,
                item
            );
        }

        /// <summary>
        /// Lets the layer know a plugin was removed.
        /// </summary>
        private void Plugins_PluginRemoved(PluginController parent, Plugin item)
        {
            this.Layer.Request(
                new Layer.Objects.Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.PluginRemoved,
                item
            );
        }



        /// <summary>
        /// Copies the necessary files to execute a plugin to the specified directory.
        /// </summary>
        private void PreparePluginDirectory(DirectoryInfo pluginDirectory)
        {
            try
            {
                File.Copy(Defines.PROCON_DIRECTORY_PROCON_CORE_DLL, Path.Combine(pluginDirectory.FullName, Defines.PROCON_CORE_DLL), true);
                File.Copy(Defines.PROCON_DIRECTORY_PROCON_CORE_PDB, Path.Combine(pluginDirectory.FullName, Defines.PROCON_CORE_PDB), true);

                File.Copy(Defines.PROCON_DIRECTORY_PROCON_NET_DLL, Path.Combine(pluginDirectory.FullName, Defines.PROCON_NET_DLL), true);
                File.Copy(Defines.PROCON_DIRECTORY_PROCON_NET_PDB, Path.Combine(pluginDirectory.FullName, Defines.PROCON_NET_PDB), true);

                File.Copy(Defines.PROCON_DIRECTORY_NEWTONSOFT_JSON_NET35_DLL, Path.Combine(pluginDirectory.FullName, Defines.NEWTONSOFT_JSON_NET35_DLL), true);
                File.Copy(Defines.PROCON_DIRECTORY_NEWTONSOFT_JSON_NET35_PDB, Path.Combine(pluginDirectory.FullName, Defines.NEWTONSOFT_JSON_NET35_PDB), true);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Setup the plugins located in or in sub-folders of this directory.
        /// </summary>
        private void LoadPlugins(DirectoryInfo pluginDirectory) 
        {
            // Find all the dll files in this directory.
            FileInfo[] dllFiles = pluginDirectory.GetFiles("*.dll").Where(x => 
                                                                  x.Name != Defines.PROCON_CORE_DLL &&
                                                                  x.Name != Defines.PROCON_NET_DLL  &&
                                                                  x.Name != Defines.NEWTONSOFT_JSON_NET35_DLL).ToArray();

            // Recursively load plugins within folders of the specified directory.
            foreach (DirectoryInfo subDirectory in pluginDirectory.GetDirectories())
                LoadPlugins(subDirectory);

            // If there are dll files in this directory, setup the plugins.
            if (dllFiles.Length > 0)
            {
                // No longer required since the plugin appdomains working dir is set to the procon dir.
                //PreparePluginDirectory(pluginDirectory);
                foreach (String path in dllFiles.Select(x => x.FullName)) {
                    Plugins.Add(
                        new LocalPlugin() {
                            Path          = path,
                            PluginFactory = PluginFactory,
                            Connection    = Connection,
                            Security      = Security,
                            Variables     = Variables
                        }.Read()
                    );
                    OnPluginAdded(this, Plugins[Plugins.Count - 1]);
                }
            }
        }



        /// <summary>
        /// Sets variable(s) inside of the plugin using JSON as the bridge.
        /// </summary>
        [Command(Command = CommandName.PluginSetVariable)]
        public override void SetPluginVariable(CommandInitiator initiator, String uid, String jsonVariable)
        {
            if (initiator.CommandOrigin == CommandOrigin.Local && Security.Can(Security.Account(initiator.Username), initiator.Command))
            {
                Plugin plugin = Plugins.Find(x => x.Uid == uid);
                if (plugin != null)
                {
                    Variables.Variable var = Connections.Plugins.Variables.Variable.FromJson(jsonVariable);
                    plugin.SetPluginVariable(initiator, var);

                    Layer.Request(
                        new Layer.Objects.Context() {
                            ContextType = ContextType.Connection,
                            Hostname    = Connection.Hostname,
                            Port        = Connection.Port
                        },
                        CommandName.None,
                        EventName.PluginVariableSet,
                        uid,
                        jsonVariable
                    );
                }
            }
        }
    }
}
