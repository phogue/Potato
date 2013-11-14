using System;
using System.Linq;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite {

    public class FrostbitePacket : Packet {

        public static readonly string StringResponseOkay = "OK";

        /// <summary>
        /// A list of words to send to the server or recieved from the server that make up
        /// a frostbite command/event.
        /// </summary>
        public List<string> Words { get; private set; }

        /// <summary>
        /// The sequence id for this command/event
        /// </summary>
        public UInt32? SequenceId { get; set; }

        public FrostbitePacket()
            : base() {
            this.Words = new List<string>();
        }

        public FrostbitePacket(PacketOrigin origin, PacketType type, UInt32? sequenceId, params string[] words)
            : base(origin, type) {
                this.SequenceId = sequenceId;
                this.Words = new List<string>(words);
        }

        public FrostbitePacket(PacketOrigin origin, PacketType type, UInt32? sequenceId, List<string> words)
            : base(origin, type) {
            this.SequenceId = sequenceId;
            this.Words = words;
        }

        protected override void NullPacket() {
            base.NullPacket();
            this.Words = new List<string>();
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
            return other.Words.SequenceEqual(this.Words) && other.SequenceId.Equals(this.SequenceId);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.Words != null ? this.Words.GetHashCode() : 0)*397) ^ (this.SequenceId.HasValue ? this.SequenceId.Value.GetHashCode() : 0);
            }
        }
    }
}
