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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Layer.Objects {
    using Procon.Net;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;

    public class LayerPacket : Packet {

        // public static readonly UInt32 INT_PACKET_HEADER_SIZE = 6;

        public Context Context { get; set; }
        public LayerCommand Response { get; set; }
        public LayerCommand Request { get; set; }

        [JsonIgnore]
        public bool IsEncrypted { get; set; }

        [JsonIgnore]
        public bool IsCompressed { get; set; }
        /*
        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string Salt { get; set; }
        */
        public LayerPacket()
            : base(PacketOrigin.Client, true) {
                //this.Password = this.Salt = String.Empty;
        }
        /*
        public LayerPacket()
            : base() {
                this.Password = this.Salt = String.Empty;
        }
        
        public override byte[] EncodePacket() {
            //string message =  ;
            byte[] message = Encoding.UTF8.GetBytes(this.ToJson());

            byte header = 0x00;

            if (this.IsCompressed == true) {
                header += 0x1;

                message = GZipStream.CompressBuffer(message);
            }

            if (this.IsEncrypted == true && this.Password != null && this.Salt != null && this.Password.Length > 0 && this.Salt.Length > 0) {
                header += 0x2;

                message = Encryption.Encrypt(message, this.Password, this.Salt);
            }

            byte[] encodedPacket = new byte[LayerPacket.INT_PACKET_HEADER_SIZE + message.Length];
            encodedPacket[0] = header;

            BitConverter.GetBytes((UInt32)message.Length).CopyTo(encodedPacket, 1);
            message.CopyTo(encodedPacket, 5);
            //Encoding.UTF8.GetBytes(message + Convert.ToChar(0x00)).CopyTo(encodedPacket, 5);

            return encodedPacket;
        }

        public override void DecodePacket(byte[] packet) {
            UInt32 dataSize = LayerPacket.DecodePacketSize(packet) - LayerPacket.INT_PACKET_HEADER_SIZE;

            if (LayerPacket.INT_PACKET_HEADER_SIZE + dataSize <= packet.Length) {

                this.IsCompressed = Convert.ToBoolean(packet[0] & 0x1);
                this.IsEncrypted = Convert.ToBoolean(packet[0] & 0x2);

                byte[] message = packet.Skip(5).Take((int)dataSize).ToArray();


                if (this.IsEncrypted == true && this.Password != null && this.Salt != null && this.Password.Length > 0 && this.Salt.Length > 0) {
                    message = Encryption.Decrypt(message, this.Password, this.Salt);
                }

                if (this.IsCompressed == true) {
                    message = GZipStream.UncompressBuffer(message);
                }

                this.FromJson(Encoding.UTF8.GetString(message));

                //string message = Encoding.UTF8.GetString(packet, 5, (int)dataSize);

                //this.FromJson(message);
            }
        }

        public new static UInt32 DecodePacketSize(byte[] packet) {

            UInt32 packetSize = 0;

            if (packet.Length >= LayerPacket.INT_PACKET_HEADER_SIZE) {
                packetSize = BitConverter.ToUInt32(packet, 1) + LayerPacket.INT_PACKET_HEADER_SIZE;
            }

            return packetSize;
        }
        */
        #region JSON Serializer

        public void FromJson(string json) {
            LayerPacket packet = JsonConvert.DeserializeObject<LayerPacket>(
                json,
                new JsonSerializerSettings() {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            this.Request = packet.Request;
            this.Context = packet.Context;
            this.Response = packet.Response;
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings() {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
                }
            );
        }

        #endregion
    }
}
