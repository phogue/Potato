using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

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
                // Procon 2.0 considers ip banning as obsolete
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
