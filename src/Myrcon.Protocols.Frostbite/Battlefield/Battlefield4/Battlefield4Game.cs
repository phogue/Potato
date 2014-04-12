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
using System.Linq;
using System.Text.RegularExpressions;
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield4.Objects;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield4 {
    [ProtocolDeclaration(Type = CommonProtocolType.DiceBattlefield4, Name = "Battlefield 4", Provider = "Myrcon")]
    public class Battlefield4Game : BattlefieldGame {

        public Battlefield4Game() : base() {

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
                        Name = "server.onLevelLoaded",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.ServerOnLevelLoadedDispatchHandler)
                }
            });
        }

        //[DispatchPacket(MatchText = "admin.listPlayers", PacketOrigin = PacketOrigin.Client)]
        public override void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<PlayerModel> players = Battlefield4Players.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public override void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {

                List<MapModel> maps = Battlefield4FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (MapModel map in maps) {
                    MapModel mapInfo = this.State.MapPool.Find(item => String.Compare(item.Name, map.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.GameMode.Name, map.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode = mapInfo.GameMode;
                    }
                }

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolMaplistUpdated,
                    new ProtocolStateDifference() {
                        Modified = {
                            Maps = maps
                        }
                    }
                );
            }
        }

        public override void BanListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 1) {

                int startOffset = 0;

                if (request.Packet.Words.Count >= 2) {
                    if (int.TryParse(request.Packet.Words[1], out startOffset) == false) {
                        startOffset = 0;
                    }
                }

                // We've just started requesting the banlist, clear it.
                if (startOffset == 0) {
                    this.State.Bans.Clear();
                }

                List<BanModel> banList = Battlefield4BanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                if (banList.Count > 0) {
                    foreach (BanModel ban in banList)
                        this.State.Bans.Add(ban);

                    this.Send(this.CreatePacket("banList.list {0}", startOffset + 100));
                }
                else {
                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolBanlistUpdated,
                        new ProtocolStateDifference() {
                            Override = true,
                            Modified = {
                                Bans = this.State.Bans
                            }
                        }
                    );
                }
            }
        }

        protected override void SendEventsEnabledPacket() {
            this.Send(this.CreatePacket("admin.eventsEnabled true"));
        }

        public override void PlayerOnAuthenticatedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            // Ignore this in BF4? Seems onJoin handles both.
        }

        public override void PlayerOnJoinDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {
                PlayerModel player = new PlayerModel() {
                    Name = request.Packet.Words[1],
                    Uid = request.Packet.Words[2]
                };

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerJoin,
                    new ProtocolStateDifference() {
                        Modified = {
                            Players = this.State.Players.Any(item => item.Name == player.Name) ? new List<PlayerModel>() {
                                player
                            } : new List<PlayerModel>()
                        }
                    }, 
                    new ProtocolEventData() {
                        Players = new List<PlayerModel>() {
                            player
                        }
                    }
                );
            }
        }

        public override void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 5) {

                bool headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {

                    ItemModel item = this.State.Items.FirstOrDefault(i => i.Name == Regex.Replace(request.Packet.Words[3], @"[^\w\/_-]+", ""));

                    PlayerModel killer = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);
                    PlayerModel victim = this.State.Players.Find(x => x.Name == request.Packet.Words[2]);

                    // Assign the item to the player, overwriting everything else attached to this killer.
                    if (killer != null) {
                        killer.Inventory.Now.Items.Clear();
                        killer.Inventory.Now.Items.Add(item);
                    }

                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolPlayerKill,
                        new ProtocolStateDifference() {
                            Modified = {
                                Players = new List<PlayerModel>() {
                                    killer,
                                    victim
                                }
                            }
                        },
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

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            foreach (MapModel map in action.Now.Maps) {
                if (action.ActionType == NetworkActionType.NetworkMapAppend) {
                    // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                    wrappers.Add(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2}", map.Name, map.GameMode.Name, map.Rounds));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list"));
                }
                    // Added by Imisnew2 - You should check this phogue!
                else if (action.ActionType == NetworkActionType.NetworkMapChangeMode) {
                    if (map.GameMode != null) {
                        wrappers.Add(this.CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                    }
                }
                else if (action.ActionType == NetworkActionType.NetworkMapInsert) {
                    // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                    wrappers.Add(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2} {3}", map.Name, map.GameMode.Name, map.Rounds, map.Index));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRemove) {
                    var matchingMaps = this.State.Maps.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                    wrappers.AddRange(matchingMaps.Select(match => this.CreatePacket("mapList.remove {0}", match.Index)));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                    wrappers.Add(this.CreatePacket("mapList.remove {0}", map.Index));

                    wrappers.Add(this.CreatePacket("mapList.list"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapNextIndex) {
                    wrappers.Add(this.CreatePacket("mapList.setNextMapIndex {0}", map.Index));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRestart || action.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                    wrappers.Add(this.CreatePacket("mapList.restartRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapNext || action.ActionType == NetworkActionType.NetworkMapRoundNext) {
                    wrappers.Add(this.CreatePacket("mapList.runNextRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapClear) {
                    wrappers.Add(this.CreatePacket("mapList.clear"));

                    wrappers.Add(this.CreatePacket("mapList.save"));
                }
            }

            return wrappers;
        }

    }
}
