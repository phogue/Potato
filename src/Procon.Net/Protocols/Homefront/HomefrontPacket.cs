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
using System.Text;

namespace Procon.Net.Protocols.Homefront {
    using Procon.Net.Utils;

    public class HomefrontPacket : Packet {

        public ChannelType ChannelType { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }
        public List<string> MessageWords { get; set; }

        public HomefrontPacket()
            : base() {
        }

        public HomefrontPacket(PacketOrigin origin, bool isResponse, MessageType messageType, string message)
            : base(origin, isResponse) {

                this.Message = message;
                this.MessageType = messageType;
                this.MessageWords = this.Message.Wordify();
        }

        public override string ToString() {
            return this.Message;
        }

        public override string ToDebugString() {
            return String.Format("[MT: {0}], [CT: {1}], [MSG: {2}]", this.MessageType, this.ChannelType, this.Message);
        }
    }
}
