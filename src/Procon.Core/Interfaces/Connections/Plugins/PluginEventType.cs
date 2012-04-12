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

namespace Procon.Core.Interfaces.Connections.Plugins {

    [Serializable]
    public enum PluginEventType {
        /// <summary>
        /// All methods are currently being registered so the plugin can communicate back with procon
        /// </summary>
        RegisteringCallbacks,
        /// <summary>
        /// Methods have been registered, the plugin can now function as normal
        /// </summary>
        CallbacksRegistered,
        /// <summary>
        /// The path and config name have been set, the config may now be loaded.
        /// </summary>
        ConfigSetup,
        /// <summary>
        /// The config for this plugin is being loaded
        /// </summary>
        ConfigLoading,
        /// <summary>
        /// The config for this plugin has been loaded
        /// </summary>
        ConfigLoaded
    }
}
