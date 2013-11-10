using System;

namespace Procon.Core.Connections.Plugins {
    public interface IPluginLoaderProxy : IExecutableBase {

        IRemotePlugin Create(String assemblyFile, String typeName);
    }
}
