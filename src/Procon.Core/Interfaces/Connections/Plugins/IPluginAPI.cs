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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Connections.Text;
    using Procon.Core.Interfaces.Connections.Plugins.Variables;
    using Procon.Core.Interfaces.Security;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;
    using Procon.Core.Interfaces.Security.Objects;

    public interface IPluginAPI : IDisposable {

        string Hostname { get; set; }
        ushort Port { get; set; }
        System.Version ProconVersion { get; set; }

        string Name { get; set; }
        string Author { get; set; }
        string Website { get; set; }
        string Description { get; set; }

        DirectoryInfo ConfigDirectoryInfo { get; set; }
        DirectoryInfo LogDirectoryInfo { get; set; }
        //string ConfigFile { get; set; }

        //Config Config { get; set; }

        #region Callbacks

        PluginAPI.ProtocolActionHandler ProtocolActionCallback { get; set; }

        PluginAPI.ExecuteHandler ExecuteCallback { get; set; }

        PluginAPI.TextCommandHandler RegisterTextCommandCallback { get; set; }
        PluginAPI.TextCommandHandler UnregisterTextCommandCallback { get; set; }

        PluginAPI.LocHandler LocCallback { get; set; }
        PluginAPI.PlayerLocHandler PlayerLocCallback { get; set; }

        #endregion

        #region Core

        /// <summary>
        /// This is so only one call needs to be made across the AppDomain
        /// to collect all the information about this plugin.
        /// </summary>
        PluginDetails PluginDetails { get; }

        void SetPluginVariable(Variable variable);

        void LoadConfig();
        
        #endregion

        #region Events

        void PluginEvent(PluginEventArgs e);

        void GameEvent(GameEventArgs e);

        void ClientEvent(ClientEventArgs e);

        void TextCommandEvent(TextCommandEventArgs e);

        #endregion
    }
}
