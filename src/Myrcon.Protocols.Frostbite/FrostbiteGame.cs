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
using System.Globalization;
using System.Linq;
using System.Text;
using Myrcon.Protocols.Frostbite.Objects;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Geolocation;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Protocols.PunkBuster;
using Potato.Net.Shared.Protocols.PunkBuster.Packets;
using Potato.Net.Shared.Utils;

namespace Myrcon.Protocols.Frostbite {
    public abstract class FrostbiteGame : Protocol {
        /// <summary>
        /// Used when determining a player's Country Name and Code.
        /// </summary>
        protected static readonly IGeolocate Geolocation = new GeolocateIp();

        /// <summary>
        /// Flags showing a headshot (neck/head)
        /// </summary>
        public const HumanHitLocation Headshot = HumanHitLocation.Head | HumanHitLocation.Neck;

        /// <summary>
        /// Flags showing a body shot, which is just anything not covered by FrostbiteGame.Headshot
        /// </summary>
        public const HumanHitLocation Bodyshot = HumanHitLocation.LeftHand | HumanHitLocation.LeftFoot | HumanHitLocation.RightHand | HumanHitLocation.RightFoot | HumanHitLocation.LowerLeftArm | HumanHitLocation.LowerLeftLeg | HumanHitLocation.LowerRightArm | HumanHitLocation.LowerRightLeg | HumanHitLocation.UpperLeftArm | HumanHitLocation.UpperLeftLeg | HumanHitLocation.UpperRightArm | HumanHitLocation.UpperRightLeg | HumanHitLocation.LowerTorso | HumanHitLocation.UpperTorso;

        protected List<String> ServerInfoParameters = new List<String>();

        /// <summary>
        /// Date for the next sync of the banlist/maplist/pb list. Everything that
        /// does not change often, but we sync to make sure changes applied
        /// from other tools will be updated in a timely manner.
        /// </summary>
        protected DateTime NextAuxiliarySynchronization = DateTime.Now;

        protected FrostbiteGame() : base() {
            this.State.Settings.Maximum.ChatLinesCount = 100;

            // Client
            this.Client = new FrostbiteClient();

            this.PacketDispatcher = new FrostbitePacketDispatcher() {
                PacketQueue = ((FrostbiteClient)this.Client).PacketQueue
            };

            this.PacketDispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                {
                    new PacketDispatch() {
                        Name = "serverInfo", 
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.ServerInfoDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "login.plainText",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.LoginPlainTextDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "login.hashed",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.LoginHashedDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "admin.eventsEnabled",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.AdminEventsEnabledDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "admin.listPlayers",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.AdminListPlayersResponseDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "mapList.list",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.MapListListDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "admin.say",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.AdminSayDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "version",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VersionDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "banList.list",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.BanListListDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "banList.add",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.BanListAddDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "banList.remove",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.BanListRemoveDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.serverName",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsServerNameDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.gamePassword",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsGamePasswordDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.punkBuster",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsGamePunkbusterDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.hardCore",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsHardcoreDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.ranked",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsRankedDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.rankLimit",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsRankLimitDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.teamBalance",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsTeamBalanceDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.friendlyFire",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsFriendlyFireDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.bannerUrl",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsBannerUrlDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.serverDescription",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsServerDescriptionDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.killCam",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsKillCamDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.miniMap",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsMiniMapDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.crossHair",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsCrossHairDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.idleTimeout",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsIdleTimeoutDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "vars.profanityFilter",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.VarsProfanityFilterDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "punkBuster.onMessage",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PunkBusterOnMessageDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onKill",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnKillDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "server.onLoadingLevel",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.ServerOnLoadingLevelDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onJoin",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnJoinDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onLeave",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnLeaveDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onChat",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnChatDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onAuthenticated",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnAuthenticatedDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onSpawn",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnSpawnDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onKicked",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnKickedDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onSquadChange",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnSquadChangeDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "player.onTeamChange",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.PlayerOnTeamChangeDispatchHandler)
                }, {
                    new PacketDispatch() {
                        Name = "InvalidPasswordHash",
                        Origin = PacketOrigin.Client
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.InvalidPasswordHashDispatchHandler)
                }
            });
        }

        protected virtual void AuxiliarySynchronize() {
            this.Send(this.CreatePacket("punkBuster.pb_sv_command pb_sv_plist"));
            // this.Send(this.Create("mapList.list rounds")); BF3 doesn't take the "rounds" on the end.
            this.Send(this.CreatePacket("mapList.list"));
            this.Send(this.CreatePacket("banList.list"));
        }

        public override void Synchronize() {
            base.Synchronize();

            if (this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                this.Send(this.CreatePacket("admin.listPlayers all"));
                this.Send(this.CreatePacket("serverInfo"));

                if (DateTime.Now >= this.NextAuxiliarySynchronization) {
                    this.AuxiliarySynchronize();

                    this.NextAuxiliarySynchronization = DateTime.Now.AddSeconds(120);
                }
            }
        }

        public string GeneratePasswordHash(byte[] salt, string data) {
            byte[] combined = new byte[salt.Length + data.Length];
            salt.CopyTo(combined, 0);
            Encoding.ASCII.GetBytes(data).CopyTo(combined, salt.Length);

            return MD5.Data(combined).ToUpper();
        }

        public byte[] HashToByteArray(string hexString) {
            byte[] returnHash = new byte[hexString.Length / 2];

            for (int i = 0; i < returnHash.Length; i++) {
                returnHash[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnHash;
        }

        /// <summary>
        /// Update the current map name/gamemode, populating other settings and firing map change events as required.
        /// </summary>
        /// <param name="name">The name of the map to update with</param>
        /// <param name="gameModeName">The game mdoe of the map being played</param>
        protected void UpdateSettingsMap(String name, String gameModeName) {
            var modified = this.State.Settings.Current.MapNameText != name || this.State.Settings.Current.GameModeNameText != gameModeName;

            if (modified == true) {
                MapModel oldMap = this.State.MapPool.Select(m => m.Value).FirstOrDefault(map => String.Compare(map.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(map.GameMode.Name, this.State.Settings.Current.GameModeNameText, StringComparison.OrdinalIgnoreCase) == 0);

                this.State.Settings.Current.MapNameText = name;
                this.State.Settings.Current.GameModeNameText = gameModeName;

                MapModel currentMap = this.State.MapPool.Select(m => m.Value).FirstOrDefault(map => String.Compare(map.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(map.GameMode.Name, this.State.Settings.Current.GameModeNameText, StringComparison.OrdinalIgnoreCase) == 0);

                if (currentMap != null) {
                    this.State.Settings.Current.FriendlyGameModeNameText = currentMap.GameMode.FriendlyName;
                    this.State.Settings.Current.FriendlyMapNameText = currentMap.FriendlyName;

                    IProtocolStateDifference difference = new ProtocolStateDifference() {
                        Modified = {
                            Settings = this.State.Settings
                        }
                    };

                    this.ApplyProtocolStateDifference(difference);

                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolMapChanged,
                        difference,
                        new ProtocolEventData() {
                            Maps = new List<MapModel>() {
                                currentMap
                            }
                        },
                        new ProtocolEventData() {
                            Maps = new List<MapModel>() {
                                oldMap
                            }
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Updates the current and total rounds, firing an events if they have changed.
        /// </summary>
        /// <param name="currentRound">The current round index being played</param>
        /// <param name="totalRounds">The total number of rounds to be played</param>
        protected void UpdateSettingsRound(int currentRound, int totalRounds) {
            var modified = this.State.Settings.Current.RoundIndex != currentRound;

            this.State.Settings.Maximum.RoundIndex = totalRounds;

            if (modified == true) {
                this.State.Settings.Current.RoundIndex = currentRound;

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Settings = this.State.Settings
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolRoundChanged,
                    difference
                );
            }
        }

        #region Dispatching

        public void ServerInfoDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (response != null) {
                IProtocolStateDifference difference = null;
                FrostbiteServerInfo info = new FrostbiteServerInfo().Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1), this.ServerInfoParameters);

                this.UpdateSettingsMap(info.Map, info.GameMode);
                this.UpdateSettingsRound(info.CurrentRound, info.TotalRounds);

                this.State.Settings.Current.ServerNameText = info.ServerName;
                // this.State.Variables.ConnectionState = ConnectionState.Connected; String b = info.ConnectionState;
                this.State.Settings.Current.PlayerCount = info.PlayerCount;
                this.State.Settings.Maximum.PlayerCount = info.MaxPlayerCount;
                //this.State.Settings.RankedEnabled = info.Ranked;
                this.State.Settings.Current.AntiCheatEnabled = info.PunkBuster;
                this.State.Settings.Current.PasswordProtectionEnabled = info.Passworded;
                this.State.Settings.Current.UpTimeMilliseconds = info.ServerUptime * 1000;
                this.State.Settings.Current.RoundTimeMilliseconds = info.RoundTime * 1000;
                this.State.Settings.Current.ModNameText = info.GameMod.ToString();

                if (this.State.MapPool.Count == 0) {
                    ProtocolConfigModel config = null;

                    if (info.GameMod == GameMods.None) {
                        config = ProtocolConfigLoader.Load<ProtocolConfigModel>(this.Options.ConfigDirectory, this.ProtocolType);
                    }
                    else {
                        config = ProtocolConfigLoader.Load<ProtocolConfigModel>(this.Options.ConfigDirectory, new ProtocolType(this.ProtocolType) {
                            Type = String.Format("{0}_{1}", this.ProtocolType, info.GameMod)
                        });
                    }

                    if (config != null) {
                        config.Parse(this);
                    }

                    difference = new ProtocolStateDifference() {
                        Override = true,
                        Modified = this.State
                    };

                    this.ApplyProtocolStateDifference(difference);

                    this.OnProtocolEvent(ProtocolEventType.ProtocolConfigExecuted, difference);
                }

                difference = new ProtocolStateDifference() {
                    Modified = {
                        Settings = this.State.Settings
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolSettingsUpdated,
                    difference,
                    new ProtocolEventData() {
                        Settings = new List<Settings>() {
                            this.State.Settings
                        }
                    }
                );
            }
        }

        public void LoginPlainTextDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (response != null) {
                if (request.Packet.Words.Count >= 2 && response.Packet.Words.Count == 1 && response.Packet.Words[0] == "OK") {
                    // We logged in successfully. Make sure we have events enabled before we announce we are ready though.
                    this.SendEventsEnabledPacket();

                    this.NextAuxiliarySynchronization = DateTime.Now;
                    //this.Synchronize();
                }
            }
        }

        public void LoginHashedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (response != null) {
                if (request.Packet.Words.Count == 1 && response.Packet.Words.Count >= 2) {
                    this.SendRequest("login.hashed", this.GeneratePasswordHash(this.HashToByteArray(response.Packet.Words[1]), this.Options.Password));
                }
                else if (request.Packet.Words.Count >= 2 && response.Packet.Words.Count == 1) {
                    // We logged in successfully. Make sure we have events enabled before we announce we are ready though.

                    this.SendEventsEnabledPacket();

                    this.NextAuxiliarySynchronization = DateTime.Now;
                    //this.Synchronize();
                }
            }
        }

        public void AdminEventsEnabledDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (response != null) {
                if (request.Packet.Words.Count >= 2 && response.Packet.Words.Count == 1 && response.Packet.Words[0] == "OK") {
                    // We logged in successfully and we have bilateral communication established. READY UP!

                    this.Client.ConnectionState = ConnectionState.ConnectionLoggedIn;
                }
            }
        }

        protected virtual void AdminListPlayersFinalize(List<PlayerModel> players) {
            var modified = new ConcurrentDictionary<String, PlayerModel>();

            // 2. Add or update any new players
            foreach (PlayerModel player in players) {
                PlayerModel statePlayer;
                this.State.Players.TryGetValue(player.Uid, out statePlayer);

                player.Ping = player.Ping > 1000 ? 0 : player.Ping;

                if (statePlayer != null) {
                    // Already exists, update with any new information we have.
                    statePlayer.Kills = player.Kills;
                    statePlayer.Score = player.Score;
                    statePlayer.Deaths = player.Deaths;
                    statePlayer.ClanTag = player.ClanTag;
                    statePlayer.Ping = player.Ping;
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

        public virtual void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<PlayerModel> players = FrostbitePlayers.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public void AdminSayDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 3) {
                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolChat,
                    new ProtocolStateDifference(),
                    new ProtocolEventData() {
                        Chats = new List<ChatModel>() {
                            FrostbiteChat.ParseAdminSay(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1))
                        }
                    }
                );
            }

        }

        public void VersionDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 3) {
                this.State.Settings.Current.ServerVersionText = request.Packet.Words[2];
            }
        }

        public virtual void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                ConcurrentDictionary<String, MapModel> modified = new ConcurrentDictionary<String, MapModel>();

                List<MapModel> maps = FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

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

        public virtual void BanListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

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

                List<BanModel> banList = FrostbiteBanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

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

        public void BanListAddDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                BanModel ban = FrostbiteBan.ParseBanAdd(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Bans = new ConcurrentDictionary<String, BanModel>(new Dictionary<String, BanModel>() {
                            { String.Format("{0}/{1}", ban.Scope.Times.First().Context, ban.Scope.Players.First().Uid ?? ban.Scope.Players.First().Name ?? ban.Scope.Players.First().Ip), ban }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerBanned,
                    difference,
                    new ProtocolEventData() {
                        Bans = new List<BanModel>() {
                            ban
                        }
                    }
                );
            }
        }

        public void BanListRemoveDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                BanModel ban = FrostbiteBan.ParseBanRemove(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Removed = {
                        Bans = new ConcurrentDictionary<String, BanModel>(new Dictionary<String, BanModel>() {
                            { String.Format("{0}/{1}", ban.Scope.Times.First().Context, ban.Scope.Players.First().Uid ?? ban.Scope.Players.First().Name ?? ban.Scope.Players.First().Ip), ban }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerUnbanned,
                    difference,
                    new ProtocolEventData() {
                        Bans = new List<BanModel>() {
                            ban
                        }
                    }
                );
            }
        }

        #region Variables

        public void VarsServerNameDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (response.Packet.Words.Count >= 2) {
                this.State.Settings.Current.ServerNameText = response.Packet.Words[1];
            }
        }

        public void VarsGamePasswordDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (response.Packet.Words.Count >= 2) {
                this.State.Settings.Current.PasswordText = response.Packet.Words[1];
            }
        }

        public void VarsGamePunkbusterDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut = false;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.AntiCheatEnabled = boolOut;
            }
        }

        public void VarsHardcoreDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut = false;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.HardcoreEnabled = boolOut;
            }
        }

        public void VarsRankedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut = false;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                //this.State.Settings.RankedEnabled = boolOut;
            }
        }

        public void VarsRankLimitDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            int intOut = 0;
            if (response.Packet.Words.Count >= 2 && int.TryParse(response.Packet.Words[1], out intOut)) {
                this.State.Settings.Maximum.PlayerRank = intOut;
            }
        }

        public void VarsTeamBalanceDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut = false;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.AutoBalanceEnabled = boolOut;
            }
        }

        public void VarsFriendlyFireDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut = false;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.FriendlyFireEnabled = boolOut;
            }
        }

        public void VarsBannerUrlDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (response.Packet.Words.Count >= 2) {
                this.State.Settings.Current.ServerBannerUrlText = response.Packet.Words[1];
            }
        }

        public void VarsServerDescriptionDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (response.Packet.Words.Count >= 2) {
                this.State.Settings.Current.ServerDescriptionText = response.Packet.Words[1];
            }
        }

        public void VarsKillCamDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.KillCameraEnabled = boolOut;
            }
        }

        public void VarsMiniMapDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.MiniMapEnabled = boolOut;
            }
        }

        public void VarsCrossHairDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.CrossHairEnabled = boolOut;
            }
        }

        public void VarsIdleTimeoutDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            int intOut = 0;
            if (response.Packet.Words.Count >= 2 && int.TryParse(response.Packet.Words[1], out intOut)) {
                this.State.Settings.Current.IdleTimeoutEnabled = intOut != -1;

                this.State.Settings.Maximum.IdleTimeoutMilliseconds = intOut * 1000;
            }
        }

        public void VarsProfanityFilterDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            bool boolOut;
            if (response.Packet.Words.Count >= 2 && bool.TryParse(response.Packet.Words[1], out boolOut)) {
                this.State.Settings.Current.ProfanityFilterEnabled = boolOut;
            }
        }

        #endregion

        public void PunkBusterOnMessageDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 1) {

                IPunkBuster pbObject = PunkBusterSerializer.Deserialize(request.Packet.Words[1]);

                if (pbObject is PunkBusterPlayer) {
                    PunkBusterPlayer player = pbObject as PunkBusterPlayer;

                    PlayerModel statePlayer = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == player.Name);

                    if (statePlayer != null) {
                        statePlayer.SlotId = player.SlotId;
                        statePlayer.Ip = player.Ip;

                        Location location = FrostbiteGame.Geolocation.Locate(statePlayer.Ip);

                        if (location != null) {
                            statePlayer.Location = location;
                        }


                    }
                }
                else if (pbObject is PunkBusterBeginPlayerList) {
                    
                }
                else if (pbObject is PunkBusterEndPlayerList) {
                    this.OnProtocolEvent(ProtocolEventType.ProtocolPlayerlistUpdated, new ProtocolStateDifference());
                }
            }
        }

        public virtual void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 11) {

                bool headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {
                    var killer = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
                    var victim = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[2]);

                    IProtocolStateDifference difference = new ProtocolStateDifference() {
                        Modified = {
                            Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                                { killer != null ? killer.Uid : "", killer },
                                { victim != null ? victim.Uid : "", victim }
                            })
                        }
                    };

                    this.ApplyProtocolStateDifference(difference);

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
                                            new ItemModel() {
                                                Name = request.Packet.Words[3]
                                            }
                                        },
                                        Points = new List<Point3DModel>() {
                                            new Point3DModel(request.Packet.Words[8], request.Packet.Words[10], request.Packet.Words[9])
                                        },
                                        HumanHitLocations = new List<HumanHitLocation>() {
                                            headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot
                                        }
                                    },
                                    Now = {
                                        Players = new List<PlayerModel>() {
                                            killer
                                        },
                                        Points = new List<Point3DModel>() {
                                            new Point3DModel(request.Packet.Words[5], request.Packet.Words[7], request.Packet.Words[6])
                                        }
                                    }
                                }
                            }
                        }
                    );
                }
            }
        }

        public void ServerOnLoadingLevelDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 4) {

                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Packet.Words[2], out currentRound) == true && int.TryParse(request.Packet.Words[3], out totalRounds) == true) {

                    this.State.Settings.Current.RoundIndex = currentRound;
                    this.State.Settings.Maximum.RoundIndex = totalRounds;

                    // Maps are the same, only a round change
                    if (String.Compare(this.State.Settings.Current.MapNameText, request.Packet.Words[1], StringComparison.OrdinalIgnoreCase) == 0) {
                        IProtocolStateDifference difference = new ProtocolStateDifference() {
                            Modified = {
                                Settings = this.State.Settings
                            }
                        };

                        this.ApplyProtocolStateDifference(difference);

                        this.OnProtocolEvent(
                            ProtocolEventType.ProtocolRoundChanged,
                            difference
                        );
                    }
                    else {
                        MapModel selectedMap = this.State.MapPool.Select(m => m.Value).FirstOrDefault(x => String.Compare(x.Name, request.Packet.Words[1], StringComparison.OrdinalIgnoreCase) == 0);

                        if (selectedMap != null) {
                            this.State.Settings.Current.GameModeNameText = selectedMap.GameMode.Name;
                            this.State.Settings.Current.FriendlyGameModeNameText = selectedMap.GameMode.FriendlyName;

                            this.State.Settings.Current.FriendlyMapNameText = selectedMap.FriendlyName;
                        }

                        this.State.Settings.Current.MapNameText = request.Packet.Words[1];

                        IProtocolStateDifference difference = new ProtocolStateDifference() {
                            Modified = {
                                Settings = this.State.Settings
                            }
                        };

                        this.ApplyProtocolStateDifference(difference);

                        this.OnProtocolEvent(
                            ProtocolEventType.ProtocolMapChanged,
                            difference
                        );
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Move the PlayerJoin event to the onAuthenticated?
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void PlayerOnJoinDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {
                // todo this is blanked out to follow a "no unique id, no existence" type of policy instead of juggling different states of players
                // todo I need to look into older frostbite games to see if this will present a problem.
                /*
                PlayerModel player = new PlayerModel() {
                    Name = request.Packet.Words[1]
                };

                if (this.State.Players.Find(x => x.Name == player.Name) == null) {
                    this.State.Players.Add(player);
                }
                */
            }
        }

        public void PlayerOnLeaveDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {
                //request.Packet.Words.RemoveAt(1);

                PlayerModel player = FrostbitePlayers.Parse(request.Packet.Words.GetRange(2, request.Packet.Words.Count - 2)).FirstOrDefault();

                if (player != null) {
                    PlayerModel statePlayer = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == player.Name);

                    if (statePlayer != null) {
                        // Already exists, update with any new information we have.
                        // Note: We must keep the same Player object which is why we update and swap
                        // instead of just assigning.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping > 1000 ? 0 : player.Ping;
                        statePlayer.Uid = player.Uid;

                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Team));
                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == GroupModel.Squad));

                        player = statePlayer;
                    }

                    IProtocolStateDifference difference = new ProtocolStateDifference() {
                        Removed = {
                            Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                                { player.Uid, player }
                            })
                        }
                    };

                    this.ApplyProtocolStateDifference(difference);

                    this.OnProtocolEvent(
                        ProtocolEventType.ProtocolPlayerLeave,
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

        public void PlayerOnChatDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            // player.onChat <source soldier name: string> <text: string> <target group: player subset>
            if (request.Packet.Words.Count >= 2) {
                ChatModel chat = FrostbiteChat.ParsePlayerChat(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                // If it was directed towards a specific player.
                if (chat.Scope.Groups != null && chat.Scope.Groups.Any(group => group.Type == GroupModel.Player) == true) {
                    chat.Scope.Players = new List<PlayerModel>() {
                        this.State.Players.Select(p => p.Value).FirstOrDefault(player => player.Uid == chat.Scope.Groups.First(group => @group.Type == GroupModel.Player).Uid)
                    };
                }

                if (chat.Now.Players != null && chat.Now.Players.Count > 0 && this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == chat.Now.Players.First().Name) != null) {
                    chat.Now.Players = new List<PlayerModel>() {
                        this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == chat.Now.Players.First().Name)
                    };
                }
                else {
                    // Couldn't find the player, must be from the server.
                    chat.Origin = NetworkOrigin.Server;
                }

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolChat,
                    new ProtocolStateDifference(),
                    new ProtocolEventData() {
                        Chats = new List<ChatModel>() {
                            chat
                        }
                    }
                );
            }
        }

        public virtual void PlayerOnAuthenticatedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 3) {
                PlayerModel statePlayer = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);

                if (statePlayer != null) {
                    statePlayer.Uid = request.Packet.Words[2];
                }
                else {
                    statePlayer = new PlayerModel() {
                        Name = request.Packet.Words[1],
                        Uid = request.Packet.Words[2]
                    };
                }

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { statePlayer.Uid, statePlayer }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerJoin,
                    difference,
                    new ProtocolEventData() {
                        Players = new List<PlayerModel>() {
                            statePlayer
                        }
                    }
                );
            }
        }

        public void PlayerOnSpawnDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            SpawnModel spawn = FrostbiteSpawn.Parse(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

            PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == spawn.Player.Name);

            if (player != null) {
                player.Role = spawn.Role;
                player.Inventory = spawn.Inventory;

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerSpawn,
                    difference,
                    new ProtocolEventData() {
                        Spawns = new List<SpawnModel>() {
                            spawn
                        }
                    }
                );
            }
        }

        public void PlayerOnKickedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);

            if (player != null) {
                // Note that this is removed when the player.OnLeave event is fired.
                //this.State.PlayerList.RemoveAll(x => x.Name == request.Packet.Words[1]);

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Removed = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerKicked,
                    difference,
                    new ProtocolEventData() {
                        Kicks = new List<KickModel>() {
                            new KickModel() {
                                Now = {
                                    Players = new List<PlayerModel>() {
                                        player
                                    },
                                    Content = new List<String>() {
                                        request.Packet.Words[2]
                                    }
                                }
                            }
                        }
                    }
                );
            }
        }

        public void PlayerOnSquadChangeDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Packet.Words[2], out teamId) == true && int.TryParse(request.Packet.Words[3], out squadId) == true) {

                player.ModifyGroup(new GroupModel() {
                    Type = GroupModel.Squad,
                    Uid = squadId.ToString(CultureInfo.InvariantCulture)
                });

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerMoved,
                    difference,
                    new ProtocolEventData() {
                        Players = new List<PlayerModel>() {
                            player
                        }
                    }
                );
            }
        }

        public void PlayerOnTeamChangeDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            PlayerModel player = this.State.Players.Select(p => p.Value).FirstOrDefault(p => p.Name == request.Packet.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Packet.Words[2], out teamId) == true && int.TryParse(request.Packet.Words[3], out squadId) == true) {
                player.ModifyGroup(new GroupModel() {
                    Type = GroupModel.Team,
                    Uid = teamId.ToString(CultureInfo.InvariantCulture)
                });

                player.ModifyGroup(new GroupModel() {
                    Type = GroupModel.Squad,
                    Uid = squadId.ToString(CultureInfo.InvariantCulture)
                });

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Players = new ConcurrentDictionary<String, PlayerModel>(new Dictionary<String, PlayerModel>() {
                            { player.Uid, player }
                        })
                    }
                };

                this.ApplyProtocolStateDifference(difference);

                this.OnProtocolEvent(
                    ProtocolEventType.ProtocolPlayerMoved,
                    difference,
                    new ProtocolEventData() {
                        Players = new List<PlayerModel>() {
                            player
                        }
                    }
                );
            }
        }

        /// <summary>
        /// Called when a password hash returns an invalid response. We pipe this up as a connection
        /// failure with a invalid password description.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void InvalidPasswordHashDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            this.OnClientEvent(ClientEventType.ClientConnectionFailure, new ClientEventData() {
                Exceptions = new List<String>() {
                    "Invalid password"
                }
            });

            // Shutdown the connection, nothing else to do here but reconnect with a new password.
            this.Shutdown();
        }

        #endregion

        #region Packet Helpers

        #region Frostbite specific

        protected void SendResponse(FrostbitePacket request, params string[] words) {
            this.Send(new FrostbitePacket() {
                Packet = {
                    Origin = request.Packet.Origin,
                    Type = PacketType.Response,
                    RequestId = request.Packet.RequestId,
                    Words = new List<String>(words)
                }
            });
        }

        protected void SendRequest(params string[] words) {
            this.Send(new FrostbitePacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = ((FrostbiteClient)this.Client).AcquireSequenceNumber,
                    Words = new List<String>(words)
                }
            });
        }

        protected virtual void SendEventsEnabledPacket() {
            this.Send(this.CreatePacket("eventsEnabled true"));
        }

        #endregion

        /// <summary>
        /// Create a packet from a string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override IPacketWrapper CreatePacket(string format, params object[] args) {
            String packetText = format;

            try {
                packetText = String.Format(format, args);
            }
            catch {
                packetText = String.Empty;
            }

            return new FrostbitePacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = null,
                    Words = packetText.Wordify()
                }
            };
        }

        /// <summary>
        /// Wraps a completed packet in a packet wrapper, allowing it to be sent to the server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected override IPacketWrapper WrapPacket(IPacket packet) {
            return new FrostbitePacket() {
                Packet = packet
            };
        }

        protected override List<IPacketWrapper> ActionChat(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (action.Now.Content != null) {
                foreach (String chatMessage in action.Now.Content) {
                    String subset = String.Empty;

                    if (action.Scope.Groups == null && action.Scope.Players == null) {
                        subset = "all";
                    }
                    else if (action.Scope.Players != null && action.Scope.Players.Count > 0) {
                        subset = String.Format(@"player ""{0}""", action.Scope.Players.First().Name);
                    }
                    else if (action.Scope.Groups != null && action.Scope.Groups.Any(group => @group.Type == GroupModel.Team) == true) {
                        subset = String.Format("team {0}", action.Scope.Groups.First(group => @group.Type == GroupModel.Team).Uid);
                    }
                    else if (action.Scope.Groups != null && action.Scope.Groups.Any(group => @group.Type == GroupModel.Team) == true && action.Scope.Groups.Any(group => @group.Type == GroupModel.Squad) == true) {
                        subset = String.Format("squad {0} {1}", action.Scope.Groups.First(group => @group.Type == GroupModel.Team).Uid, action.Scope.Groups.First(group => @group.Type == GroupModel.Squad).Uid);
                    }

                    if (action.ActionType == NetworkActionType.NetworkTextSay) {
                        wrappers.Add(this.CreatePacket("admin.say \"{0}\" {1}", chatMessage, subset));
                    }
                    else if (action.ActionType == NetworkActionType.NetworkTextYell || action.ActionType == NetworkActionType.NetworkTextYellOnly) {
                        wrappers.Add(this.CreatePacket("admin.yell \"{0}\" 8000 {1}", chatMessage, subset));
                    }
                }
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionKill(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            String reason = action.Scope.Content != null ? action.Scope.Content.FirstOrDefault() : String.Empty;

            if (action.Scope.Players != null) {
                foreach (PlayerModel target in action.Scope.Players) {
                    wrappers.Add(this.CreatePacket("admin.killPlayer \"{0}\"", target.Name));

                    if (string.IsNullOrEmpty(reason) == false) {
                        wrappers.Add(this.CreatePacket("admin.say \"{0}\" player {1}", reason, target.Name));
                    }
                }
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionKick(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            String reason = action.Scope.Content != null ? action.Scope.Content.FirstOrDefault() : String.Empty;

            foreach (PlayerModel player in action.Scope.Players) {
                wrappers.Add(string.IsNullOrEmpty(reason) == false ? this.CreatePacket("admin.kickPlayer \"{0}\" \"{1}\"", player.Name, reason) : this.CreatePacket("admin.kickPlayer \"{0}\"", player.Name));
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionBan(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            String reason = action.Scope.Content != null ? action.Scope.Content.FirstOrDefault() : String.Empty;
            TimeSubsetModel time = action.Scope.Times != null ? action.Scope.Times.FirstOrDefault() ?? new TimeSubsetModel() : new TimeSubsetModel();

            if (action.ActionType == NetworkActionType.NetworkPlayerBan) {
                if (time.Context == TimeSubsetContext.Permanent) {
                    if (String.IsNullOrEmpty(reason) == true) {
                        wrappers.Add(this.CreatePacket("banList.add guid \"{0}\" perm", action.Scope.Players.First().Uid));
                    }
                    else {
                        wrappers.Add(this.CreatePacket("banList.add guid \"{0}\" perm \"{1}\"", action.Scope.Players.First().Uid, reason));
                    }
                }
                else if (time.Context == TimeSubsetContext.Time && time.Length.HasValue == true) {
                    if (String.IsNullOrEmpty(reason) == true) {
                        wrappers.Add(this.CreatePacket("banList.add guid \"{0}\" seconds {1}", action.Scope.Players.First().Uid, time.Length.Value.TotalSeconds));
                    }
                    else {
                        wrappers.Add(this.CreatePacket("banList.add guid \"{0}\" seconds {1} \"{2}\"", action.Scope.Players.First().Uid, time.Length.Value.TotalSeconds, reason));
                    }
                }
            }
            else if (action.ActionType == NetworkActionType.NetworkPlayerUnban) {
                var player = action.Scope.Players.FirstOrDefault();

                if (player != null) {
                    if (String.IsNullOrEmpty(player.Uid) == false) {
                        wrappers.Add(this.CreatePacket("banList.remove guid \"{0}\"", player.Uid));
                    }
                    else if (String.IsNullOrEmpty(player.Name) == false) {
                        wrappers.Add(this.CreatePacket("banList.remove name \"{0}\"", player.Name));
                    }
                    else if (String.IsNullOrEmpty(player.Ip) == false) {
                        wrappers.Add(this.CreatePacket("banList.remove ip \"{0}\"", player.Ip));
                    }
                }
            }

            wrappers.Add(this.CreatePacket("banList.save"));

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionMove(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (action.Now.Groups == null) {
                action.Now.Groups = new List<GroupModel>();
            }

            if (action.Scope.Players != null) {
                // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
                bool forceMove = (action.ActionType == NetworkActionType.NetworkPlayerMoveForce || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce);

                MapModel selectedMap = this.State.MapPool.Select(m => m.Value).FirstOrDefault(map => String.Compare(map.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0 && map.GameMode != null && String.Compare(map.GameMode.Name, this.State.Settings.Current.GameModeNameText, StringComparison.OrdinalIgnoreCase) == 0);

                foreach (var movePlayer in action.Scope.Players) {
                    // Lookup the player from the state. The command may only include basic information, or just include
                    // the Uid and nothing more.
                    PlayerModel stateMovePlayer;
                    this.State.Players.TryGetValue(movePlayer.Uid, out stateMovePlayer);

                    if (stateMovePlayer != null) {
                        if (selectedMap != null) {
                            // If they are just looking to rotate the player through the teams
                            if (action.ActionType == NetworkActionType.NetworkPlayerMoveRotate || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce) {

                                int currentTeamId = -1;

                                int.TryParse(stateMovePlayer.Groups.First(group => @group.Type == GroupModel.Team).Uid, out currentTeamId);

                                var teams = selectedMap.Groups.Count(group => @group.Type == GroupModel.Team);

                                // Avoid divide by 0 error - shouldn't ever be encountered though.
                                if (selectedMap.GameMode != null && teams > 0) {
                                    int newTeamId = (currentTeamId + 1) % (teams + 1);

                                    action.Now.Groups.Add(new GroupModel() {
                                        Type = GroupModel.Team,
                                        Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                                    });
                                }
                            }

                            // Now check if the destination squad is supported.
                            if (selectedMap.GameMode != null && (selectedMap.GameMode.Name == "SQDM" || selectedMap.GameMode.Name == "SQRUSH")) {
                                if (selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == GroupModel.Squad) != null) {
                                    action.Now.Groups.Add(selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == GroupModel.Squad));
                                }
                            }
                        }

                        // Fix up the team uid
                        if (action.Now.Groups.FirstOrDefault(group => @group.Type == GroupModel.Team) == null) {
                            if (stateMovePlayer.Groups.FirstOrDefault(group => @group.Type == GroupModel.Team) != null) {
                                // No destination team set, use the players current team.
                                action.Now.Groups.Add(stateMovePlayer.Groups.First(group => @group.Type == GroupModel.Team));
                            }
                            else {
                                // Panic, set team uid to 1.
                                action.Now.Groups.Add(new GroupModel() {
                                    Uid = "1"
                                });
                            }
                        }

                        // Fix up the squad uid
                        if (action.Now.Groups.FirstOrDefault(group => @group.Type == GroupModel.Squad) == null) {
                            if (selectedMap != null && selectedMap.GameMode != null && selectedMap.GameMode.DefaultGroups.FirstOrDefault(group => @group.Type == GroupModel.Squad) != null) {
                                action.Now.Groups.Add(selectedMap.GameMode.DefaultGroups.First(group => @group.Type == GroupModel.Squad));
                            }
                            else {
                                action.Now.Groups.Add(new GroupModel() {
                                    Uid = "0"
                                });
                            }
                        }

                        wrappers.Add(this.CreatePacket(
                            "admin.movePlayer \"{0}\" {1} {2} {3}",
                            stateMovePlayer.Name,
                            action.Now.Groups.First(group => @group.Type == GroupModel.Team).Uid,
                            action.Now.Groups.First(group => @group.Type == GroupModel.Squad).Uid,
                            FrostbiteConverter.BoolToString(forceMove)
                        ));
                    }
                }
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            foreach (MapModel map in action.Now.Maps) {
                var closureMap = map;

                if (action.ActionType == NetworkActionType.NetworkMapAppend) {
                    wrappers.Add(this.CreatePacket("mapList.append \"{0}\" {1}", map.Name, map.Rounds));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list rounds"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapChangeMode) {
                    if (map.GameMode != null) {
                        wrappers.Add(this.CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                    }
                }
                else if (action.ActionType == NetworkActionType.NetworkMapInsert) {
                    wrappers.Add(this.CreatePacket("mapList.insert {0} \"{1}\" {2}", map.Index, map.Name, map.Rounds));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list rounds"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRemove) {
                    var matchingMaps = this.State.Maps.Where(m => m.Value.Name == closureMap.Name).OrderByDescending(m => m.Value.Index);

                    wrappers.AddRange(matchingMaps.Select(match => this.CreatePacket("mapList.remove {0}", match.Value.Index)));

                    wrappers.Add(this.CreatePacket("mapList.save"));

                    wrappers.Add(this.CreatePacket("mapList.list rounds"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                    wrappers.Add(this.CreatePacket("mapList.remove {0}", map.Index));

                    wrappers.Add(this.CreatePacket("mapList.list rounds"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapNextIndex) {
                    wrappers.Add(this.CreatePacket("mapList.nextLevelIndex {0}", map.Index));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapRestart || action.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                    wrappers.Add(this.CreatePacket("admin.restartRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapNext || action.ActionType == NetworkActionType.NetworkMapRoundNext) {
                    wrappers.Add(this.CreatePacket("admin.runNextRound"));
                }
                else if (action.ActionType == NetworkActionType.NetworkMapClear) {
                    wrappers.Add(this.CreatePacket("mapList.clear"));

                    wrappers.Add(this.CreatePacket("mapList.save"));
                }
            }

            return wrappers;
        }

        protected override void Login(string password) {
            this.Send(this.CreatePacket("login.hashed"));
        }
        
        #endregion

    }
}
