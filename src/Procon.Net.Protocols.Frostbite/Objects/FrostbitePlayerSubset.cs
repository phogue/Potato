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

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbitePlayerSubset : PlayerSubset {

        public FrostbitePlayerSubset() : base() {

        }

        private PlayerSubsetContext GetSubsetContext(string context) {

            PlayerSubsetContext result = PlayerSubsetContext.All;

            try {
                result = (PlayerSubsetContext)Enum.Parse(typeof(PlayerSubsetContext), context, true);
            }
            catch (Exception) { }

            return result;
        }

        public FrostbitePlayerSubset Parse(List<string> words) {
            
            if (words.Count >= 1) {

                this.Context = this.GetSubsetContext(words[0]);

                if (words.Count >= 2) {

                    int parsedTeamId = 0, parsedSquadId = 0;

                    if (this.Context == PlayerSubsetContext.Player) {
                        this.Player = new Player() { Name = words[1] };
                    }
                    else if (this.Context == PlayerSubsetContext.Team && int.TryParse(words[1], out parsedTeamId) == true) {
                        this.Team = FrostbiteConverter.TeamIdToTeam(parsedTeamId);
                    }
                    else if (words.Count >= 3) {
                        if (this.Context == PlayerSubsetContext.Squad && int.TryParse(words[1], out parsedTeamId) == true && int.TryParse(words[2], out parsedSquadId) == true) {
                            this.Team = FrostbiteConverter.TeamIdToTeam(parsedTeamId);
                            this.Squad = FrostbiteConverter.SquadIdToSquad(parsedSquadId);
                        }
                    }
                }
            }

            return this;
        }

    }
}
