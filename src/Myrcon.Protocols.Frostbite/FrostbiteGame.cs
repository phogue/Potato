using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Myrcon.Protocols.Frostbite.Objects;
using Procon.Net;
using Procon.Net.Protocols.PunkBuster;
using Procon.Net.Protocols.PunkBuster.Packets;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Myrcon.Protocols.Frostbite {
    public abstract class FrostbiteGame : Protocol {

        protected const HumanHitLocation Headshot = HumanHitLocation.Head | HumanHitLocation.Neck;

        protected const HumanHitLocation Bodyshot = HumanHitLocation.LeftHand | HumanHitLocation.LeftFoot | HumanHitLocation.RightHand | HumanHitLocation.RightFoot | HumanHitLocation.LowerLeftArm | HumanHitLocation.LowerLeftLeg | HumanHitLocation.LowerRightArm | HumanHitLocation.LowerRightLeg | HumanHitLocation.UpperLeftArm | HumanHitLocation.UpperLeftLeg | HumanHitLocation.UpperRightArm | HumanHitLocation.UpperRightLeg | HumanHitLocation.LowerTorso | HumanHitLocation.UpperTorso;

        protected List<String> ServerInfoParameters = new List<String>();

        /// <summary>
        /// Date for the next sync of the banlist/maplist/pb list. Everything that
        /// does not change often, but we sync to make sure changes applied
        /// from other tools will be updated in a timely manner.
        /// </summary>
        protected DateTime NextAuxiliarySynchronization = DateTime.Now;

        protected FrostbiteGame(string hostName, ushort port) : base(hostName, port) {
            State.Settings.Maximum.ChatLinesCount = 100;
            
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

        protected override IPacketDispatcher CreatePacketDispatcher() {
            return new FrostbitePacketDispatcher() {
                PacketQueue = ((FrostbiteClient)this.Client).PacketQueue
            };
        }

        protected override IClient CreateClient(string hostName, ushort port) {
            return new FrostbiteClient(hostName, port);
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

        #region Dispatching

        public void ServerInfoDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (response != null) {

                FrostbiteServerInfo info = new FrostbiteServerInfo().Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1), this.ServerInfoParameters);

                this.State.Settings.Current.ServerNameText = info.ServerName;
                this.State.Settings.Current.MapNameText = info.Map;
                this.State.Settings.Current.GameModeNameText = info.GameMode;
                // this.State.Variables.ConnectionState = ConnectionState.Connected; String b = info.ConnectionState;
                this.State.Settings.Current.PlayerCount = info.PlayerCount;
                this.State.Settings.Maximum.PlayerCount = info.MaxPlayerCount;
                this.State.Settings.Current.RoundIndex = info.CurrentRound;
                this.State.Settings.Maximum.RoundIndex = info.TotalRounds;
                //this.State.Settings.RankedEnabled = info.Ranked;
                this.State.Settings.Current.AntiCheatEnabled = info.PunkBuster;
                this.State.Settings.Current.PasswordProtectionEnabled = info.Passworded;
                this.State.Settings.Current.UpTimeMilliseconds = info.ServerUptime * 1000;
                this.State.Settings.Current.RoundTimeMilliseconds = info.RoundTime * 1000;
                this.State.Settings.Current.ModNameText = info.GameMod.ToString();

                if (this.State.MapPool.Count == 0) {
                    if (info.GameMod == GameMods.None) {
                        ProtocolConfigLoader.Load<ProtocolConfigModel>(this.ProtocolsConfigDirectory, this.ProtocolType).Parse(this);
                    }
                    else {
                        ProtocolConfigLoader.Load<ProtocolConfigModel>(this.ProtocolsConfigDirectory, new ProtocolType(this.ProtocolType) {
                            Type = String.Format("{0}_{1}", this.ProtocolType, info.GameMod)
                        }).Parse(this);
                    }

                    this.OnGameEvent(ProtocolEventType.ProtocolConfigExecuted);
                }

                this.OnGameEvent(ProtocolEventType.ProtocolSettingsUpdated, new ProtocolEventData() {
                    Settings = new List<Settings>() {
                        this.State.Settings
                    }
                });
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
                    this.SendRequest("login.hashed", this.GeneratePasswordHash(this.HashToByteArray(response.Packet.Words[1]), this.Password));
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
        
        protected virtual void AdminListPlayersFinalize(List<Player> players) {

            // 1. Remove all names in the state list that are not found in the new list (players that have left)
            this.State.Players.RemoveAll(x => players.Select(y => y.Name).Contains(x.Name) == false);

            // 2. Add or update any new players
            foreach (Player player in players) {
                Player statePlayer = this.State.Players.Find(x => x.Name == player.Name);

                if (statePlayer == null) {
                    this.State.Players.Add(player);
                }
                else {
                    // Already exists, update with any new information we have.
                    statePlayer.Kills = player.Kills;
                    statePlayer.Score = player.Score;
                    statePlayer.Deaths = player.Deaths;
                    statePlayer.ClanTag = player.ClanTag;
                    statePlayer.Ping = Math.Min(player.Ping, 1000);
                    statePlayer.Uid = player.Uid;
                    
                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Team));
                    statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Squad));
                }
            }

            this.OnGameEvent(ProtocolEventType.ProtocolPlayerlistUpdated, new ProtocolEventData() {
                Players = new List<Player>(this.State.Players)
            });
        }

        public virtual void AdminListPlayersResponseDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            List<Player> players = FrostbitePlayers.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        public void AdminSayDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 3) {
                this.OnGameEvent(ProtocolEventType.ProtocolChat, new ProtocolEventData() {
                    Chats = new List<Chat>() {
                        FrostbiteChat.ParseAdminSay(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1))
                    }
                });
            }

        }

        public void VersionDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 3) {
                this.State.Settings.Current.ServerVersionText = request.Packet.Words[2];
            }
        }

        public virtual void MapListListDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {

                List<Map> maps = FrostbiteMapList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

                foreach (Map map in maps) {
                    Map mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, System.StringComparison.OrdinalIgnoreCase) == 0);

                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode     = mapInfo.GameMode;
                    }
                }
                this.State.Maps = maps;

                this.OnGameEvent(
                    ProtocolEventType.ProtocolMaplistUpdated
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

                List<Ban> banList = FrostbiteBanList.Parse(response.Packet.Words.GetRange(1, response.Packet.Words.Count - 1));

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

        public void BanListAddDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                Ban ban = FrostbiteBan.ParseBanAdd(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                this.State.Bans.Add(ban);

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerBanned, new ProtocolEventData() { Bans = new List<Ban>() { ban } });
            }
        }

        public void BanListRemoveDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            if (request.Packet.Words.Count >= 1) {
                Ban ban = FrostbiteBan.ParseBanRemove(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                Ban stateBan = this.State.Bans.Find(b => (b.Scope.Players.First().Name != null && b.Scope.Players.First().Name == ban.Scope.Players.First().Name)
                                                         || (b.Scope.Players.First().Uid != null && b.Scope.Players.First().Uid == ban.Scope.Players.First().Uid));
                this.State.Bans.Remove(stateBan);

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerUnbanned, new ProtocolEventData() { Bans = new List<Ban>() { ban } });
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

                    Player statePlayer = this.State.Players.Find(x => x.Name == player.Name);

                    if (statePlayer != null) {
                        statePlayer.SlotId = player.SlotId;
                        statePlayer.Ip = player.Ip;
                    }
                }
                else if (pbObject is PunkBusterBeginPlayerList) {
                    
                }
                else if (pbObject is PunkBusterEndPlayerList) {
                    this.OnGameEvent(ProtocolEventType.ProtocolPlayerlistUpdated);
                }
            }
        }

        public virtual void PlayerOnKillDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 11) {

                bool headshot = false;

                if (bool.TryParse(request.Packet.Words[4], out headshot) == true) {

                    this.OnGameEvent(ProtocolEventType.ProtocolPlayerKill, new ProtocolEventData() {
                        Kills = new List<Kill>() {
                            new Kill() {
                                Scope = {
                                    Players = new List<Player>() {
                                        this.State.Players.Find(x => x.Name == request.Packet.Words[2])
                                    },
                                    Items = new List<Item>() {
                                        new Item() {
                                            Name = request.Packet.Words[3]
                                        }
                                    },
                                    Points = new List<Point3D>() {
                                        new Point3D(request.Packet.Words[8], request.Packet.Words[10], request.Packet.Words[9])
                                    },
                                    HumanHitLocations = new List<HumanHitLocation>() {
                                        headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot
                                    }
                                },
                                Now = {
                                    Players = new List<Player>() {
                                        this.State.Players.Find(x => x.Name == request.Packet.Words[1])
                                    },
                                    Points = new List<Point3D>() {
                                        new Point3D(request.Packet.Words[5], request.Packet.Words[7], request.Packet.Words[6])
                                    }
                                }
                            }
                        }
                    });
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
                    if (String.Compare(this.State.Settings.Current.MapNameText, request.Packet.Words[1], StringComparison.OrdinalIgnoreCase) == 0)
                        this.OnGameEvent(ProtocolEventType.ProtocolRoundChanged);
                    else {
                        Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, request.Packet.Words[1], StringComparison.OrdinalIgnoreCase) == 0);

                        if (selectedMap != null)
                            this.State.Settings.Current.GameModeNameText = selectedMap.GameMode.Name;
                        this.State.Settings.Current.MapNameText = request.Packet.Words[1];
                        this.OnGameEvent(ProtocolEventType.ProtocolMapChanged);
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

                Player player = new Player() {
                    Name = request.Packet.Words[1]
                };

                if (this.State.Players.Find(x => x.Name == player.Name) == null) {
                    this.State.Players.Add(player);
                }
            }
        }

        public void PlayerOnLeaveDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 2) {
                //request.Packet.Words.RemoveAt(1);

                Player player = FrostbitePlayers.Parse(request.Packet.Words.GetRange(2, request.Packet.Words.Count - 2)).FirstOrDefault();

                if (player != null) {
                    Player statePlayer = this.State.Players.Find(x => x.Name == player.Name);

                    if (statePlayer != null) {
                        // Already exists, update with any new information we have.
                        // Note: We must keep the same Player object which is why we update and swap
                        // instead of just assigning.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        statePlayer.Uid = player.Uid;

                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Team));
                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Squad));

                        player = statePlayer;
                    }

                    this.State.Players.RemoveAll(x => x.Name == player.Name);

                    this.OnGameEvent(ProtocolEventType.ProtocolPlayerLeave, new ProtocolEventData() {
                        Players = new List<Player>() {
                            player
                        }
                    });
                }
            }
        }

        public void PlayerOnChatDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            // player.onChat <source soldier name: string> <text: string> <target group: player subset>
            if (request.Packet.Words.Count >= 2) {
                Chat chat = FrostbiteChat.ParsePlayerChat(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

                // If it was directed towards a specific player.
                if (chat.Scope.Groups != null && chat.Scope.Groups.Any(group => group.Type == Grouping.Player) == true) {
                    chat.Scope.Players = new List<Player>() {
                        this.State.Players.FirstOrDefault(player => player.Uid == chat.Scope.Groups.First(group => @group.Type == Grouping.Player).Uid)
                    };
                }

                if (chat.Now.Players != null && chat.Now.Players.Count > 0 && this.State.Players.Find(x => x.Name == chat.Now.Players.First().Name) != null) {
                    chat.Now.Players = new List<Player>() {
                        this.State.Players.Find(x => x.Name == chat.Now.Players.First().Name)
                    };
                }
                else {
                    // Couldn't find the player, must be from the server.
                    chat.Origin = NetworkOrigin.Server;
                }

                this.OnGameEvent(ProtocolEventType.ProtocolChat, new ProtocolEventData() { Chats = new List<Chat>() { chat } });
            }
        }

        public virtual void PlayerOnAuthenticatedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            if (request.Packet.Words.Count >= 3) {
                Player statePlayer = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);

                if (statePlayer != null) {
                    statePlayer.Uid = request.Packet.Words[2];
                }
                else {
                    statePlayer = new Player() {
                        Name = request.Packet.Words[1],
                        Uid = request.Packet.Words[2]
                    };

                    this.State.Players.Add(statePlayer);
                }

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerJoin, new ProtocolEventData() { Players = new List<Player>() { statePlayer } });
            }
        }

        public void PlayerOnSpawnDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            Spawn spawn = FrostbiteSpawn.Parse(request.Packet.Words.GetRange(1, request.Packet.Words.Count - 1));

            Player player = this.State.Players.Find(x => x.Name == spawn.Player.Name);

            if (player != null) {
                player.Role = spawn.Role;
                player.Inventory = spawn.Inventory;

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerSpawn, new ProtocolEventData() { Spawns = new List<Spawn>() { spawn } });
            }
        }

        public void PlayerOnKickedDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            Player player = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);

            if (player != null) {
                // Note that this is removed when the player.OnLeave event is fired.
                //this.State.PlayerList.RemoveAll(x => x.Name == request.Packet.Words[1]);

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerKicked, new ProtocolEventData() {
                    Kicks = new List<Kick>() {
                        new Kick() {
                            Now = {
                                Players = new List<Player>() {
                                    player
                                },
                                Content = new List<String>() {
                                    request.Packet.Words[2]
                                }
                            }
                        }
                    }
                });
            }
        }

        public void PlayerOnSquadChangeDispatchHandler(IPacketWrapper request, IPacketWrapper response) {

            Player player = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Packet.Words[2], out teamId) == true && int.TryParse(request.Packet.Words[3], out squadId) == true) {

                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Squad,
                    Uid = squadId.ToString(CultureInfo.InvariantCulture)
                });

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerMoved, new ProtocolEventData() {
                    Players = new List<Player>() {
                        player
                    }
                });
            }
        }

        public void PlayerOnTeamChangeDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            Player player = this.State.Players.Find(x => x.Name == request.Packet.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Packet.Words[2], out teamId) == true && int.TryParse(request.Packet.Words[3], out squadId) == true) {
                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Team,
                    Uid = teamId.ToString(CultureInfo.InvariantCulture)
                });

                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Squad,
                    Uid = squadId.ToString(CultureInfo.InvariantCulture)
                });

                this.OnGameEvent(ProtocolEventType.ProtocolPlayerMoved, new ProtocolEventData() {
                    Players = new List<Player>() {
                        player
                    }
                });
            }
        }

        public void InvalidPasswordHashDispatchHandler(IPacketWrapper request, IPacketWrapper response) {
            this.Shutdown();
            //this.Client.ConnectionState = ConnectionState.Ready;
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
                    else if (action.Scope.Groups != null && action.Scope.Groups.Any(group => @group.Type == Grouping.Team) == true) {
                        subset = String.Format("team {0}", action.Scope.Groups.First(group => @group.Type == Grouping.Team).Uid);
                    }
                    else if (action.Scope.Groups != null && action.Scope.Groups.Any(group => @group.Type == Grouping.Team) == true && action.Scope.Groups.Any(group => @group.Type == Grouping.Squad) == true) {
                        subset = String.Format("squad {0} {1}", action.Scope.Groups.First(group => @group.Type == Grouping.Team).Uid, action.Scope.Groups.First(group => @group.Type == Grouping.Squad).Uid);
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
                foreach (Player target in action.Scope.Players) {
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

            foreach (Player player in action.Scope.Players) {
                wrappers.Add(string.IsNullOrEmpty(reason) == false ? this.CreatePacket("admin.kickPlayer \"{0}\" \"{1}\"", player.Name, reason) : this.CreatePacket("admin.kickPlayer \"{0}\"", player.Name));
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionBan(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            String reason = action.Scope.Content != null ? action.Scope.Content.FirstOrDefault() : String.Empty;
            TimeSubset time = action.Scope.Times != null ? action.Scope.Times.FirstOrDefault() ?? new TimeSubset() : new TimeSubset();

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
                wrappers.Add(this.CreatePacket("banList.remove guid \"{0}\"", action.Scope.Players.First().Uid));
            }

            wrappers.Add(this.CreatePacket("banList.save"));

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionMove(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (action.Scope.Players != null) {
                // admin.movePlayer <name: player name> <teamId: Team ID> <squadId: Squad ID> <forceKill: boolean>
                bool forceMove = (action.ActionType == NetworkActionType.NetworkPlayerMoveForce || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce);

                Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, this.State.Settings.Current.MapNameText, StringComparison.OrdinalIgnoreCase) == 0);

                foreach (Player movePlayer in action.Scope.Players.Select(scopePlayer => this.State.Players.First(player => player.Uid == scopePlayer.Uid)).Where(movePlayer => movePlayer != null)) {
                    if (selectedMap != null) {
                        // If they are just looking to rotate the player through the teams
                        if (action.ActionType == NetworkActionType.NetworkPlayerMoveRotate || action.ActionType == NetworkActionType.NetworkPlayerMoveRotateForce) {

                            int currentTeamId = -1;

                            int.TryParse(movePlayer.Groups.First(group => @group.Type == Grouping.Team).Uid, out currentTeamId);

                            // Avoid divide by 0 error - shouldn't ever be encountered though.
                            if (selectedMap.GameMode != null && selectedMap.GameMode.TeamCount > 0) {
                                int newTeamId = (currentTeamId + 1) % (selectedMap.GameMode.TeamCount + 1);

                                action.Now.Groups.Add(new Grouping() {
                                    Type = Grouping.Team,
                                    Uid = newTeamId == 0 ? "1" : newTeamId.ToString(CultureInfo.InvariantCulture)
                                });
                            }
                        }

                        // Now check if the destination squad is supported.
                        if (selectedMap.GameMode != null && (selectedMap.GameMode.Name == "SQDM" || selectedMap.GameMode.Name == "SQRUSH")) {
                            if (selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == Grouping.Squad) != null) {
                                action.Now.Groups.Add(selectedMap.GameMode.DefaultGroups.Find(group => @group.Type == Grouping.Squad));
                            }
                        }
                    }

                    wrappers.Add(this.CreatePacket(
                        "admin.movePlayer \"{0}\" {1} {2} {3}",
                        movePlayer.Name,
                        action.Now.Groups.First(group => @group.Type == Grouping.Team).Uid,
                        action.Now.Groups.First(group => @group.Type == Grouping.Squad).Uid,
                        FrostbiteConverter.BoolToString(forceMove)
                    ));
                }
            }

            return wrappers;
        }

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            foreach (Map map in action.Now.Maps) {
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
                    var matchingMaps = this.State.Maps.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                    wrappers.AddRange(matchingMaps.Select(match => this.CreatePacket("mapList.remove {0}", match.Index)));

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
