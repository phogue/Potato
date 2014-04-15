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
    [Serializable]
    public static class FrostbiteGroupingList {

        private static FrostbitePlayerSubsetContext GetSubsetContext(String context) {
            FrostbitePlayerSubsetContext result = FrostbitePlayerSubsetContext.All;

            try {
                result = (FrostbitePlayerSubsetContext) Enum.Parse(typeof (FrostbitePlayerSubsetContext), context, true);
            }
            catch {
                // If any errors occur, default to all players.
                result = FrostbitePlayerSubsetContext.All;
            }

            return result;
        }

        public static List<GroupModel> Parse(List<String> words) {
            List<GroupModel> groups = new List<GroupModel>();

            if (words.Count >= 1) {

                FrostbitePlayerSubsetContext context = FrostbiteGroupingList.GetSubsetContext(words[0]);

                if (words.Count >= 2) {
                    int parsedTeamId = 0;

                    if (context == FrostbitePlayerSubsetContext.Player) {
                        groups.Add(new GroupModel() {
                            Type = GroupModel.Player,
                            Uid = words[1]
                        });
                    }
                    else if (context == FrostbitePlayerSubsetContext.Team && int.TryParse(words[1], out parsedTeamId) == true) {
                        groups.Add(new GroupModel() {
                            Type = GroupModel.Team,
                            Uid = parsedTeamId.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                    else if (words.Count >= 3) {
                        int parsedSquadId = 0;

                        if (context == FrostbitePlayerSubsetContext.Squad && int.TryParse(words[1], out parsedTeamId) == true && int.TryParse(words[2], out parsedSquadId) == true) {
                            groups.Add(new GroupModel() {
                                Type = GroupModel.Team,
                                Uid = parsedTeamId.ToString(CultureInfo.InvariantCulture)
                            });
                            groups.Add(new GroupModel() {
                                Type = GroupModel.Squad,
                                Uid = parsedSquadId.ToString(CultureInfo.InvariantCulture)
                            });
                        }
                    }
                }
            }

            return groups;
        }

    }
}
