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

namespace Procon.Core {

    /// <summary>
    /// These events are sent exclusively from the server to the client
    /// </summary>
    [Serializable]
    public enum EventName {
        None,

        InterfaceSynchronization,

        ConnectionsConnectionAdded,
        ConnectionsConnectionRemoved,

        SecurityGroupsAdded,
        SecurityGroupsRemoved,
        SecurityGroupsPermissionAuthorityChanged,
        SecurityGroupsAccountAssigned,
        SecurityGroupsAccountRevoked,

        SecurityAccountsAdded,
        SecurityAccountsRemoved,
        SecurityAccountsUidAssigned,
        SecurityAccountsUidRevoked,

        PluginAdded,
        PluginRemoved,
        PluginVariableSet,

        PackageLoaded,
        PackageStateChanged,

        VariableSet,

        // Game Events
        GameEvent,
        ClientEvent,
        /*
        PlayerlistCommand,
        PunkBusterPlayerlist,
        PunkBusterBeginPlayerlist,
        PunkBusterEndPlayerlist,
        PlayerOnKill,
        PlayerOnJoin,
        PlayerOnLeave,
        PlayerOnChat,
        PlayerTeamChange,
        PlayerSquadChange,
        PlayerOnAuthenticated,
        ServerInfo,
        PacketSent,
        PacketReceived,
        SocketException,
        ConnectionFailure,
        ConnectionStateChanged,
        */
    }
}
