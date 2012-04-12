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

namespace Procon.Net.Protocols.Frostbite {
    using System.Linq;

    public class FrostbitePacket : Packet {

        public static readonly string STRING_RESPONSE_OKAY = "OK";

        public List<string> Words { get; private set; }

        public UInt32? SequenceId { get; set; }

        public FrostbitePacket()
            : base() {
            this.Words = new List<string>();
        }

        public FrostbitePacket(PacketOrigin origin, bool isResponse, UInt32? sequenceId, params string[] words)
            : base(origin, isResponse) {
                this.SequenceId = sequenceId;
                this.Words = new List<string>(words);
        }

        public FrostbitePacket(PacketOrigin origin, bool isResponse, UInt32? sequenceId, List<string> words)
            : base(origin, isResponse) {
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
