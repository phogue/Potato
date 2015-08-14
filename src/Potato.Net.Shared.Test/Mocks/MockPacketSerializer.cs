#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Text;

namespace Potato.Net.Shared.Test.Mocks {
    public class MockPacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        public MockPacketSerializer() {
            PacketHeaderSize = 16;
        }

        public IPacketWrapper Deserialize(byte[] packetData) {
            var length = BitConverter.ToInt32(packetData, 12);

            var wrapper = new MockPacket {
                Packet = {
                    Origin = (PacketOrigin)BitConverter.ToInt32(packetData, 0),
                    Type = (PacketType)BitConverter.ToInt32(packetData, 4),
                    RequestId = BitConverter.ToInt32(packetData, 8)
                },
                Text = Encoding.ASCII.GetString(packetData, (int)PacketHeaderSize, length)
            };

            wrapper.Packet.DebugText = string.Format("{0} {1} {2} {3}", wrapper.Packet.Origin, wrapper.Packet.Type, wrapper.Packet.RequestId, wrapper.Packet.Text);

            return wrapper;
        }

        public byte[] Serialize(IPacketWrapper wrapper) {
            var mockPacket = wrapper as MockPacket;
            byte[] serialized = null;

            if (mockPacket != null) {
                if (mockPacket.Packet.RequestId == null)
                    mockPacket.Packet.RequestId = 111;

                serialized = new byte[PacketHeaderSize + mockPacket.Text.Length];

                BitConverter.GetBytes((int)mockPacket.Packet.Origin).CopyTo(serialized, 0);
                BitConverter.GetBytes((int)mockPacket.Packet.Type).CopyTo(serialized, 4);
                BitConverter.GetBytes(mockPacket.Packet.RequestId.Value).CopyTo(serialized, 8);
                BitConverter.GetBytes(mockPacket.Text.Length).CopyTo(serialized, 12);
                Encoding.ASCII.GetBytes(mockPacket.Text).CopyTo(serialized, PacketHeaderSize);

                mockPacket.Packet.DebugText = string.Format("{0} {1} {2} {3}", mockPacket.Packet.Origin, mockPacket.Packet.Type, mockPacket.Packet.RequestId, mockPacket.Packet.Text);
            }

            return serialized;
        }

        public long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 12) + PacketHeaderSize;
            }

            return length;
        }
    }
}
