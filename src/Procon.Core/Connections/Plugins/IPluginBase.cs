using System;
using System.IO;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Events;
    using Procon.Net;

    public interface IPluginBase : IExecutableBase, IDisposable {

        Guid PluginGuid { get; }

        Guid ConnectionGuid { get; set; }

        DirectoryInfo ConfigDirectoryInfo { get; set; }
        DirectoryInfo LogDirectoryInfo { get; set; }

        PluginBase.CommandHandler ProxyExecuteCallback { get; set; }

        void GameEvent(GameEventArgs e);

        void ClientEvent(ClientEventArgs e);

        void GenericEvent(GenericEventArgs e);
    }
}
