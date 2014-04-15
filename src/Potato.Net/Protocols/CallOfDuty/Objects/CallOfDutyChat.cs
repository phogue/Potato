using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Potato.Net.Protocols.CallOfDuty.Objects {
    using Potato.Net.Protocols.Objects;

    [Serializable]
    public static class CallOfDutyChat {

        public static Chat Parse(Match match) {
            Chat chat = new Chat {
                Now = {
                    Players = new List<Player>() {
                        new Player() {
                            Name = match.Groups["Name"].Value,
                            Uid = match.Groups["GUID"].Value
                        }
                    },
                    Content = new List<String>() {
                        match.Groups["Text"].Value.Replace("\r", "")
                    }
                },
                Origin = ChatOrigin.Player
            };

            if (String.Compare(match.Groups["Command"].Value, "say", StringComparison.OrdinalIgnoreCase) == 0) {
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
    }
}
