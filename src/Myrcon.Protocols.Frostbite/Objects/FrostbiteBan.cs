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
using System.Linq;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public class FrostbiteBan {

        public static BanModel ParseBanListItem(List<string> words) {
            BanModel ban = new BanModel();

            if (words.Count == 5) {

                ban.Scope.Times.Add(FrostbiteTimeSubset.Parse(words.GetRange(2, 2)));

                if (String.CompareOrdinal(words[0], "name") == 0) {
                    ban.Scope.Players.Add(new PlayerModel() {
                        Name = words[1]
                    });
                }
                // Potato 2.0 considers ip banning as obsolete
                //else if (String.Compare(words[0], "ip") == 0) {
                //    this.IpAddress = words[1];
                //}
                else if (String.CompareOrdinal(words[0], "guid") == 0) {
                    ban.Scope.Players.Add(new PlayerModel() {
                        Uid = words[1]
                    });
                }

                ban.Scope.Content = new List<String>() {
                    words[4]
                };
            }

            return ban;
        }

        public static BanModel ParseBanRemove(List<string> words) {
            BanModel ban = new BanModel();

            if (String.CompareOrdinal(words[0], "name") == 0) {
                ban.Scope.Players.Add(new PlayerModel() {
                    Name = words[1]
                });
            }
            else if (String.CompareOrdinal(words[0], "guid") == 0) {
                ban.Scope.Players.Add(new PlayerModel() {
                    Uid = words[1]
                });
            }

            return ban;
        }

        public static BanModel ParseBanAdd(List<string> words) {
            BanModel ban = new BanModel();

            if (String.CompareOrdinal(words[0], "name") == 0) {
                ban.Scope.Players.Add(new PlayerModel() {
                    Name = words[1]
                });
            }
            else if (String.CompareOrdinal(words[0], "guid") == 0) {
                ban.Scope.Players.Add(new PlayerModel() {
                    Uid = words[1]
                });
            }

            if (words.Count == 3) {
                ban.Scope.Times.Add(FrostbiteTimeSubset.Parse(words.GetRange(2, 1)));
            }
            else if (words.Count == 4) {
                ban.Scope.Times.Add(FrostbiteTimeSubset.Parse(words.GetRange(2, 2)));

                // Time has a seconds parameter
                if (ban.Scope.Times.First().Context != TimeSubsetContext.Time) {
                    ban.Scope.Content = new List<String>() {
                        words[3]
                    };
                }
            }
            else if (words.Count == 5) {
                ban.Scope.Times.Add(FrostbiteTimeSubset.Parse(words.GetRange(2, 2)));

                ban.Scope.Content = new List<String>() {
                    words[4]
                };
            }

            return ban;
        }

    }
}
