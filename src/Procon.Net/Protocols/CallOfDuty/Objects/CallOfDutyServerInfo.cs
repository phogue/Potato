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
using System.Reflection;
using System.ComponentModel;

namespace Procon.Net.Protocols.CallOfDuty.Objects {
    public class CallOfDutyServerInfo : Dictionary<string, string> {

        private static readonly Regex ServerInfoMatch = new Regex(@"^(?<key>.*?)[ \t]+(?<value>.*?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        public static List<string> Parameters = new List<string>() {
            "mapname",
            "sv_hostname",
            "g_gametype",
            "scr_team_fftype",
            "sv_maxclients",
            "sv_pure",
            "sv_ranked",
            "sv_security", // Note: if another variable is affixed to both games this method
            "sv_voice"  // will need to be split into MoHClient and BFBC2Client.
        };

        public string mapname { get; set; }

        public string sv_hostname { get; set; }

        public string g_gametype { get; set; }

        public int scr_team_fftype { get; set; }
        public int sv_maxclients { get; set; }

        public int sv_pure { get; set; }
        public int sv_ranked { get; set; }
        public int sv_security { get; set; }
        public int sv_voice { get; set; }

        public CallOfDutyServerInfo() {
            this.mapname = String.Empty;
            this.sv_hostname = String.Empty;
            this.g_gametype = String.Empty;
        }


        public CallOfDutyServerInfo Parse(string text) {

            //for (int paramCount = 0, varCount = 0; paramCount < CallOfDutyServerInfo.Parameters.Count && varCount < words.Count; paramCount++, varCount++) {
            foreach (Match match in CallOfDutyServerInfo.ServerInfoMatch.Matches(text)) {
                PropertyInfo property = null;
                if ((property = this.GetType().GetProperty(match.Groups["key"].Value)) != null) {
                    try {
                        object value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(match.Groups["value"].Value);

                        if (value != null) {
                            property.SetValue(this, value, null);
                        }

                    }
                    catch (Exception) { }
                }
            }

            this.sv_hostname = this.sv_hostname.Replace("^?", "");

            return this;
        }

    }
}
