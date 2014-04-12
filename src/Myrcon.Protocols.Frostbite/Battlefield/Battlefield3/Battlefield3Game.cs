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
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield3.Objects;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield3 {
    [ProtocolDeclaration(Type = CommonProtocolType.DiceBattlefield3, Name = "Battlefield 3", Provider = "Myrcon")]
    public class Battlefield3Game : BattlefieldGame {

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

        protected override void AuxiliarySynchronize() {
            base.AuxiliarySynchronize();

            foreach (PlayerModel player in this.State.Players) {
                this.SendPlayerPingPacket(player.Name);
            }
        }

        protected override void AdminListPlayersFinalize(List<PlayerModel> players) {
            var modified = new List<PlayerModel>();

            // We're essentially updating the state here anyway, but we keep the difference
            // so others can remain in sync
            foreach (PlayerModel player in players) {
                PlayerModel statePlayer = this.State.Players.Find(x => x.Name == player.Name);

                if (statePlayer != null) {
                    // Already exists, update with any new information we have.
                    statePlayer.Kills = player.Kills;
                    statePlayer.Score = player.Score;
                    statePlayer.Deaths = player.Deaths;
                    statePlayer.ClanTag = player.ClanTag;
                    statePlayer.Uid = player.Uid;

                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Team));
                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Squad));

                    modified.Add(statePlayer);
                }
                else {
                    modified.Add(player);
                }
            }

            this.OnProtocolEvent(ProtocolEventType.ProtocolPlayerlistUpdated, new ProtocolStateDifference() {
                Removed = {
                    Players = this.State.Players.Where(existing => players.Select(current => current.Uid).Contains(existing.Uid) == false).ToList()
                },
                Modified = {
                    Players = modified
                }
            }, new ProtocolEventData() {
                Players = new List<PlayerModel>(this.State.Players)
            });
        }

        public override void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<PlayerModel> players = Battlefield3Players.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public override void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {

                List<MapModel> maps = Battlefield3FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (MapModel map in maps) {
                    MapModel mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode = mapInfo.GameMode;
                    }
                }

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolMaplistUpdated,
                    new ProtocolStateDifference() {
                        Override = true,
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

                List<BanModel> banList = Battlefield3BanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                if (banList.Count > 0) {
                    foreach (BanModel ban in banList) {
                        this.State.Bans.Add(ban);
                    }
                    
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

        public void PlayerPingResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2 && response != null && response.Packet.Words.Count >= 2) {
                PlayerModel player = this.State.Players.FirstOrDefault(p => p.Name == request.Packet.Words[1]);
                uint ping = 0;

                if (player != null && uint.TryParse(response.Packet.Words[1], out ping) == true) {
                    // Sometimes the protocol sends through the max value of uint. We ignore everything above 1000.
                    player.Ping = ping > 1000 ? 0 : ping;
                }
            }
        }

        protected void SendPlayerPingPacket(String playerName) {
            this.Send(this.CreatePacket("player.ping {0}", playerName));
        }

        protected override void SendEventsEnabledPacket() {
            this.Send(this.CreatePacket("admin.eventsEnabled true"));
        }

        public override void PlayerOnAuthenticatedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            // Ignore this in bf3? Seems onJoin handles both.
        }

        public override void PlayerOnJoinDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {

                PlayerModel player = new PlayerModel() {
                    Name = request.Packet.Words[1],
                    Uid = request.Packet.Words[2]
                };

                if (this.State.Players.Find(x => x.Name == player.Name) == null) {
                    this.State.Players.Add(player);
                }

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerJoin,
                    new ProtocolStateDifference() {
                        Override = true,
                        Modified = {
                            Players = new List<PlayerModel>() {
                                player
                            }
                        }
                    }, new ProtocolEventData() {
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

                    PlayerModel killer = this.State.Players.FirstOrDefault(p => p.Name == request.Packet.Words[1]);
                    PlayerModel target = this.State.Players.FirstOrDefault(p => p.Name == request.Packet.Words[1]);

                    if (killer != null && target != null) {
                        // If not a suicide.
                        if (killer.Uid != target.Uid) {
                            killer.Kills++;
                        }

                        target.Deaths++;
                    }

                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolPlayerKill,
                        new ProtocolStateDifference() {
                            Modified = {
                                Players = new List<PlayerModel>() {
                                    killer,
                                    target
                                }
                            }
                        },
                        new ProtocolEventData() {
                            Kills = new List<KillModel>() {
                                new KillModel() {
                                    Scope = {
                                        Players = new List<PlayerModel>() {
                                            target
                                        },
                                        Items = new List<ItemModel>() {
                                            new ItemModel() {
                                                // Servers sends garbage at the end of the round?
                                                Name = Regex.Replace(request.Packet.Words[3], @"[^\\w\\/_-]+", "")
                                            }
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
