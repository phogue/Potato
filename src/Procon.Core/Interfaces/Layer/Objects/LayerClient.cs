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
using System.Net;
using System.Net.Sockets;

namespace Procon.Core.Interfaces.Layer.Objects {
    using Procon.Net;
    public class LayerClient : TCPClient<LayerPacket> {

        /// <summary>
        /// Is this required anymore now that LayerPacketSerializer holds this data?
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Is this required anymore now that LayerPacketSerializer holds this data?
        /// </summary>
        public string Salt { get; set; }

        public LayerClient(string hostname, ushort port)
            : base(hostname, port) {
                this.PacketSerializer = new LayerPacketSerializer() {
                    Salt = this.Salt,
                    Password = this.Password
                };
        }

        public LayerClient(TcpClient client)
            : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.m_tcpClient = client;
            this.m_nwsStream = client.GetStream();
            this.ConnectionState = Net.ConnectionState.Ready;

            this.BeginRead();
            // setup reader.
        }

        /*
        protected override LayerPacket CreatePacket(byte[] packet) {
            LayerPacket returnPacket = new LayerPacket() {
                // The packet is incoming so it will contain details of it being encrypted or not.
                Password = this.Password,
                Salt = this.Salt
            };

            returnPacket.DecodePacket(packet);

            return returnPacket;
        }

        protected override UInt32 DecodePacketSize(byte[] packet) {
            return LayerPacket.DecodePacketSize(packet);
        }

        protected override UInt32 GetPacketHeaderSize() {
            return LayerPacket.INT_PACKET_HEADER_SIZE;
        }
        */
    }
}
