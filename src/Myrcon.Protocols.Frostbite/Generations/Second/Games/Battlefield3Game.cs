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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Generations.Second.Games {
    /// <summary>
    /// Protocol implementation for Battlefield 3
    /// </summary>
    [ProtocolDeclaration(Type = CommonProtocolType.DiceBattlefield3, Name = "Battlefield 3", Provider = "Myrcon")]
    public class Battlefield3Game : SecondGame {

        /// <summary>
        /// Game constructor to initalize the server info and any other dispatch methods
        /// </summary>
        public Battlefield3Game() : base() {

            this.ServerInfoParameters = new List<string>() {
                "ServerName",
                "PlayerCount",
                "MaxPlayerCount",
                "GameMode",
                "Map",
                "CurrentRound",
                "TotalRounds",
                "TeamScores",
                "ConnectionState",
                "Ranked",
                "PunkBuster",
                "Passworded",
                "ServerUpTime",
                "RoundTime"
            };

            this.PacketDispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                {
                    new PacketDispatch() {
                        Name = "player.ping", 
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerPingResponseDispatchHandler)
                }
            });
        }

        /// <summary>
        /// Override and provide support for battlefield 3 to fetch each players ping
        /// </summary>
        protected override void AuxiliarySynchronize() {
            base.AuxiliarySynchronize();

            foreach (var player in this.State.Players) {
                this.SendPlayerPingPacket(player.Value.Name);
            }
        }

        /// <summary>
        /// Override to maintain the ping value and not set from the results of the player list.
        /// </summary>
        /// <param name="players"></param>
        protected override void AdminListPlayersFinalize(List<PlayerModel> players) {
            var modified = new ConcurrentDictionary<String, PlayerModel>();

            // We're essentially updating the state here anyway, but we keep the difference
            // so others can remain in sync
            foreach (PlayerModel player in players) {
                PlayerModel statePlayer;
                this.State.Players.TryGetValue(player.Uid, out statePlayer);

                if (statePlayer != null) {
                    // Already exists, update with any new information we have.
                    statePlayer.Kills = player.Kills;
                    statePlayer.Score = player.Score;
                    statePlayer.Deaths = player.Deaths;
                    statePlayer.ClanTag = player.ClanTag;
                    statePlayer.Uid = player.Uid;

                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Team));
                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Squad));

                    modified.AddOrUpdate(player.Uid, id => statePlayer, (id, model) => statePlayer);
                }
                else {
                    modified.TryAdd(player.Uid, player);
                }
            }

            IProtocolStateDifference difference = new ProtocolStateDifference() {
                Override = true,
                Removed = {
                    Players = new ConcurrentDictionary<String, PlayerModel>(this.State.Players.Where(existing => players.Select(current => current.Uid).Contains(existing.Key) == false).ToDictionary(item => item.Key, item => item.Value))
                },
                Modified = {
                    Players = modified
                }
            };

            this.ApplyProtocolStateDifference(difference);

            this.OnProtocolEvent(ProtocolEventType.ProtocolPlayerlistUpdated, difference, new ProtocolEventData() {
                Players = new List<PlayerModel>(this.State.Players.Values)
            });
        }
        
        /// <summary>
        /// Support to handle battlefield 3's response to a ping request of a player.
        /// </summary>
        /// <remarks>
        /// <para>We don't update the state to avoid spamming the sandbox with 64 simple ping requests, instead we allow the player list to pass through these changes when it updates.</para>
        /// </remarks>
        protected void PlayerPingResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2 && response != null && response.Packet.Words.Count >= 2) {
                PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
                uint ping = 0;

                if (player != null && uint.TryParse(response.Packet.Words[1], out ping) == true) {
                    // Sometimes the protocol sends through the max value of uint. We ignore everything above 1000.
                    player.Ping = ping > 1000 ? 0 : ping;
                }
            }
        }

        /// <summary>
        /// Builds and sends the player ping packet
        /// </summary>
        protected void SendPlayerPingPacket(String playerName) {
            this.Send(this.CreatePacket("player.ping {0}", playerName));
        }
    }
}
