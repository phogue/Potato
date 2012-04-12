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

namespace Procon.Net.Protocols.Frostbite {
    public class FrostbitePacketSerializer : PacketSerializer<FrostbitePacket> {

        public FrostbitePacketSerializer()
            : base() {
                this.PacketHeaderSize = 12;
        }

        public override byte[] Serialize(FrostbitePacket packet) {

            // Construct the header uint32
            UInt32 ui32Header = (UInt32)packet.SequenceId & 0x3fffffff;

            if (packet.Origin == PacketOrigin.Server) {
                ui32Header |= 0x80000000;
            }

            if (packet.IsResponse == true) {
                ui32Header |= 0x40000000;
            }

            // Construct the remaining packet headers
            UInt32 packetSize = this.PacketHeaderSize;
            UInt32 wordCount = Convert.ToUInt32(packet.Words.Count);

            // Encode each word (WordLength, Word Bytes, Null Byte)
            byte[] a_encodedWords = new byte[] { };
            foreach (string word in packet.Words) {

                string strWord = word;

                // Truncate words over 64 kbs (though the string is Unicode it gets converted below so this does make sense)
                if (strWord.Length > UInt16.MaxValue - 1) {
                    strWord = strWord.Substring(0, UInt16.MaxValue - 1);
                }

                byte[] a_bAppendEncodedWords = new byte[a_encodedWords.Length + strWord.Length + 5];

                a_encodedWords.CopyTo(a_bAppendEncodedWords, 0);

                BitConverter.GetBytes(strWord.Length).CopyTo(a_bAppendEncodedWords, a_encodedWords.Length);
                Encoding.GetEncoding(1252).GetBytes(strWord + Convert.ToChar(0x00)).CopyTo(a_bAppendEncodedWords, a_encodedWords.Length + 4);

                a_encodedWords = a_bAppendEncodedWords;
            }

            // Get the full size of the packet.
            packetSize += Convert.ToUInt32(a_encodedWords.Length);

            // Now compile the whole packet.
            byte[] returnPacket = new byte[packetSize];

            BitConverter.GetBytes(ui32Header).CopyTo(returnPacket, 0);
            BitConverter.GetBytes(packetSize).CopyTo(returnPacket, 4);
            BitConverter.GetBytes(wordCount).CopyTo(returnPacket, 8);
            a_encodedWords.CopyTo(returnPacket, this.PacketHeaderSize);

            return returnPacket;
        }

        public override FrostbitePacket Deserialize(byte[] packetData) {

            FrostbitePacket packet = new FrostbitePacket();

            UInt32 ui32Header = BitConverter.ToUInt32(packetData, 0);
            //this.PacketSize = BitConverter.ToUInt32(packet, 4);
            UInt32 wordsTotal = BitConverter.ToUInt32(packetData, 8);

            if (Convert.ToBoolean(ui32Header & 0x80000000) == true) {
                packet.Origin = PacketOrigin.Server;
            }
            else {
                packet.Origin = PacketOrigin.Client;
            }

            packet.IsResponse = Convert.ToBoolean(ui32Header & 0x40000000);
            packet.SequenceId = ui32Header & 0x3fffffff;

            int iWordOffset = 0;

            for (UInt32 wordCount = 0; wordCount < wordsTotal; wordCount++) {
                UInt32 wordLength = BitConverter.ToUInt32(packetData, (int)this.PacketHeaderSize + iWordOffset);

                packet.Words.Add(Encoding.GetEncoding(1252).GetString(packetData, (int)this.PacketHeaderSize + iWordOffset + 4, (int)wordLength));

                iWordOffset += Convert.ToInt32(wordLength) + 5; // WordLength + WordSize + NullByte
            }

            return packet;
        }

        public override uint ReadPacketSize(byte[] packetData) {
            UInt32 returnPacketSize = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                returnPacketSize = BitConverter.ToUInt32(packetData, 4);
            }

            return returnPacketSize;
        }
    }
}
