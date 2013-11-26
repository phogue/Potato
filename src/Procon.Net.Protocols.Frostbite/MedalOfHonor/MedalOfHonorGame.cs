using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Procon.Net.Attributes;
using Procon.Net.Protocols.Objects;
using Procon.Net.Protocols.Frostbite.Objects;

namespace Procon.Net.Protocols.Frostbite.MedalOfHonor {
    [GameType(Type = CommonGameType.MOH_2010, Name = "Medal of Honor 2010", Provider = "Myrcon")]
    public class MohGame : FrostbiteGame {

        public MohGame(string hostName, ushort port) : base(hostName, port) {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }

        protected override List<IPacket> Action(Move move) {
            List<IPacket> packets = new List<IPacket>();

            // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
            bool forceMove = (move.ActionType == NetworkActionType.NetworkPlayerForceMove || move.ActionType == NetworkActionType.NetworkPlayerForceRotate);

            Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, this.State.Settings.MapName, StringComparison.OrdinalIgnoreCase) == 0);

            Player movePlayer = this.State.PlayerList.First(player => player.Uid == move.Scope.Players.First().Uid);

            if (selectedMap != null) {
                // If they are just looking to rotate the player through the teams
                if (move.ActionType == NetworkActionType.NetworkPlayerRotate || move.ActionType == NetworkActionType.NetworkPlayerForceRotate) {

                    int currentTeamId = -1;

                    int.TryParse(movePlayer.Groups.First(group => group.Type == Grouping.Team).Uid, out currentTeamId);

                    // Avoid divide by 0 error - shouldn't ever be encountered though.
                    if (selectedMap.GameMode.TeamCount > 0) {
                        int newTeamId = (currentTeamId + 1) % (selectedMap.GameMode.TeamCount + 1);

                        move.Now.Groups.Add(new Grouping() {
                            Type = Grouping.Team,
                            Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            packets.Add(
                this.Send(
                    this.CreatePacket(
                        "admin.movePlayer \"{0}\" {1} {2}",
                        movePlayer.Name,
                        move.Now.Groups.First(group => group.Type == Grouping.Team).Uid,
                        FrostbiteConverter.BoolToString(forceMove)
                    )
                )
            );

            return packets;
        }
    }
}
