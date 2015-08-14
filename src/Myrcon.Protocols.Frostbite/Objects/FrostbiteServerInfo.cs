#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public class FrostbiteServerInfo : NetworkModel {

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
            ServerName = string.Empty;
            Map = string.Empty;
            GameMode = string.Empty;
            ConnectionState = string.Empty;

            PlayerCount = 0;
            MaxPlayerCount = 0;
            CurrentRound = 0;
            TotalRounds = 0;
        }

        public FrostbiteServerInfo Parse(List<string> words, List<string> parameters) {

            RoundTime = ServerUptime = -1;

            for (int paramCount = 0, varCount = 0; paramCount < parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (parameters[paramCount]) {
                    case "TeamScores":

                        var scoresCount = 0;

                        if (int.TryParse(words[varCount], out scoresCount) == true) {
                            scoresCount++;

                            TeamScores = new TeamScoreList().Parse(words.GetRange(varCount, scoresCount));

                            varCount += scoresCount;
                        }
                        else {
                            varCount--;
                        }

                        break;
                    case "GameMod":

                        if (Enum.IsDefined(typeof(GameMods), words[varCount]) == true) {
                            GameMod = (GameMods)Enum.Parse(typeof(GameMods), words[varCount]);
                        }

                        break;
                    default:
                        PropertyInfo property = null;
                        if ((property = GetType().GetProperty(parameters[paramCount])) != null) {

                            try {
                                var value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(words[varCount]);

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
