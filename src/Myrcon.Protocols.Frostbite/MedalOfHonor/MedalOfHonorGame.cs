using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Myrcon.Protocols.Frostbite.Objects;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.MedalOfHonor {
    [ProtocolDeclaration(Type = CommonGameType.DiceMedalOfHonor2010, Name = "Medal of Honor 2010", Provider = "Myrcon")]
    public class MohGame : FrostbiteGame {

        public MohGame(string hostName, ushort port) : base(hostName, port) {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }

        protected override List<IPacketWrapper> ActionMove(NetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
            bool forceMove = (action.ActionType == NetworkActionType.NetworkPlayerMoveForce || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce);

            Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0);

            Player movePlayer = this.State.Players.First(player => player.Uid == action.Scope.Players.First().Uid);

            if (selectedMap != null) {
                // If they are just looking to rotate the player through the teams
                if (action.ActionType == NetworkActionType.NetworkPlayerMoveRotate || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce) {

                    int currentTeamId = -1;

                    int.TryParse(movePlayer.Groups.First(group => group.Type == Grouping.Team).Uid, out currentTeamId);

                    // Avoid divide by 0 error - shouldn't ever be encountered though.
                    if (selectedMap.GameMode.TeamCount > 0) {
                        int newTeamId = (currentTeamId + 1) % (selectedMap.GameMode.TeamCount + 1);

                        action.Now.Groups.Add(new Grouping() {
                            Type = Grouping.Team,
                            Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            wrappers.Add(
                this.CreatePacket(
                    "admin.movePlayer \"{0}\" {1} {2}",
                    movePlayer.Name,
                    action.Now.Groups.First(group => group.Type == Grouping.Team).Uid,
                    FrostbiteConverter.BoolToString(forceMove)
                )
            );

            return wrappers;
        }
    }
}
