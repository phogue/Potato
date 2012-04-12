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
using System.Reflection;
using System.ComponentModel;

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbiteServerInfo : ProtocolObject {

        public GameMods GameMod { get; set; }

        public TeamScoreList TeamScores { get; set; }

        public string ServerName { get; set; }

        public string Map { get; set; }

        public string GameMode { get; set; }

        public string ConnectionState { get; set; }

        public int PlayerCount { get; set; }

        public int MaxPlayerCount { get; set; }

        public int CurrentRound { get; set; }

        public int TotalRounds { get; set; }

        // R27
        public bool Ranked { get; set; }

        public bool PunkBuster { get; set; }

        public bool Passworded { get; set; }

        public int ServerUptime { get; set; }

        public int RoundTime { get; set; }

        public int Mappack { get; set; }

        public FrostbiteServerInfo()
            : base() {
            this.ServerName = String.Empty;
            this.Map = String.Empty;
            this.GameMode = String.Empty;
            this.ConnectionState = String.Empty;

            this.PlayerCount = 0;
            this.MaxPlayerCount = 0;
            this.CurrentRound = 0;
            this.TotalRounds = 0;
        }


        public FrostbiteServerInfo Parse(List<string> words, List<string> parameters) {

            this.RoundTime = this.ServerUptime = -1;

            for (int paramCount = 0, varCount = 0; paramCount < parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (parameters[paramCount]) {
                    case "TeamScores":

                        int scoresCount = 0;

                        if (int.TryParse(words[varCount], out scoresCount) == true) {
                            scoresCount++;

                            this.TeamScores = new TeamScoreList().Parse(words.GetRange(varCount, scoresCount));

                            varCount += scoresCount;
                        }
                        else {
                            varCount--;
                        }

                        break;
                    case "GameMod":

                        if (Enum.IsDefined(typeof(GameMods), words[varCount]) == true) {
                            this.GameMod = (GameMods)Enum.Parse(typeof(GameMods), words[varCount]);
                        }

                        break;
                    default:
                        PropertyInfo property = null;
                        if ((property = this.GetType().GetProperty(parameters[paramCount])) != null) {

                            try {
                                object value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(words[varCount]);

                                if (value != null) {
                                    property.SetValue(this, value, null);
                                }

                            }
                            catch (Exception) { }
                        }

                        break;
                }
            }

            return this;
        }

    }
}
