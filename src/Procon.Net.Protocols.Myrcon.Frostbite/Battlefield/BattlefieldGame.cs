using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Procon.Net.Actions;
using Procon.Net.Data;
using Procon.Net.Protocols.Myrcon.Frostbite.Objects;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Battlefield {
    public abstract class BattlefieldGame : FrostbiteGame {

        protected BattlefieldGame(string hostName, ushort port) : base(hostName, port) {

        }

        // todo this command has about four error paths if the exact correct data is now passed in or a map is missing
        protected override List<IPacketWrapper> Action(Move move) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (move.Scope.Players != null) {
                // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
                bool forceMove = (move.ActionType == NetworkActionType.NetworkPlayerForceMove || move.ActionType == NetworkActionType.NetworkPlayerForceRotate);

                Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0);

                foreach (Player movePlayer in move.Scope.Players.Select(scopePlayer => this.State.Players.First(player => player.Uid == scopePlayer.Uid)).Where(movePlayer => movePlayer != null)) {
                    if (selectedMap != null) {
                        // If they are just looking to rotate the player through the teams
                        if (move.ActionType == NetworkActionType.NetworkPlayerRotate || move.ActionType == NetworkActionType.NetworkPlayerForceRotate) {

                            int currentTeamId = -1;

                            int.TryParse(movePlayer.Groups.First(group => @group.Type == Grouping.Team).Uid, out currentTeamId);

                            // Avoid divide by 0 error - shouldn't ever be encountered though.
                            if (selectedMap.GameMode != null && selectedMap.GameMode.TeamCount > 0) {
                                int newTeamId = (currentTeamId + 1) % (selectedMap.GameMode.TeamCount + 1);

                                move.Now.Groups.Add(new Grouping() {
                                    Type = Grouping.Team,
                                    Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                        }

                        // Now check if the destination squad is supported.
                        if (selectedMap.GameMode != null && (selectedMap.GameMode.Name == "SQDM" || selectedMap.GameMode.Name == "SQRUSH")) {
                            if (selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == Grouping.Squad) != null) {
                                move.Now.Groups.Add(selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == Grouping.Squad));
                            }
                        }
                    }

                    wrappers.Add(this.CreatePacket(
                        "admin.movePlayer \"{0}\" {1} {2} {3}",
                        movePlayer.Name,
                        move.Now.Groups.First(group => @group.Type == Grouping.Team).Uid,
                        move.Now.Groups.First(group => @group.Type == Grouping.Squad).Uid,
                        FrostbiteConverter.BoolToString(forceMove)
                    ));
                }
            }

            return wrappers;
        }
    }
}
