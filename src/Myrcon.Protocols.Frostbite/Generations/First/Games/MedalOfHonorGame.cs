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
using System.Globalization;
using System.Linq;
using Myrcon.Protocols.Frostbite.Objects;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Generations.First.Games {
    [ProtocolDeclaration(Type = CommonProtocolType.DiceMedalOfHonor2010, Name = "Medal of Honor 2010", Provider = "Myrcon")]
    public class MedalOfHonorGame : FirstGame {

        public MedalOfHonorGame() : base() {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }

        protected override List<IPacketWrapper> ActionMove(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
            bool forceMove = (action.ActionType == NetworkActionType.NetworkPlayerMoveForce || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce);

            MapModel selectedMap = this.State.MapPool.Values.FirstOrDefault(x => String.Compare(x.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0);

            PlayerModel movePlayer = this.State.Players.Values.First(player => player.Uid == action.Scope.Players.First().Uid);

            if (selectedMap != null) {
                // If they are just looking to rotate the player through the teams
                if (action.ActionType == NetworkActionType.NetworkPlayerMoveRotate || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce) {

                    int currentTeamId = -1;

                    int.TryParse(movePlayer.Groups.First(group => group.Type == GroupModel.Team).Uid, out currentTeamId);

                    // Avoid divide by 0 error - shouldn't ever be encountered though.
                    if (selectedMap.GameMode.TeamCount > 0) {
                        int newTeamId = (currentTeamId + 1) % (selectedMap.GameMode.TeamCount + 1);

                        action.Now.Groups.Add(new GroupModel() {
                            Type = GroupModel.Team,
                            Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            wrappers.Add(
                this.CreatePacket(
                    "admin.movePlayer \"{0}\" {1} {2}",
                    movePlayer.Name,
                    action.Now.Groups.First(group => group.Type == GroupModel.Team).Uid,
                    FrostbiteConverter.BoolToString(forceMove)
                )
            );

            return wrappers;
        }
    }
}
