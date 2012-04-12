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
using System.ComponentModel;

namespace Procon.Net {

    [Serializable]
    public class ClientEventArgs : EventArgs {

        public DateTime Stamp { get; set; }

        /// <summary>
        /// Stores the type of event (ConnectionStateChanged, PacketSent etc)
        /// </summary>
        public ClientEventType EventType { get; set; }

        /// <summary>
        /// The state of the connection (Connected/Disconnected/LoggedIn)
        /// </summary>
        public ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// The last error recorded on the connection
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Exception ConnectionError { get; set; }

        /// <summary>
        /// The packet that was sent/recv - EventType == PacketSent || PacketReceived
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Packet Packet { get; set; }

        public ClientEventArgs() {
            this.Stamp = DateTime.Now;
        }
    }
}
