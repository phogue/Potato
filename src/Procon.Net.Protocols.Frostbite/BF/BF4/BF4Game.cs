using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Frostbite.BF.BF4 {
    using Procon.Net.Protocols.Frostbite.BF.BF4.Objects;
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.Frostbite.Objects;

    [GameType(Type = CommonGameType.BF_4, Name = "Battlefield 4", Provider = "Myrcon")]
    public class BF4Game : BFGame {

        public BF4Game(string hostName, ushort port)
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
        public override void AdminListPlayersResponseDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            BF4PlayerList players = new BF4PlayerList() {
                Subset = new FrostbiteGroupingList().Parse(request.Words.GetRange(1, request.Words.Count - 1))
            }.Parse(response.Words.GetRange(1, response.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public override void MapListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {

                FrostbiteMapList maps = new BF4FrostbiteMapList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

                foreach (Map map in maps) {
                    Map mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode = mapInfo.GameMode;
                    }
                }
                this.State.MapList = maps;

                this.OnGameEvent(
                    GameEventType.GameMaplistUpdated
                );
            }
        }

        public override void BanListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 1) {

                int startOffset = 0;

                if (request.Words.Count >= 2) {
                    if (int.TryParse(request.Words[1], out startOffset) == false) {
                        startOffset = 0;
                    }
                }

                // We've just started requesting the banlist, clear it.
                if (startOffset == 0) {
                    this.State.BanList.Clear();
                }

                FrostbiteBanList banList = new BF4BanList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

                if (banList.Count > 0) {
                    foreach (Ban ban in banList)
                        this.State.BanList.Add(ban);

                    this.Send(this.CreatePacket("banList.list {0}", startOffset + 100));
                }
                else {
                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    this.OnGameEvent(
                        GameEventType.GameBanlistUpdated
                    );
                }
            }
        }

        protected override void SendEventsEnabledPacket() {
            this.Send(this.CreatePacket("admin.eventsEnabled true"));
        }

        public override void PlayerOnAuthenticatedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            // Ignore this in BF4? Seems onJoin handles both.
        }

        public override void PlayerOnJoinDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 2) {

                Player player = new Player() {
                    Name = request.Words[1],
                    Uid = request.Words[2]
                };

                if (this.State.PlayerList.Find(x => x.Name == player.Name) == null) {
                    this.State.PlayerList.Add(player);
                }

                this.OnGameEvent(GameEventType.GamePlayerJoin, new GameEventData() { Players = new List<Player>() { player } });
            }
        }

        public override void PlayerOnKillDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 5) {

                bool headshot = false;

                if (bool.TryParse(request.Words[4], out headshot) == true) {

                    this.OnGameEvent(GameEventType.GamePlayerKill, new GameEventData() {
                        Kills = new List<Kill>() {
                            new Kill() {
                                HumanHitLocation = headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot,
                                Killer = this.State.PlayerList.Find(x => x.Name == request.Words[1]),
                                Target = this.State.PlayerList.Find(x => x.Name == request.Words[2]),
                                DamageType = new Item() {
                                    // Servers sends garbage at the end of the round?
                                    Name = Regex.Replace(request.Words[3], @"[^\\w\\/_-]+", "")
                                }
                            }
                        }
                    });
                }
            }
        }

        #region Packet Helpers

        protected override void Action(Map map) {

            if (map.ActionType == NetworkActionType.NetworkMapAppend) {
                // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                this.Send(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2}", map.Name, map.GameMode.Name, map.Rounds));

                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list"));
            }
            // Added by Imisnew2 - You should check this phogue!
            else if (map.ActionType == NetworkActionType.NetworkMapChangeMode) {
                if (map.GameMode != null) {
                    this.Send(this.CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                }
            }
            else if (map.ActionType == NetworkActionType.NetworkMapInsert) {
                // mapList.add <map: string> <gamemode: string> <rounds: integer> [index: integer]
                this.Send(this.CreatePacket("mapList.add \"{0}\" \"{1}\" {2} {3}", map.Name, map.GameMode.Name, map.Rounds, map.Index));

                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemove) {
                var matchingMaps = this.State.MapList.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                foreach (Map match in matchingMaps) {
                    this.Send(this.CreatePacket("mapList.remove {0}", match.Index));
                }
                
                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                this.Send(this.CreatePacket("mapList.remove {0}", map.Index));

                this.Send(this.CreatePacket("mapList.list"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNextIndex) {
                this.Send(this.CreatePacket("mapList.setNextMapIndex {0}", map.Index));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRestart || map.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                this.Send(this.CreatePacket("mapList.restartRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNext || map.ActionType == NetworkActionType.NetworkMapRoundNext) {
                this.Send(this.CreatePacket("mapList.runNextRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapClear) {
                this.Send(this.CreatePacket("mapList.clear"));

                this.Send(this.CreatePacket("mapList.save"));
            }
        }

        #endregion

    }
}
