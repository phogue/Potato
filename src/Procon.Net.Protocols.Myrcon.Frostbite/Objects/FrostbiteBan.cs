using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {
    [Serializable]
    public class FrostbiteBan {

        public static Ban ParseBanListItem(List<string> words) {
            Ban ban = new Ban();

            if (words.Count == 5) {

                ban.Time = FrostbiteTimeSubset.Parse(words.GetRange(2, 2));

                if (String.CompareOrdinal(words[0], "name") == 0) {
                    ban.Scope.Players.Add(new Player() {
                        Name = words[1]
                    });
                }
                // Procon 2.0 considers ip banning as obsolete
                //else if (String.Compare(words[0], "ip") == 0) {
                //    this.IpAddress = words[1];
                //}
                else if (String.CompareOrdinal(words[0], "guid") == 0) {
                    ban.Scope.Players.Add(new Player() {
                        Uid = words[1]
                    });
                }

                ban.ActionType = NetworkActionType.NetworkMapListed;

                ban.Scope.Content = new List<String>() {
                    words[4]
                };
            }

            return ban;
        }

        public static Ban ParseBanRemove(List<string> words) {
            Ban ban = new Ban();

            if (String.CompareOrdinal(words[0], "name") == 0) {
                ban.Scope.Players.Add(new Player() {
                    Name = words[1]
                });
            }
            else if (String.CompareOrdinal(words[0], "guid") == 0) {
                ban.Scope.Players.Add(new Player() {
                    Uid = words[1]
                });
            }

            return ban;
        }

        public static Ban ParseBanAdd(List<string> words) {
            Ban ban = new Ban();

            if (String.CompareOrdinal(words[0], "name") == 0) {
                ban.Scope.Players.Add(new Player() {
                    Name = words[1]
                });
            }
            else if (String.CompareOrdinal(words[0], "guid") == 0) {
                ban.Scope.Players.Add(new Player() {
                    Uid = words[1]
                });
            }

            if (words.Count == 3) {
                ban.Time = FrostbiteTimeSubset.Parse(words.GetRange(2, 1));
            }
            else if (words.Count == 4) {
                ban.Time = FrostbiteTimeSubset.Parse(words.GetRange(2, 2));

                // Time has a seconds parameter
                if (ban.Time.Context != TimeSubsetContext.Time) {
                    ban.Scope.Content = new List<String>() {
                        words[3]
                    };
                }
            }
            else if (words.Count == 5) {
                ban.Time = FrostbiteTimeSubset.Parse(words.GetRange(2, 2));

                ban.Scope.Content = new List<String>() {
                    words[4]
                };
            }

            return ban;
        }

    }
}
