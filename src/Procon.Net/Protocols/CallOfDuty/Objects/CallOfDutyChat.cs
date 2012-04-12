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
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.CallOfDuty.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class CallOfDutyChat : Chat, ICallOfDutyObject {

        public CallOfDutyChat(): base() {

        }

        public ICallOfDutyObject Parse(Match match) {

            this.Author = new Player() {
                Name = match.Groups["Name"].Value,
                GUID = match.Groups["GUID"].Value
            };

            if (String.Compare(match.Groups["Command"].Value, "say", true) == 0) {
                this.Subset = new PlayerSubset() {
                    Context = PlayerSubsetContext.All
                };
            }
            else {
                this.Subset = new PlayerSubset() {
                    Context = PlayerSubsetContext.Team
                };
            }

            this.Text = match.Groups["Text"].Value.Replace("\r", "");

            this.Origin = ChatOrigin.Player;

            return this;
        }
    }
}
