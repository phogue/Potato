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
using System.Text.RegularExpressions;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield {
    /// <summary>
    /// The base battlefield game that handles methods used by the majority of implementations.
    /// Some protocol types may require further specialization, but the most common function
    /// will always be placed here.
    /// </summary>
    public class BattlefieldGame : FrostbiteGame {

        public void ServerOnLevelLoadedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 5) {
                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Packet.Words[3], out currentRound) == true && int.TryParse(request.Packet.Words[4], out totalRounds) == true) {
                    this.UpdateSettingsMap(request.Packet.Words[1], request.Packet.Words[2]);
                    this.UpdateSettingsRound(currentRound, totalRounds);
                }
            }
        }

        /// <summary>
        /// Handles the player.onKill event sent from the server.
        /// This method holds for Battlefield 3/4, but may need an override in bfbc2.
        /// </summary>
        public override void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 5) {
                bool headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {
                    ItemModel item = this.State.Items.Select(i => i.Value).FirstOrDefault(i => i.Name == Regex.Replace(request.Packet.Words[3], @"[^\w\/_-]+", "")) ?? new ItemModel() { Name = request.Packet.Words[3] };

                    var killer = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]) ?? new PlayerModel() { Name = request.Packet.Words[1] };
                    var victim = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[2]) ?? new PlayerModel() { Name = request.Packet.Words[2] };

                    // Assign the item to the player, overwriting everything else attached to this killer.
                    killer.Inventory.Now.Items.Clear();
                    killer.Inventory.Now.Items.Add(item);

                    victim.Deaths++;

                    // If this wasn't an inside job.
                    if (killer.Uid != victim.Uid) {
                        killer.Kills++;
                    }

                    var difference = new ProtocolStateDifference() {
                        Modified = {
                            Players = new ConcurrentDictionary<String, PlayerModel>()
                        }
                    };

                    difference.Modified.Players.AddOrUpdate(killer.Uid, id => killer, (id, model) => killer);
                    difference.Modified.Players.AddOrUpdate(victim.Uid, id => victim, (id, model) => victim);

                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolPlayerKill,
                        difference,
                        new ProtocolEventData() {
                            Kills = new List<KillModel>() {
                                new KillModel() {
                                    Scope = {
                                        Players = new List<PlayerModel>() {
                                            victim
                                        },
                                        Items = new List<ItemModel>() {
                                            item
                                        },
                                        HumanHitLocations = new List<HumanHitLocation>() {
                                            headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot
                                        }
                                    },
                                    Now = {
                                        Players = new List<PlayerModel>() {
                                            killer
                                        }
                                    }
                                }
                            }
                        }
                    );
                }
            }
        }
    }
}
