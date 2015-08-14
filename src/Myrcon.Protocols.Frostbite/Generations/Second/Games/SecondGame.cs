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
using Myrcon.Protocols.Frostbite.Generations.Second.Objects;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Generations.Second.Games {
    /// <summary>
    /// The base battlefield game that handles methods used by the majority of implementations.
    /// Some protocol types may require further specialization, but the most common function
    /// will always be placed here.
    /// </summary>
    public class SecondGame : FrostbiteGame {

        public void ServerOnLevelLoadedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 5) {
                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Packet.Words[3], out currentRound) == true && int.TryParse(request.Packet.Words[4], out totalRounds) == true) {
                    UpdateSettingsMap(request.Packet.Words[1], request.Packet.Words[2]);
                    UpdateSettingsRound(currentRound, totalRounds);
                }
            }
        }

        /// <summary>
        /// Handles the player.onKill event sent from the server.
        /// This method holds for Battlefield 3/4, but may need an override in bfbc2.
        /// </summary>
        public override void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 5) {
                var headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {
                    var item = State.Items.Select(i => i.Value).FirstOrDefault(i => i.Name == Regex.Replace(request.Packet.Words[3], @"[^\w\/_-]+", "")) ?? new ItemModel() { Name = request.Packet.Words[3] };

                    var killer = State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
                    var victim = State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[2]);

                    if (killer != null && victim != null) {
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
                                Players = new ConcurrentDictionary<string, PlayerModel>()
                            }
                        };

                        difference.Modified.Players.AddOrUpdate(killer.Uid, id => killer, (id, model) => killer);
                        difference.Modified.Players.AddOrUpdate(victim.Uid, id => victim, (id, model) => victim);

                        ApplyProtocolStateDifference(difference);

                        // We've updated the state, now fetch the players from the state with all of the statistics information attached.
                        PlayerModel stateKiller = null;
                        PlayerModel stateVictim = null;

                        State.Players.TryGetValue(killer.Uid, out stateKiller);
                        State.Players.TryGetValue(victim.Uid, out stateVictim);

                        OnProtocolEvent(
                            ProtocolEventType.ProtocolPlayerKill,
                            difference,
                            new ProtocolEventData() {
                                Kills = new List<KillModel>() {
                                    new KillModel() {
                                        Scope = {
                                            Players = new List<PlayerModel>() {
                                                stateVictim
                                            },
                                            Items = new List<ItemModel>() {
                                                item
                                            },
                                            HumanHitLocations = new List<HumanHitLocation>() {
                                                headshot == true ? Headshot : Bodyshot
                                            }
                                        },
                                        Now = {
                                            Players = new List<PlayerModel>() {
                                                stateKiller
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

        public override void BanListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 1) {

                var startOffset = 0;

                if (request.Packet.Words.Count >= 2) {
                    if (int.TryParse(request.Packet.Words[1], out startOffset) == false) {
                        startOffset = 0;
                    }
                }

                // We've just started requesting the banlist, clear it.
                if (startOffset == 0) {
                    State.Bans.Clear();
                }

                var banList = SecondFrostbiteBanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                if (banList.Count > 0) {
                    foreach (var ban in banList) {
                        if (ban.Scope.Times.Count > 0 && ban.Scope.Players.Count > 0) {
                            var closureBan = ban;
                            var key = string.Format("{0}/{1}", ban.Scope.Times.First().Context, ban.Scope.Players.First().Uid ?? ban.Scope.Players.First().Name ?? ban.Scope.Players.First().Ip);
                            State.Bans.AddOrUpdate(key, id => closureBan, (id, model) => closureBan);
                        }
                    }

                    Send(CreatePacket("banList.list {0}", startOffset + 100));
                }
                else {
                    IProtocolStateDifference difference = new ProtocolStateDifference() {
                        Override = true,
                        Modified = {
                            Bans = State.Bans
                        }
                    };

                    ApplyProtocolStateDifference(difference);

                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    OnProtocolEvent(
                        ProtocolEventType.ProtocolBanlistUpdated,
                        difference
                    );
                }
            }
        }

        public override void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                var modified = new ConcurrentDictionary<string, MapModel>();

                var maps = SecondFrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (var map in maps) {
                    var closureMap = map;

                    var mapInfo = State.MapPool.Values.FirstOrDefault(m => string.Compare(m.Name, closureMap.Name, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(m.GameMode.Name, closureMap.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    if (mapInfo != null) {
                        closureMap.FriendlyName = mapInfo.FriendlyName;
                        closureMap.GameMode = mapInfo.GameMode;
                    }

                    modified.AddOrUpdate(string.Format("{0}/{1}", closureMap.GameMode.Name, closureMap.Name), id => closureMap, (id, model) => closureMap);
                }

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Override = true,
                    Modified = {
                        Maps = modified
                    }
                };

                ApplyProtocolStateDifference(difference);

                OnProtocolEvent(
                    ProtocolEventType.ProtocolMaplistUpdated,
                    difference
                );
            }
        }

        public override void PlayerOnJoinDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {
                var player = new PlayerModel() {
                    Name = request.Packet.Words[1],
                    Uid = request.Packet.Words[2]
                };

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Players = new ConcurrentDictionary<string, PlayerModel>(new Dictionary<string, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                ApplyProtocolStateDifference(difference);

                OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerJoin,
                    difference,
                    new ProtocolEventData() {
                        Players = new List<PlayerModel>() {
                            player
                        }
                    }
                );
            }
        }

        public override void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            var players = SecondFrostbitePlayerList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            AdminListPlayersFinalize(players);
        }

        protected override void SendEventsEnabledPacket() {
            Send(CreatePacket("admin.eventsEnabled true"));
        }

        public override void PlayerOnAuthenticatedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            // Ignored.
        }

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            var wrappers = new List<IPacketWrapper>();

            // If it's centered around a list of maps (adding, removing etc)
            if (action.Now.Maps != null && action.Now.Maps.Count > 0) {
                foreach (var map in action.Now.Maps) {
                    var closureMap = map;

                    if (action.ActionType == NetworkActionType.NetworkMapAppend) {
                        // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                        wrappers.Add(CreatePacket("mapList.add \"{0}\" \"{1}\" {2}", map.Name, closureMap.GameMode.Name, map.Rounds));

                        wrappers.Add(CreatePacket("mapList.save"));

                        wrappers.Add(CreatePacket("mapList.list"));
                    }
                    // Added by Imisnew2 - You should check this phogue!
                    else if (action.ActionType == NetworkActionType.NetworkMapChangeMode) {
                        if (map.GameMode != null) {
                            wrappers.Add(CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                        }
                    }
                    else if (action.ActionType == NetworkActionType.NetworkMapInsert) {
                        // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                        wrappers.Add(CreatePacket("mapList.add \"{0}\" \"{1}\" {2} {3}", map.Name, closureMap.GameMode.Name, map.Rounds, map.Index));

                        wrappers.Add(CreatePacket("mapList.save"));

                        wrappers.Add(CreatePacket("mapList.list"));
                    }
                    else if (action.ActionType == NetworkActionType.NetworkMapRemove) {
                        var matchingMaps = State.Maps.Where(m => m.Value.Name == closureMap.Name).OrderByDescending(m => m.Value.Index);

                        wrappers.AddRange(matchingMaps.Select(match => CreatePacket("mapList.remove {0}", match.Value.Index)));

                        wrappers.Add(CreatePacket("mapList.save"));

                        wrappers.Add(CreatePacket("mapList.list"));
                    }
                    else if (action.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                        wrappers.Add(CreatePacket("mapList.remove {0}", map.Index));

                        wrappers.Add(CreatePacket("mapList.list"));
                    }
                    else if (action.ActionType == NetworkActionType.NetworkMapNextIndex) {
                        wrappers.Add(CreatePacket("mapList.setNextMapIndex {0}", map.Index));
                    }
                }
            }
            else {
                // The action does not need a map to function
                if (action.ActionType == NetworkActionType.NetworkMapRestart || action.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                    wrappers.Add(CreatePacket("mapList.restartRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapNext || action.ActionType == NetworkActionType.NetworkMapRoundNext) {
                    wrappers.Add(CreatePacket("mapList.runNextRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapClear) {
                    wrappers.Add(CreatePacket("mapList.clear"));

                    wrappers.Add(CreatePacket("mapList.save"));
                }
            }

            return wrappers;
        }
    }
}
