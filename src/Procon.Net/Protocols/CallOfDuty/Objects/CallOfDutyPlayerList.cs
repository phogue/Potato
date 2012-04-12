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
    public class CallOfDutyPlayerList : PlayerList {

        private static readonly Regex PlayerMatch = new Regex(@"^[ ]*(?<num>[\-0-9]+)[ ]*(?<score>[\-0-9]+)[ ]*(?<ping>[\-0-9]+)[ ]*(?<guid>[\-0-9]+)[ ]*(?<name>.+?)\^7?[ ]*(?<team>[\-0-9]+)[ ]*(?<lastmsg>[\-0-9]+)[ ]*(?<address>[0-9\\.]+?):[\-0-9]+?[ ]*(?<qport>[\-0-9]+)[ ]*(?<rate>[\-0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public CallOfDutyPlayerList Parse(string text) {

            this.Subset = new PlayerSubset() { Context = PlayerSubsetContext.All };

            //System.IO.File.WriteAllText("doutput/playerlist.txt", "");

            foreach (string line in text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                Match player = CallOfDutyPlayerList.PlayerMatch.Match(line);

                if (player.Success == true) {
                    this.Add(
                        new CallOfDutyPlayer() {
                            SlotID = uint.Parse(player.Groups["num"].Value),
                            GUID = player.Groups["guid"].Value,
                            Name = player.Groups["name"].Value,
                            Ping = uint.Parse(player.Groups["ping"].Value) == 999 ? 0 : uint.Parse(player.Groups["ping"].Value),
                            Score = int.Parse(player.Groups["score"].Value),
                            Team = CallOfDutyConverter.TeamIdToTeam(int.Parse(player.Groups["team"].Value)),
                            IP = player.Groups["address"].Value
                        }
                    );
                    //System.IO.File.AppendAllText("doutput/playerlist.txt", String.Format("PASS: {0}\r\n", line));
                }
                else {
                    //System.IO.File.AppendAllText("doutput/playerlist.txt", String.Format("FAIL: {0}\r\n", line));
                }
            }

            //foreach (Match player in CallOfDutyPlayerList.PlayerMatch.Matches(text)) {

            //}

            return this;
        }

    }
}
