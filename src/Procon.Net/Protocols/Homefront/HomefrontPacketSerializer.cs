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

namespace Procon.Net.Protocols.Homefront {
    using Procon.Net.Utils;
    public class HomefrontPacketSerializer : PacketSerializer<HomefrontPacket> {

        public static readonly UInt32 INT_OUTGOING_PACKET_HEADER_SIZE = 2 + sizeof(int);

        public HomefrontPacketSerializer()
            : base() {
                this.PacketHeaderSize = 2 + sizeof(int) + 1;
        }

        protected string MessageTypeToString(MessageType messageType) {
            string returnMessageTypeString = String.Empty;

            switch (messageType) {
                case Homefront.MessageType.Connect:
                    returnMessageTypeString = "CC";
                    break;
                case Homefront.MessageType.ClientTransmission:
                    returnMessageTypeString = "CT";
                    break;
                case Homefront.MessageType.ClientDisconnect:
                    returnMessageTypeString = "CD";
                    break;
                case Homefront.MessageType.ClientPing:
                    returnMessageTypeString = "CP";
                    break;
                case Homefront.MessageType.ServerAnnounce:
                    returnMessageTypeString = "SA";
                    break;
                case Homefront.MessageType.ServerTransmission:
                    returnMessageTypeString = "ST";
                    break;
                case Homefront.MessageType.ServerDisconnect:
                    returnMessageTypeString = "SD";
                    break;
                case Homefront.MessageType.ServerResponse:
                    returnMessageTypeString = "SR";
                    break;
            }

            return returnMessageTypeString;
        }

        protected MessageType ToServerMessageType(string message) {

            MessageType returnType = Homefront.MessageType.Unknown;

            foreach (MessageType item in Enum.GetValues(typeof(MessageType))) {
                if (this.MessageTypeToString(item).Equals(message) == true) {
                    returnType = item;
                    break;
                }
            }

            return returnType;
        }

        public override byte[] Serialize(HomefrontPacket packet) {

            byte[] returnPacket = new byte[HomefrontPacketSerializer.INT_OUTGOING_PACKET_HEADER_SIZE + packet.Message.Length];

            Encoding.ASCII.GetBytes(this.MessageTypeToString(packet.MessageType)).CopyTo(returnPacket, 0);
            returnPacket[2] = (byte)((packet.Message.Length >> 24) & 255);
            returnPacket[3] = (byte)((packet.Message.Length >> 16) & 255);
            returnPacket[4] = (byte)((packet.Message.Length >> 8) & 255);
            returnPacket[5] = (byte)(packet.Message.Length & 255);

            //BitConverter.GetBytes(this.Message.Length).CopyTo(returnPacket, 2);
            Encoding.UTF8.GetBytes(packet.Message).CopyTo(returnPacket, HomefrontPacketSerializer.INT_OUTGOING_PACKET_HEADER_SIZE);

            return returnPacket;
        }

        public override HomefrontPacket Deserialize(byte[] packetData) {
            HomefrontPacket packet = new HomefrontPacket();

            packet.MessageType = this.ToServerMessageType(Encoding.ASCII.GetString(packetData, 0, 2)); // GetEncoding(1252)
            packet.ChannelType = (Homefront.ChannelType)packetData[2];
            UInt32 packetDataLength = this.ReadPacketSize(packetData) - this.PacketHeaderSize;
            packet.Message = Encoding.UTF8.GetString(packetData, (int)this.PacketHeaderSize, (int)packetDataLength);
            packet.MessageWords = packet.Message.Wordify();

            return packet;
        }

        public override uint ReadPacketSize(byte[] packetData) {
            UInt32 returnPacketSize = 0;

            if (packetData.Length >= this.PacketHeaderSize + 1) {

                returnPacketSize = (UInt32)(packetData[3] << 24 |
                                            packetData[4] << 16 |
                                            packetData[5] << 8 |
                                            packetData[6]);

                returnPacketSize += this.PacketHeaderSize;
            }

            return returnPacketSize;
        }

    }
}
