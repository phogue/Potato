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

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public static class FrostbiteConverter {

        public static string BoolToString(bool b) {
            return b == true ? "true" : "false";
        }

        public static Team TeamIdToTeam(int teamId) {
            Team result = Team.None;

            switch (teamId) {
                case 0:
                    result = Team.NeutralOrSpectator;
                    break;
                case 1:
                    result = Team.Team1;
                    break;
                case 2:
                    result = Team.Team2;
                    break;
                case 3:
                    result = Team.Team3;
                    break;
                case 4:
                    result = Team.Team4;
                    break;
            }

            return result;
        }

        public static int TeamToTeamId(Team team) {

            int result = 0;

            switch (team) {
                case Team.NeutralOrSpectator:
                    result = 0;
                    break;
                case Team.Team1:
                    result = 1;
                    break;
                case Team.Team2:
                    result = 2;
                    break;
                case Team.Team3:
                    result = 3;
                    break;
                case Team.Team4:
                    result = 4;
                    break;
            }

            return result;
        }

        public static Squad SquadIdToSquad(int squadId) {
            Squad result = Squad.None;

            switch (squadId) {
                case 0:
                    result = Squad.None;
                    break;
                case 1:
                    result = Squad.Squad1;
                    break;
                case 2:
                    result = Squad.Squad2;
                    break;
                case 3:
                    result = Squad.Squad3;
                    break;
                case 4:
                    result = Squad.Squad4;
                    break;
                case 5:
                    result = Squad.Squad5;
                    break;
                case 6:
                    result = Squad.Squad6;
                    break;
                case 7:
                    result = Squad.Squad7;
                    break;
                case 8:
                    result = Squad.Squad8;
                    break;
            }

            return result;
        }

        public static int SquadToSquadId(Squad Squad) {

            int result = 0;

            switch (Squad) {
                case Squad.None:
                    result = 0;
                    break;
                case Squad.Squad1:
                    result = 1;
                    break;
                case Squad.Squad2:
                    result = 2;
                    break;
                case Squad.Squad3:
                    result = 3;
                    break;
                case Squad.Squad4:
                    result = 4;
                    break;
                case Squad.Squad5:
                    result = 5;
                    break;
                case Squad.Squad6:
                    result = 6;
                    break;
                case Squad.Squad7:
                    result = 7;
                    break;
                case Squad.Squad8:
                    result = 8;
                    break;
            }

            return result;
        }

    }
}
