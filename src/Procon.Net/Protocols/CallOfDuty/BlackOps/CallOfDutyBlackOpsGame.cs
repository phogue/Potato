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

namespace Procon.Net.Protocols.CallOfDuty.BlackOps {
    using Procon.Net.Attributes;
    using Procon.Net.Utils.HTTP;

    [Game(GameType = GameType.COD_BO)]
    public class CallOfDutyBlackOpsGame : CallOfDutyGame {

        public CallOfDutyBlackOpsGame(string hostName, ushort port) : base(hostName, port) {

        }

        protected override void AssignEvents() {
            base.AssignEvents();

            Request timeout = new Request("http://logs.gameservers.com/timeout");
            timeout.RequestComplete += new Request.RequestEventDelegate(timeout_RequestComplete);
            timeout.BeginRequest();
        }

        private void timeout_RequestComplete(Request sender) {
            string data = Encoding.ASCII.GetString(sender.CompleteFileData);

            short timeout = 0;

            if (short.TryParse(data, out timeout) == true) {
                this.LogFile.Interval = timeout;
            }

        }

    }
}
