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
using System.Globalization;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {

    public static class FrostbitePlayer {
        
        public static PlayerModel Parse(IList<String> parameters, IList<String> variables) {
            PlayerModel player = new PlayerModel();

            // Make sure the parameter's passed in are correct.
            if (parameters.Count == variables.Count) {
                // Parse and normalize the parameters.
                for (int i = 0; i < parameters.Count; i++) {
                    int intValue = 0;
                    switch (parameters[i].ToLower()) {
                        case "guid":
                            player.Uid = variables[i];
                            break;
                        case "name":
                            player.Name = variables[i];
                            break;
                        case "clantag":
                            player.ClanTag = variables[i];
                            break;
                        case "teamid":
                            if (int.TryParse(variables[i], out intValue)) {
                                player.Groups.Add(new GroupModel() {
                                    Type = GroupModel.Team,
                                    Uid = intValue.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                            break;
                        case "squadid":
                            if (int.TryParse(variables[i], out intValue)) {
                                player.Groups.Add(new GroupModel() {
                                    Type = GroupModel.Squad,
                                    Uid = intValue.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                            break;
                        case "kills":
                            if (int.TryParse(variables[i], out intValue))
                                player.Kills = intValue;
                            break;
                        case "deaths":
                            if (int.TryParse(variables[i], out intValue))
                                player.Deaths = intValue;
                            break;
                        case "score":
                            if (int.TryParse(variables[i], out intValue))
                                player.Score = intValue;
                            break;
                        case "ping":
                            uint uintValue = 0;
                            if (uint.TryParse(variables[i], out uintValue))
                                player.Ping = uintValue;
                            break;
                    }
                }
            }

            return player;
        }
    }
}