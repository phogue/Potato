using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Potato.Net.Protocols.Source.Objects {
    using Potato.Net.Protocols.Objects;
    public class SourceChat : ISourceObject {

        public NetworkObject Parse(Match match) {

            Chat chat = new Chat {
                Now = {
                    Players = new List<Player>() {
                        new Player() {
                            Name = match.Groups["name"].Value,
                            Uid = match.Groups["uniqueid"].Value
                        }
                    },
                    Content = new List<String>() {
                        match.Groups["text"].Value.Replace("\r", "")
                    }
                },
                Origin = ChatOrigin.Player
            };

            if (String.Compare(match.Groups["context"].Value, "say", StringComparison.OrdinalIgnoreCase) == 0) {
                chat.Scope.Groups = new GroupingList();
            }
            else {
                // Can we get a team identifier here?
                chat.Scope.Groups = new GroupingList() {
                    new Grouping() {
                        Type = Grouping.Team
                    }
                };
            }

            return chat;
        }

        public Chat ParseConsoleSay(List<string> words) {
            Chat chat = new Chat {
                Origin = ChatOrigin.Reflected,
                Now = {
                    Players = new List<Player>() {
                        new Player() {
                            Name = "Potato"
                        }
                    },
                    Content = new List<String>() {
                        String.Join(" ", words.Skip(1).ToArray())
                    }
                }
            };

            return chat;
        }
    }
}
