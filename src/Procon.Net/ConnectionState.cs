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

using System.Text;

namespace Procon.Net {
    // Listener Ideal: Disconnected, Listening, Ready
    // Client Connection Ideal: Disconnected, Connecting, Connected, Ready, LoggedIn
    [Serializable]
    public enum ConnectionState {
        /// <summary>
        /// Connection/Listener is down
        /// </summary>
        Disconnected,
        /// <summary>
        /// Connection/Listener is shutting down, connections will be closed soon
        /// </summary>
        Disconnecting,
        /// <summary>
        /// Attempting a client connection
        /// </summary>
        Connecting,
        /// <summary>
        /// Client connection has been established
        /// </summary>
        Connected,
        /// <summary>
        /// Server is listening on a port for incoming connections
        /// </summary>
        Listening,
        /// <summary>
        /// Connection/Listener is ready to accept and send data
        /// </summary>
        Ready,
        /// <summary>
        /// Connection has been authenticated
        /// </summary>
        LoggedIn
    }
}
