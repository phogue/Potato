// Copyright 2011 Geoffrey 'Phogue' Green
// 
// Altered by Cameron 'Imisnew2' Gunnin
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
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils;

namespace Procon.Net.Protocols.Frostbite.Objects
{
    [Serializable]
    public class FrostbitePlayer : Player
    {
        public FrostbitePlayer() { }

        public FrostbitePlayer(List<String> parameters, List<String> variables)
        {
            // Make sure the parameter's passed in are correct.
            if (parameters.Count != variables.Count) return;

            // Parse and normalize the parameters.
            int  intValue  = 0;
            uint uintValue = 0;
            for (int i = 0; i < parameters.Count; i++) {
                switch (parameters[i].ToLower())
                {
                    case "guid":
                        UID = variables[i];
                        break;
                    case "name":
                        Name = variables[i];
                        break;
                    case "clantag":
                        ClanTag = variables[i];
                        break;
                    case "teamid":
                        if (int.TryParse(variables[i], out intValue))
                            Team = FrostbiteConverter.TeamIdToTeam(intValue);
                        break;
                    case "squadid":
                        if (int.TryParse(variables[i], out intValue))
                            Squad = FrostbiteConverter.SquadIdToSquad(intValue);
                        break;
                    case "kills":
                        if (int.TryParse(variables[i], out intValue))
                            Kills = intValue;
                        break;
                    case "deaths":
                        if (int.TryParse(variables[i], out intValue))
                            Deaths = intValue;
                        break;
                    case "score":
                        if (int.TryParse(variables[i], out intValue))
                            Score = intValue;
                        break;
                    case "ping":
                        if (uint.TryParse(variables[i], out uintValue))
                            Ping = uintValue;
                        break;
                    default:
                        DataAddSet("frostbite." + parameters[i], variables[i]);
                        break;
                }
            }
        }


        /// <summary>A Game-specific Unique Identifier.</summary>
        public override string UID
        {
            get { return GUID; }
            set { GUID = value; }
        }
    }
}
