using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Procon.Net.Protocols.Myrcon.Frostbite.Battlefield.Battlefield4.Objects;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Battlefield.Battlefield4 {
    [ProtocolDeclaration(Type = CommonGameType.DiceBattlefield4, Name = "Battlefield 4", Provider = "Myrcon")]
    public class Battlefield4Game : BattlefieldGame {

        public Battlefield4Game(string hostName, ushort port)
            : base(hostName, port) {

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
        }

        //[DispatchPacket(MatchText = "admin.listPlayers", PacketOrigin = PacketOrigin.Client)]
        public override void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<Player> players = Battlefield4Players.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public override void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {

                List<Map> maps = Battlefield4FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (Map map in maps) {
                    Map mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode = mapInfo.GameMode;
                    }
                }
                this.State.Maps = maps;

                this.OnGameEvent(
                    ProtocolEventType.ProtocolMaplistUpdated
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

                List<Ban> banList = Battlefield4BanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                if (banList.Count > 0) {
                    foreach (Ban ban in banList)
                        this.State.Bans.Add(ban);

                    this.Send(this.CreatePacket("banList.list {0}", startOffset + 100));
                }
                else {
                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    this.OnGameEvent(
                        ProtocolEventType.ProtocolBanlistUpdated
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

                Player player = new Player() {
                    Name = request.Packet.Words[1],
                    Uid = request.Packet.Words[2]
                };

                if (this.State.Players.Find(x => x.Name == player.Name) == null) {
                    this.State.Players.Add(player);
                }

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerJoin, new ProtocolEventData() { Players = new List<Player>() { player } });
            }
        }

        public override void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 5) {

                bool headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {

                    Item item = this.State.Items.FirstOrDefault(i => i.Name == Regex.Replace(request.Packet.Words[3], @"[^\w\/_-]+", ""));

                    Player killer = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);
                    Player victim = this.State.Players.Find(x => x.Name == request.Packet.Words[2]);

                    // Assign the item to the player, overwriting everything else attached to this killer.
                    if (killer != null) {
                        killer.Inventory.Items.Clear();
                        killer.Inventory.Items.Add(item);
                    }

                    this.OnGameEvent(ProtocolEventType.ProtocolPlayerKill, new ProtocolEventData() {
                        Kills = new List<Kill>() {
                            new Kill() {
                                HumanHitLocation = headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot,
                                Scope = {
                                    Players = new List<Player>() {
                                        victim
                                    },
                                    Items = new List<Item>() {
                                        item
                                    }
                                },
                                Now = {
                                    Players = new List<Player>() {
                                        killer
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }

        protected override List<IPacketWrapper> Action(Map map) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (map.ActionType == NetworkActionType.NetworkMapAppend) {
                // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                wrappers.Add(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2}", map.Name, map.GameMode.Name, map.Rounds));

                wrappers.Add(this.CreatePacket("mapList.save"));

                wrappers.Add(this.CreatePacket("mapList.list"));
            }
            // Added by Imisnew2 - You should check this phogue!
            else if (map.ActionType == NetworkActionType.NetworkMapChangeMode) {
                if (map.GameMode != null) {
                    wrappers.Add(this.CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                }
            }
            else if (map.ActionType == NetworkActionType.NetworkMapInsert) {
                // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                wrappers.Add(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2} {3}", map.Name, map.GameMode.Name, map.Rounds, map.Index));

                wrappers.Add(this.CreatePacket("mapList.save"));

                wrappers.Add(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemove) {
                var matchingMaps = this.State.Maps.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                wrappers.AddRange(matchingMaps.Select(match => this.CreatePacket("mapList.remove {0}", match.Index)));

                wrappers.Add(this.CreatePacket("mapList.save"));

                wrappers.Add(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                wrappers.Add(this.CreatePacket("mapList.remove {0}", map.Index));

                wrappers.Add(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNextIndex) {
                wrappers.Add(this.CreatePacket("mapList.setNextMapIndex {0}", map.Index));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRestart || map.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                wrappers.Add(this.CreatePacket("mapList.restartRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNext || map.ActionType == NetworkActionType.NetworkMapRoundNext) {
                wrappers.Add(this.CreatePacket("mapList.runNextRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapClear) {
                wrappers.Add(this.CreatePacket("mapList.clear"));

                wrappers.Add(this.CreatePacket("mapList.save"));
            }

            return wrappers;
        }

    }
}
