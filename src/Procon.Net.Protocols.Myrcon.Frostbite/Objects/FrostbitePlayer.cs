using System;
using System.Collections.Generic;
using System.Globalization;
using Procon.Net.Shared.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {

    public static class FrostbitePlayer {
        
        public static Player Parse(IList<String> parameters, IList<String> variables) {
            Player player = new Player();

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
                                player.Groups.Add(new Grouping() {
                                    Type = Grouping.Team,
                                    Uid = intValue.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                            break;
                        case "squadid":
                            if (int.TryParse(variables[i], out intValue)) {
                                player.Groups.Add(new Grouping() {
                                    Type = Grouping.Squad,
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