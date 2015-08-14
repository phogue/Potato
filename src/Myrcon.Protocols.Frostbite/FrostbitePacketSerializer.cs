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
using System.Linq;
using System.Text;
using Potato.Net.Shared;

namespace Myrcon.Protocols.Frostbite {
    public class FrostbitePacketSerializer : IPacketSerializer {
        /// <summary>
        /// The minimum packet size requires to be passed into the packet serializer. Anything smaller
        /// and it the full header of a packet wouldn't be available, therefore we wouldn't know
        /// how many bytes the full packet is.
        /// </summary>
        public uint PacketHeaderSize { get; set; }

        public FrostbitePacketSerializer()
            : base() {
                PacketHeaderSize = 12;
        }

        /// <summary>
        /// Serializes a packet into an array of bytes to send to the server.
        /// </summary>
        /// <param name="wrapper">The packe to serialize</param>
        /// <returns>An array of bytes to send to the server.</returns>
        public byte[] Serialize(IPacketWrapper wrapper) {
            var frostbiteWrapper = wrapper as FrostbitePacket;
            byte[] serialized = null;

            if (frostbiteWrapper != null) {
                // Construct the header uint32
                var header = frostbiteWrapper.Packet.RequestId != null ? (uint)frostbiteWrapper.Packet.RequestId & 0x3fffffff : 0x3fffffff;

                if (frostbiteWrapper.Packet.Origin == PacketOrigin.Server) {
                    header |= 0x80000000;
                }

                if (frostbiteWrapper.Packet.Type == PacketType.Response) {
                    header |= 0x40000000;
                }

                // Construct the remaining packet headers
                var packetSize = PacketHeaderSize;
                var wordCount = Convert.ToUInt32(frostbiteWrapper.Packet.Words.Count);

                // Encode each word (WordLength, Word Bytes, Null Byte)
                var encodedWords = new byte[] { };
                foreach (var word in frostbiteWrapper.Packet.Words) {

                    var convertedWord = word;

                    // Truncate words over 64 kbs (though the string is Unicode it gets converted below so this does make sense)
                    if (convertedWord.Length > ushort.MaxValue - 1) {
                        convertedWord = convertedWord.Substring(0, ushort.MaxValue - 1);
                    }

                    var appendEncodedWords = new byte[encodedWords.Length + convertedWord.Length + 5];

                    encodedWords.CopyTo(appendEncodedWords, 0);

                    BitConverter.GetBytes(convertedWord.Length).CopyTo(appendEncodedWords, encodedWords.Length);
                    Encoding.GetEncoding(1252).GetBytes(convertedWord + Convert.ToChar(0x00)).CopyTo(appendEncodedWords, encodedWords.Length + 4);

                    encodedWords = appendEncodedWords;
                }

                // Get the full size of the packet.
                packetSize += Convert.ToUInt32(encodedWords.Length);

                // Now compile the whole packet.
                serialized = new byte[packetSize];

                BitConverter.GetBytes(header).CopyTo(serialized, 0);
                BitConverter.GetBytes(packetSize).CopyTo(serialized, 4);
                BitConverter.GetBytes(wordCount).CopyTo(serialized, 8);
                encodedWords.CopyTo(serialized, PacketHeaderSize);

                wrapper.Packet.Data = serialized;
                wrapper.Packet.Text = string.Join(" ", wrapper.Packet.Words);
                wrapper.Packet.DebugText = string.Join(" ", wrapper.Packet.Words.Select((word, index) => string.Format("[{0}-{1}]", index, word)));
            }

            return serialized;
        }

        /// <summary>
        /// Deserializes an array of bytes into a Packet of type P
        /// </summary>
        /// <param name="packetData">The array to deserialize to a packet. Must be exact length of bytes.</param>
        /// <returns>A new packet with data extracted from packetDate</returns>
        public IPacketWrapper Deserialize(byte[] packetData) {

            var wrapper = new FrostbitePacket();

            var header = BitConverter.ToInt32(packetData, 0);
            //this.PacketSize = BitConverter.ToInt32(packet, 4);
            var wordsTotal = BitConverter.ToInt32(packetData, 8);

            wrapper.Packet.Origin = Convert.ToBoolean(header & 0x80000000) == true ? PacketOrigin.Server : PacketOrigin.Client;

            wrapper.Packet.Type = Convert.ToBoolean(header & 0x40000000) == false ? PacketType.Request : PacketType.Response;
            wrapper.Packet.RequestId = header & 0x3fffffff;

            var wordOffset = 0;

            for (uint wordCount = 0; wordCount < wordsTotal; wordCount++) {
                var wordLength = BitConverter.ToUInt32(packetData, (int)PacketHeaderSize + wordOffset);

                wrapper.Packet.Words.Add(Encoding.GetEncoding(1252).GetString(packetData, (int)PacketHeaderSize + wordOffset + 4, (int)wordLength));

                wordOffset += Convert.ToInt32(wordLength) + 5; // WordLength + WordSize + NullByte
            }

            wrapper.Packet.Data = packetData;
            wrapper.Packet.Text = string.Join(" ", wrapper.Packet.Words);
            wrapper.Packet.DebugText = string.Join(" ", wrapper.Packet.Words.Select((word, index) => string.Format("[{0}-{1}]", index, word)));

            return wrapper;
        }

        /// <summary>
        /// Fetches the full packet size by reading the header of a packet.
        /// </summary>
        /// <param name="packetData">The possibly incomplete packet data, or as much data as we have recieved from the server.</param>
        /// <returns>The total size, in bytes, that is requires for the header + data to be deserialized.</returns>
        public long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 4);
            }

            return length;
        }
    }
}
