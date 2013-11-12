using System;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class SourcePlayerList : PlayerList {

        private static readonly Regex PlayerMatch = new Regex(@"# (?<userid>[0-9]+) ""(?<name>.+)"" (?<uniqueid>[A-Z0-9\:_]+) (?<connected>[0-9\:]+) (?<ping>[0-9]+).*? (?<ip>[0-9\.]+):(?<port>[0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SourcePlayerList Parse(string text) {

            // Empty player list implying all players.
            this.Subset = new GroupingList();

            foreach (string line in text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                Match player = SourcePlayerList.PlayerMatch.Match(line);

                if (player.Success == true) {
                    this.Add(
                        new Player() {
                            SlotId = uint.Parse(player.Groups["userid"].Value),
                            Uid = player.Groups["uniqueid"].Value,
                            Name = player.Groups["name"].Value,
                            Ping = uint.Parse(player.Groups["ping"].Value) == 999 ? 0 : uint.Parse(player.Groups["ping"].Value),
                            //Score = int.Parse(player.Groups["score"].Value),
                            // Team = CallOfDutyConverter.TeamIdToTeam(int.Parse(player.Groups["team"].Value)),
                            IP = player.Groups["ip"].Value
                        }
                    );
                }
            }

            return this;
        }
    }
}
