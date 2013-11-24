using System;
using System.Linq;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite {

    public class FrostbitePacket : Packet {

        public static readonly string StringResponseOkay = "OK";

        public FrostbitePacket() : base() {
        }

        public FrostbitePacket(PacketOrigin origin, PacketType type, int? sequenceId, params string[] words) : base(origin, type) {
            this.RequestId = sequenceId;
            this.Words = new List<String>(words);
        }

        public FrostbitePacket(PacketOrigin origin, PacketType type, int? sequenceId, List<String> words) : base(origin, type) {
            this.RequestId = sequenceId;
            this.Words = words;
        }

        public override string ToDebugString() {

            string returnString = String.Empty;

            for (int i = 0; i < this.Words.Count; i++) {
                if (i > 0) {
                    returnString += " ";
                }

                returnString += String.Format("[{0}-{1}]", i, this.Words[i]);
            }

            return returnString;
        }

        public override string ToString() {
            return this.Words.Count > 0 ? String.Join(" ", this.Words.ToArray()) : String.Empty;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof (FrostbitePacket)) {
                return false;
            }
            return Equals((FrostbitePacket) obj);
        }

        public bool Equals(FrostbitePacket other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return other.Words.SequenceEqual(this.Words) && other.RequestId.Equals(this.RequestId);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.Words != null ? this.Words.GetHashCode() : 0)*397) ^ (this.RequestId.HasValue ? this.RequestId.Value.GetHashCode() : 0);
            }
        }
    }
}
