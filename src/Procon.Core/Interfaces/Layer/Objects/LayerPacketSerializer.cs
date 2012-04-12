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
using Ionic.Zlib;

namespace Procon.Core.Interfaces.Layer.Objects {
    using Procon.Net;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;

    public class LayerPacketSerializer : PacketSerializer<LayerPacket> {

        public string Password { get; set; }
        public string Salt { get; set; }

        public LayerPacketSerializer() : base() {
            this.PacketHeaderSize = 6;
        }

        public override LayerPacket Deserialize(byte[] packetData) {
            LayerPacket packet = new LayerPacket();

            UInt32 dataSize = this.ReadPacketSize(packetData) - this.PacketHeaderSize;

            if (this.PacketHeaderSize + dataSize <= packetData.Length) {

                packet.IsCompressed = Convert.ToBoolean(packetData[0] & 0x1);
                packet.IsEncrypted = Convert.ToBoolean(packetData[0] & 0x2);

                byte[] message = packetData.Skip(5).Take((int)dataSize).ToArray();


                if (packet.IsEncrypted == true && this.Password != null && this.Salt != null && this.Password.Length > 0 && this.Salt.Length > 0) {
                    message = Encryption.Decrypt(message, this.Password, this.Salt);
                }

                if (packet.IsCompressed == true) {
                    message = GZipStream.UncompressBuffer(message);
                }

                packet.FromJson(Encoding.UTF8.GetString(message));
            }

            return packet;
        }

        public override byte[] Serialize(LayerPacket packet) {
            //string message =  ;
            byte[] message = Encoding.UTF8.GetBytes(packet.ToJson());

            byte header = 0x00;

            if (packet.IsCompressed == true) {
                header += 0x1;

                message = GZipStream.CompressBuffer(message);
            }

            if (packet.IsEncrypted == true && this.Password != null && this.Salt != null && this.Password.Length > 0 && this.Salt.Length > 0) {
                header += 0x2;

                message = Encryption.Encrypt(message, this.Password, this.Salt);
            }

            byte[] encodedPacket = new byte[this.PacketHeaderSize + message.Length];
            encodedPacket[0] = header;

            BitConverter.GetBytes((UInt32)message.Length).CopyTo(encodedPacket, 1);
            message.CopyTo(encodedPacket, 5);
            //Encoding.UTF8.GetBytes(message + Convert.ToChar(0x00)).CopyTo(encodedPacket, 5);

            return encodedPacket;
        }

        public override uint ReadPacketSize(byte[] packetData) {

            UInt32 packetSize = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                packetSize = BitConverter.ToUInt32(packetData, 1) + this.PacketHeaderSize;
            }

            return packetSize;
        }

    }
}
