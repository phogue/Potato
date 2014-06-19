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
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield3.Objects;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Protocols;

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

            foreach (var player in this.State.Players) {
                this.SendPlayerPingPacket(player.Value.Name);
            }
        }

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

        public override void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<PlayerModel> players = Battlefield3Players.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public override void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                ConcurrentDictionary<String, MapModel> modified = new ConcurrentDictionary<String, MapModel>();

                // todo since the parsers are generally all that change from game to game, perhaps this should go into
                // todo another virtual method of FrostbiteGame.
                List<MapModel> maps = Battlefield3FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (MapModel map in maps) {
                    var closureMap = map;

                    MapModel mapInfo = this.State.MapPool.Values.FirstOrDefault(m => String.Compare(m.Name, closureMap.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(m.GameMode.Name, closureMap.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    if (mapInfo != null) {
                        closureMap.FriendlyName = mapInfo.FriendlyName;
                        closureMap.GameMode = mapInfo.GameMode;
                    }

                    modified.AddOrUpdate(String.Format("{0}/{1}", closureMap.GameMode.Name, closureMap.Name), id => closureMap, (id, model) => closureMap);
                }

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Override = true,
                    Modified = {
                        Maps = modified
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolMaplistUpdated,
                    difference
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
                        var closureBan = ban;
                        var key = String.Format("{0}/{1}", ban.Scope.Times.First().Context, ban.Scope.Players.First().Uid ?? ban.Scope.Players.First().Name ?? ban.Scope.Players.First().Ip);
                        this.State.Bans.AddOrUpdate(key, id => closureBan, (id, model) => closureBan);
                    }
                    
                    this.Send(this.CreatePacket("banList.list {0}", startOffset + 100));
                }
                else {
                    IProtocolStateDifference difference = new ProtocolStateDifference() {
                        Override = true,
                        Modified = {
                            Bans = this.State.Bans
                        }
                    };

                    this.ApplyProtocolStateDifference(difference);

                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolBanlistUpdated,
                        difference
                    );
                }
            }
        }

        public void PlayerPingResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2 && response != null && response.Packet.Words.Count >= 2) {
                PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
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

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Override = true,
                    Modified = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
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
    }
}
