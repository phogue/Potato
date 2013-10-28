using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Utils;

    public class PluginController : Executable {

        /// <summary>
        /// The actual app domain all of the plugins are loaded into.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public AppDomain AppDomainSandbox { get; protected set; }

        /// <summary>
        /// Simple plugin factory reference to load classes into the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public PluginLoaderProxy PluginFactory { get; protected set; }

        /// <summary>
        /// List of plugins loaded in the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<Plugin> Plugins { get; protected set; }

        /// <summary>
        /// The connection which owns this plugin app domain and the connection which the plugins control.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Connection Connection { get; set; }

        // Default Initialization
        public PluginController() : base() {
            this.Plugins = new List<Plugin>();
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override ExecutableBase Execute() {
            // Make sure the plugins directory exists and set it up.
            Directory.CreateDirectory(Defines.PluginsDirectory);

            PluginController.PreparePluginDirectory(new DirectoryInfo(Defines.PluginsDirectory));

            // Use the same evidence as MyComputer.
            Evidence hostEvidence = new Evidence();
            hostEvidence.AddHost(new Zone(SecurityZone.MyComputer));

            // [XpKiller] - Mono workaround.
            AppDomainSetup appDomain = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            if (Type.GetType("Mono.Runtime") != null) {
                appDomain.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            // TODO: - The previous two lines used to use the constant: Defines.PLUGINS_DIRECTORY.
            // However, when I (Imisnew2) was doing core changes, I fubared up the config loading, causing
            // the plugins to load "Debug\Plugins\Localization" instead of "Debug\Localizations" due to the
            // directory being a compilation of CurrentDomain + BaseDirectory.  To counter this, we set the
            // app domains directory to this app domains directory.  Must set permissions or get phogue to
            // remember stuff later.

            PermissionSet pluginSandboxPermissions = new PermissionSet(PermissionState.None);

            pluginSandboxPermissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            pluginSandboxPermissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            pluginSandboxPermissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.PluginsDirectory));
            pluginSandboxPermissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.LogsDirectory));
            pluginSandboxPermissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.LocalizationDirectory));
            pluginSandboxPermissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.ConfigsDirectory));

            pluginSandboxPermissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

            // Create the app domain and the plugin factory in the new domain.
            this.AppDomainSandbox = AppDomain.CreateDomain("ProconPlugin", hostEvidence, appDomain, pluginSandboxPermissions);

            this.PluginFactory = (PluginLoaderProxy)this.AppDomainSandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginLoaderProxy).FullName);

            // Load all the plugins.
            this.LoadPlugins(new DirectoryInfo(Defines.PluginsDirectory));

            // Return the base execution.
            return base.Execute();
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose() {
            foreach (Plugin plugin in this.Plugins) {
                plugin.Dispose();
            }

            this.Plugins.Clear();
            this.Plugins = null;

            AppDomain.Unload(this.AppDomainSandbox);
            this.AppDomainSandbox = null;
            this.PluginFactory = null;

            base.Dispose();
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return new List<IExecutableBase>() {
                this.PluginFactory
            };
        }

        /// <summary>
        /// Copies the necessary files to execute a plugin to the specified directory.
        /// </summary>
        private static void PreparePluginDirectory(DirectoryInfo pluginDirectory) {
            try {
                File.Copy(Defines.ProconDirectoryProconCoreDll, Path.Combine(pluginDirectory.FullName, Defines.ProconCoreDll), true);
                File.Copy(Defines.ProconDirectoryProconNetDll, Path.Combine(pluginDirectory.FullName, Defines.ProconNetDll), true);
                File.Copy(Defines.ProconDirectoryProconNlpDll, Path.Combine(pluginDirectory.FullName, Defines.ProconNlpDll), true);
                // File.Copy(Defines.ProconDirectoryNewtonsoftJsonNet35Dll, Path.Combine(pluginDirectory.FullName, Defines.NewtonsoftJsonNet35Dll), true);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Setup the plugins located in or in sub-folders of this directory.
        /// </summary>
        private void LoadPlugins(DirectoryInfo pluginDirectory) {
            // Find all the dll files recursively within the folder and folders within the specified directory.
            FileInfo[] dllFiles = pluginDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(x =>
                                                                  x.Name != Defines.ProconCoreDll &&
                                                                  x.Name != Defines.ProconNetDll &&
                                                                  x.Name != Defines.ProconNlpDll &&
                                                                  x.Name != Defines.NewtonsoftJsonNet35Dll).ToArray();

            // If there are dll files in this directory, setup the plugins.
            if (dllFiles.Length > 0) {
                // No longer required since the plugin appdomains working dir is set to the procon dir.
                //PreparePluginDirectory(pluginDirectory);
                foreach (String path in dllFiles.Select(x => x.FullName)) {
                    this.Plugins.Add(
                        new Plugin() {
                            Path = path,
                            PluginFactory = PluginFactory,
                            Connection = Connection
                        }.Execute() as Plugin
                    );

                    // this.OnPluginAdded(this, Plugins[Plugins.Count - 1]);
                }
            }
        }
    }
}
