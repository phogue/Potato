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
    public class CallOfDutyPacket : Packet {

        public DateTime SentAt { get; set; }
        public int SentTimes { get; set; }

        public string Command { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }

        // Is End Of Packet (has \n\n at the end)
        public bool IsEOP { get; set; }

        public CallOfDutyPacket()
            : base() {

        }

        public CallOfDutyPacket(PacketOrigin origin, bool isResponse, string password, string message)
            : base(origin, isResponse) {

                this.Password = password;
                this.Message = message;
        }

        public CallOfDutyPacket Prepare() {
            this.SentAt = DateTime.Now;
            this.SentTimes++;

            return this;
        }

        public CallOfDutyPacket Combine(CallOfDutyPacket b) {

            if (String.Compare(this.Command, b.Command, true) == 0) {
                this.Message = this.Message + b.Message;
                this.IsEOP = b.IsEOP;
            }

            return this;
        }

        public override string ToString() {
            return this.Message;
        }

    }
}
