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
using System.Net;

namespace Procon.Net {
    using Procon.Net.Utils;

    [Serializable]
    public abstract class Packet {

        public DateTime Created { get; set; }
        public PacketOrigin Origin { get; set; }
        public bool IsResponse { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public Packet() {

        }

        // Used if we have recieved a packet and need it decoded..
        /*
        public Packet(byte[] packet) {
            this.NullPacket();
            this.DecodePacket(packet);
            this.Created = DateTime.Now;
        }
        */
        // Used if we'll be using EncodePacket to send to the server.
        public Packet(PacketOrigin origin, bool isResponse) {
            this.Origin = origin;
            this.IsResponse = isResponse;
//            this.SequenceId = sequenceId;
            this.Created = DateTime.Now;
        }

        protected virtual void NullPacket() {
            this.Origin = PacketOrigin.Client;
            this.IsResponse = false;
//            this.SequenceId = 0;
        }

        /*
        public abstract byte[] EncodePacket();

        public abstract void DecodePacket(byte[] packet);

        public static UInt32 DecodePacketSize(byte[] packet) {
            return 0;
        }
        */
        public virtual string ToDebugString() {
            return base.ToString();
        }

        public override string ToString() {
            return base.ToString();
        }
    }
}
