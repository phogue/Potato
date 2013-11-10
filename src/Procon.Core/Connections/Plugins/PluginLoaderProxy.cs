using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Procon.Core.Connections.Plugins {
    public class PluginLoaderProxy : ExecutableBase, IPluginLoaderProxy {

        protected List<IRemotePlugin> LoadedPlugins = new List<IRemotePlugin>();

        public IRemotePlugin Create(String assemblyFile, String typeName) {

            IRemotePlugin loadedPlugin = (IRemotePlugin)Activator.CreateInstanceFrom(
                assemblyFile,
                typeName,
                false,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                null,
                null,
                null
            ).Unwrap();
            
            this.LoadedPlugins.Add(loadedPlugin);

            return loadedPlugin;
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            List<IExecutableBase> list = new List<IExecutableBase>();

            if (command.Scope != null && command.Scope.PluginGuid != Guid.Empty) {
                this.LoadedPlugins.Where(plugin => plugin.PluginGuid == command.Scope.PluginGuid).ToList().ForEach(list.Add);
            }
            else {
                // Add all of the plugins.
                this.LoadedPlugins.ForEach(list.Add);
            }

            return list;
        }
    }
}