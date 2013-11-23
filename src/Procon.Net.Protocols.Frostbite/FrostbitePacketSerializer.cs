using System;
using System.Text;

namespace Procon.Net.Protocols.Frostbite {
    public class FrostbitePacketSerializer : PacketSerializer<FrostbitePacket> {

        public FrostbitePacketSerializer()
            : base() {
                this.PacketHeaderSize = 12;
        }

        public override byte[] Serialize(FrostbitePacket packet) {

            // Construct the header uint32
            UInt32 header = packet.RequestId != null ? (UInt32)packet.RequestId & 0x3fffffff : 0x3fffffff;

            if (packet.Origin == PacketOrigin.Server) {
                header |= 0x80000000;
            }

            if (packet.Type == PacketType.Response) {
                header |= 0x40000000;
            }

            // Construct the remaining packet headers
            UInt32 packetSize = this.PacketHeaderSize;
            UInt32 wordCount = Convert.ToUInt32(packet.Words.Count);

            // Encode each word (WordLength, Word Bytes, Null Byte)
            byte[] encodedWords = new byte[] { };
            foreach (string word in packet.Words) {

                string convertedWord = word;

                // Truncate words over 64 kbs (though the string is Unicode it gets converted below so this does make sense)
                if (convertedWord.Length > UInt16.MaxValue - 1) {
                    convertedWord = convertedWord.Substring(0, UInt16.MaxValue - 1);
                }

                byte[] appendEncodedWords = new byte[encodedWords.Length + convertedWord.Length + 5];

                encodedWords.CopyTo(appendEncodedWords, 0);

                BitConverter.GetBytes(convertedWord.Length).CopyTo(appendEncodedWords, encodedWords.Length);
                Encoding.GetEncoding(1252).GetBytes(convertedWord + Convert.ToChar(0x00)).CopyTo(appendEncodedWords, encodedWords.Length + 4);

                encodedWords = appendEncodedWords;
            }

            // Get the full size of the packet.
            packetSize += Convert.ToUInt32(encodedWords.Length);

            // Now compile the whole packet.
            byte[] returnPacket = new byte[packetSize];

            BitConverter.GetBytes(header).CopyTo(returnPacket, 0);
            BitConverter.GetBytes(packetSize).CopyTo(returnPacket, 4);
            BitConverter.GetBytes(wordCount).CopyTo(returnPacket, 8);
            encodedWords.CopyTo(returnPacket, this.PacketHeaderSize);

            return returnPacket;
        }

        public override FrostbitePacket Deserialize(byte[] packetData) {

            FrostbitePacket packet = new FrostbitePacket();

            int header = BitConverter.ToInt32(packetData, 0);
            //this.PacketSize = BitConverter.ToInt32(packet, 4);
            int wordsTotal = BitConverter.ToInt32(packetData, 8);

            packet.Origin = Convert.ToBoolean(header & 0x80000000) == true ? PacketOrigin.Server : PacketOrigin.Client;

            packet.Type = Convert.ToBoolean(header & 0x40000000) == false ? PacketType.Request : PacketType.Response;
            packet.RequestId = header & 0x3fffffff;

            int iWordOffset = 0;

            for (UInt32 wordCount = 0; wordCount < wordsTotal; wordCount++) {
                UInt32 wordLength = BitConverter.ToUInt32(packetData, (int)this.PacketHeaderSize + iWordOffset);

                packet.Words.Add(Encoding.GetEncoding(1252).GetString(packetData, (int)this.PacketHeaderSize + iWordOffset + 4, (int)wordLength));

                iWordOffset += Convert.ToInt32(wordLength) + 5; // WordLength + WordSize + NullByte
            }

            return packet;
        }

        public override long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 4);
            }

            return length;
        }
    }
}
