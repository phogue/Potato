using System;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.CallOfDuty.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class CallOfDutyPlayerList : PlayerList {

        private static readonly Regex PlayerMatch = new Regex(@"^[ ]*(?<num>[\-0-9]+)[ ]*(?<score>[\-0-9]+)[ ]*(?<ping>[\-0-9]+)[ ]*(?<guid>[\-0-9]+)[ ]*(?<name>.+?)\^7?[ ]*(?<team>[\-0-9]+)[ ]*(?<lastmsg>[\-0-9]+)[ ]*(?<address>[0-9\\.]+?):[\-0-9]+?[ ]*(?<qport>[\-0-9]+)[ ]*(?<rate>[\-0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public CallOfDutyPlayerList Parse(String text) {

            // Empty subset, implying all players.
            this.Subset = new GroupingList();

            //System.IO.File.WriteAllText("doutput/playerlist.txt", "");

            foreach (string line in text.Split(new [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                Match player = CallOfDutyPlayerList.PlayerMatch.Match(line);

                if (player.Success == true) {
                    this.Add(
                        new Player() {
                            SlotID = uint.Parse(player.Groups["num"].Value),
                            Uid = player.Groups["guid"].Value,
                            Name = player.Groups["name"].Value,
                            Ping = uint.Parse(player.Groups["ping"].Value) == 999 ? 0 : uint.Parse(player.Groups["ping"].Value),
                            Score = int.Parse(player.Groups["score"].Value),
                            Groups = new GroupingList() {
                                new Grouping() {
                                    Type = Grouping.Team,
                                    Uid = player.Groups["team"].Value
                                }
                            },
                            IP = player.Groups["address"].Value
                        }
                    );
                    //System.IO.File.AppendAllText("doutput/playerlist.txt", String.Format("PASS: {0}\r\n", line));
                }
                else {
                    //System.IO.File.AppendAllText("doutput/playerlist.txt", String.Format("FAIL: {0}\r\n", line));
                }
            }

            //foreach (Match player in CallOfDutyPlayerList.PlayerMatch.Matches(text)) {

            //}

            return this;
        }

    }
}
