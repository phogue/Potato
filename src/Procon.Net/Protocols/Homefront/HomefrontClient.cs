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

namespace Procon.Net.Protocols.Homefront {

    public class HomefrontClient : TCPClient<HomefrontPacket> {

        public HomefrontClient(string hostname, ushort port)
            : base(hostname, port) {
                this.PacketSerializer = new HomefrontPacketSerializer();
        }
        /*
        protected override HomefrontPacket CreatePacket(byte[] packet) {
            return new HomefrontPacket(packet);
        }

        protected override UInt32 DecodePacketSize(byte[] packet) {
            return HomefrontPacket.DecodePacketSize(packet);
        }

        protected override UInt32 GetPacketHeaderSize() {
            return HomefrontPacket.INT_INCOMING_PACKET_HEADER_SIZE;
        }
        */
        public override void Shutdown() {
            if (this.ConnectionState == Net.ConnectionState.Ready || this.ConnectionState == Net.ConnectionState.LoggedIn || this.ConnectionState == Net.ConnectionState.Connected) {
                this.Send(new HomefrontPacket(PacketOrigin.Client, true, MessageType.ClientDisconnect, "SD"));
            }

            base.Shutdown();
        }
    }
}
