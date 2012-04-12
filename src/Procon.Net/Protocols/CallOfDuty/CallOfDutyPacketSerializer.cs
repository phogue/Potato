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

namespace Procon.Net.Protocols.CallOfDuty {
    public class CallOfDutyPacketSerializer : PacketSerializer<CallOfDutyPacket> {

        public CallOfDutyPacketSerializer()
            : base() {
                this.PacketHeaderSize = 2 + sizeof(int) + 1;
        }

        public override byte[] Serialize(CallOfDutyPacket packet) {

            byte[] encodedPacket = new byte[(7 + packet.Password.Length + packet.Message.Length)];
            encodedPacket[0] = encodedPacket[1] = encodedPacket[2] = encodedPacket[3] = 0xFF;
            encodedPacket[4] = 0x00;

            Encoding.ASCII.GetBytes(packet.Password).CopyTo(encodedPacket, 5);
            encodedPacket[5 + packet.Password.Length] = 0x20;
            Encoding.ASCII.GetBytes(packet.Message).CopyTo(encodedPacket, 6 + packet.Password.Length);
            encodedPacket[(6 + packet.Password.Length + packet.Message.Length)] = 0x00;

            return encodedPacket;
        }

        public override CallOfDutyPacket Deserialize(byte[] packetData) {
            CallOfDutyPacket packet = new CallOfDutyPacket() {
                IsResponse = true,
                Origin = PacketOrigin.Client
            };

            string[] packetStrings = Encoding.ASCII.GetString(packetData, 5, packetData.Length - 5).Split(new char[] { '\n' }, 2);

            if (packetStrings.Length == 2) {
                packet.Command = packetStrings[0];
                packet.Message = packetStrings[1];

                packet.IsEOP = (packet.Message.Length == 0 || (packet.Message.Length >= 2 && packet.Message[packet.Message.Length - 1] == '\n' && packet.Message[packet.Message.Length - 2] == '\n'));
            }

            return packet;
        }

        // Unused by CallOfDutyClient (UDP packet and not reliable)
        public override uint ReadPacketSize(byte[] packetData) {
            return 0;
        }
    }
}
