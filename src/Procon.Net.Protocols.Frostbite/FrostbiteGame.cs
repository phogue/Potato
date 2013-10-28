using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite {
    using Procon.Net.Attributes;
    using Procon.Net.Utils;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;
    using Procon.Net.Protocols.Frostbite.Objects;
    using Procon.Net.Protocols.Objects;

    public abstract class FrostbiteGame : GameImplementation<FrostbitePacket> {

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
            State.Settings.MaxConsoleLines = 100;
        }
        
        protected override Client<FrostbitePacket> CreateClient(string hostName, ushort port) {
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

            if (this.ConnectionState == ConnectionState.ConnectionLoggedIn) {
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

        [DispatchPacket(MatchText = "serverInfo", PacketOrigin = PacketOrigin.Client)]
        public void ServerInfoDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {

                FrostbiteServerInfo info = new FrostbiteServerInfo().Parse(response.Words.GetRange(1, response.Words.Count - 1), this.ServerInfoParameters);

                this.State.Settings.ServerName = info.ServerName;
                this.State.Settings.MapName = info.Map;
                this.State.Settings.GameModeName = info.GameMode;
                // this.State.Variables.ConnectionState = ConnectionState.Connected; String b = info.ConnectionState;
                this.State.Settings.PlayerCount = info.PlayerCount;
                this.State.Settings.MaxPlayerCount = info.MaxPlayerCount;
                this.State.Settings.RoundIndex = info.CurrentRound;
                this.State.Settings.MaxRoundIndex = info.TotalRounds;
                this.State.Settings.RankedEnabled = info.Ranked;
                this.State.Settings.AntiCheatEnabled = info.PunkBuster;
                this.State.Settings.PasswordProtectionEnabled = info.Passworded;
                this.State.Settings.UpTimeSeconds = info.ServerUptime;
                this.State.Settings.RoundTimeSeconds = info.RoundTime;
                this.State.Settings.ModName = info.GameMod.ToString();

                if (info.GameMod == GameMods.None) {
                    this.ExecuteGameConfig(this.GameType.ToLower());
                }
                else {
                    this.ExecuteGameConfig(String.Format("{0}_{1}", this.GameType, info.GameMod).ToLower());
                }


                this.OnGameEvent(GameEventType.GameSettingsUpdated, new GameEventData() {
                    Settings = new List<Settings>() {
                        this.State.Settings
                    }
                });
            }
        }

        [DispatchPacket(MatchText = "login.plainText", PacketOrigin = PacketOrigin.Client)]
        public void LoginPlainTextDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {
                if (request.Words.Count >= 2 && response.Words.Count == 1 && response.Words[0] == "OK") {
                    // We logged in successfully. Make sure we have events enabled before we announce we are ready though.
                    this.SendEventsEnabledPacket();

                    this.NextAuxiliarySynchronization = DateTime.Now;
                    //this.Synchronize();
                }
            }
        }

        [DispatchPacket(MatchText = "login.hashed", PacketOrigin = PacketOrigin.Client)]
        public void LoginHashedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {
                if (request.Words.Count == 1 && response.Words.Count >= 2) {
                    this.SendRequest("login.hashed", this.GeneratePasswordHash(this.HashToByteArray(response.Words[1]), this.Password));
                }
                else if (request.Words.Count >= 2 && response.Words.Count == 1) {
                    // We logged in successfully. Make sure we have events enabled before we announce we are ready though.

                    this.SendEventsEnabledPacket();

                    this.NextAuxiliarySynchronization = DateTime.Now;
                    //this.Synchronize();
                }
            }
        }

        [DispatchPacket(MatchText = "admin.eventsEnabled", PacketOrigin = PacketOrigin.Client)]
        public void AdminEventsEnabledDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {
                if (request.Words.Count >= 2 && response.Words.Count == 1 && response.Words[0] == "OK") {
                    // We logged in successfully and we have bilateral communication established. READY UP!

                    this.Client.ConnectionState = ConnectionState.ConnectionLoggedIn;
                }
            }
        }
        
        protected virtual void AdminListPlayersFinalize(FrostbitePlayerList players) {
            // If no limits on the subset we just fetched.
            if (players.Subset.Count == 0) {

                // 1. Remove all names in the state list that are not found in the new list (players that have left)
                this.State.PlayerList.RemoveAll(x => players.Select(y => y.Name).Contains(x.Name) == false);

                // 2. Add or update any new players
                foreach (Player player in players) {
                    Player statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name);

                    if (statePlayer == null) {
                        this.State.PlayerList.Add(player);
                    }
                    else {
                        // Already exists, update with any new information we have.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Score = player.Score;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        statePlayer.Uid = player.Uid;

                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Team));
                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Squad));
                    }
                }

                this.OnGameEvent(GameEventType.GamePlayerlistUpdated, new GameEventData() {
                    Players = new List<Player>(this.State.PlayerList)
                });
            }
        }

        [DispatchPacket(MatchText = "admin.listPlayers", PacketOrigin = PacketOrigin.Client)]
        public virtual void AdminListPlayersResponseDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            FrostbitePlayerList players = new FrostbitePlayerList() {
                Subset = new FrostbiteGroupingList().Parse(request.Words.GetRange(1, request.Words.Count - 1))
            }.Parse(response.Words.GetRange(1, response.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        [DispatchPacket(MatchText = "admin.say", PacketOrigin = PacketOrigin.Client)]
        public void AdminSayDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 3) {
                this.OnGameEvent(GameEventType.GameChat, new GameEventData() {
                    Chats = new List<Chat>() {
                        FrostbiteChat.ParseAdminSay(request.Words.GetRange(1, request.Words.Count - 1))
                    }
                });
            }

        }

        [DispatchPacket(MatchText = "version", PacketOrigin = PacketOrigin.Client)]
        public void VersionDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 3) {
                this.State.Settings.ServerVersion = request.Words[2];
            }
        }

        [DispatchPacket(MatchText = "mapList.list", PacketOrigin = PacketOrigin.Client)]
        public virtual void MapListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {

                FrostbiteMapList maps = new FrostbiteMapList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

                foreach (Map map in maps) {
                    Map mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, System.StringComparison.OrdinalIgnoreCase) == 0);

                    if (mapInfo != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode     = mapInfo.GameMode;
                    }
                }
                this.State.MapList = maps;

                this.OnGameEvent(
                    GameEventType.GameMaplistUpdated
                );
            }
        }

        [DispatchPacket(MatchText = "banList.list", PacketOrigin = PacketOrigin.Client)]
        public virtual void BanListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

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

                FrostbiteBanList banList = new FrostbiteBanList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

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

        [DispatchPacket(MatchText = "banList.add", PacketOrigin = PacketOrigin.Client)]
        public void BanListAddDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {
                Ban ban = FrostbiteBan.ParseBanAdd(request.Words.GetRange(1, request.Words.Count - 1));

                this.State.BanList.Add(ban);

                this.OnGameEvent(GameEventType.GamePlayerBanned, new GameEventData() { Bans = new List<Ban>() { ban } });
            }
        }

        [DispatchPacket(MatchText = "banList.remove", PacketOrigin = PacketOrigin.Client)]
        public void BanListRemoveDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {
                Ban ban = FrostbiteBan.ParseBanRemove(request.Words.GetRange(1, request.Words.Count - 1));

                Ban stateBan = this.State.BanList.Find(x => (x.Scope.Players.First().Name != null && x.Scope.Players.First().Name == ban.Scope.Players.First().Name)
                                                         || (x.Scope.Players.First().Uid != null && x.Scope.Players.First().Uid == ban.Scope.Players.First().Uid));
                this.State.BanList.Remove(stateBan);

                this.OnGameEvent(GameEventType.GamePlayerUnbanned, new GameEventData() { Bans = new List<Ban>() { ban } });
            }
        }

        #region Variables

        [DispatchPacket(MatchText = "vars.serverName", PacketOrigin = PacketOrigin.Client)]
        public void VarsServerNameDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Settings.ServerName = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.gamePassword", PacketOrigin = PacketOrigin.Client)]
        public void VarsGamePasswordDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Settings.Password = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.punkBuster", PacketOrigin = PacketOrigin.Client)]
        public void VarsGamePunkbusterDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.AntiCheatEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.hardCore", PacketOrigin = PacketOrigin.Client)]
        public void VarsHardcoreDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.HardcoreEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.ranked", PacketOrigin = PacketOrigin.Client)]
        public void VarsRankedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.RankedEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.rankLimit", PacketOrigin = PacketOrigin.Client)]
        public void VarsRankLimitDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            int intOut = 0;
            if (response.Words.Count >= 2 && int.TryParse(response.Words[1], out intOut)) {
                this.State.Settings.RankLimit = intOut;
            }
        }

        [DispatchPacket(MatchText = "vars.teamBalance", PacketOrigin = PacketOrigin.Client)]
        public void VarsTeamBalanceDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.AutoBalanceEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.friendlyFire", PacketOrigin = PacketOrigin.Client)]
        public void VarsFriendlyFireDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.FriendlyFireEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.bannerUrl", PacketOrigin = PacketOrigin.Client)]
        public void VarsBannerUrlDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Settings.BannerUrl = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.serverDescription", PacketOrigin = PacketOrigin.Client)]
        public void VarsServerDescriptionDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Settings.ServerDescription = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.killCam", PacketOrigin = PacketOrigin.Client)]
        public void VarsKillCamDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.KillCameraEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.miniMap", PacketOrigin = PacketOrigin.Client)]
        public void VarsMiniMapDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.MiniMapEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.crossHair", PacketOrigin = PacketOrigin.Client)]
        public void VarsCrossHairDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.CrossHairEnabled = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.idleTimeout", PacketOrigin = PacketOrigin.Client)]
        public void VarsIdleTimeoutDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            int intOut = 0;
            if (response.Words.Count >= 2 && int.TryParse(response.Words[1], out intOut)) {
                this.State.Settings.IdleTimeoutEnabled = intOut != -1;

                this.State.Settings.IdleTimeoutLimitTimeSeconds = intOut;
            }
        }

        [DispatchPacket(MatchText = "vars.profanityFilter", PacketOrigin = PacketOrigin.Client)]
        public void VarsProfanityFilterDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Settings.ProfanityFilterEnabled = boolOut;
            }
        }

        #endregion

        [DispatchPacket(MatchText = "punkBuster.onMessage", PacketOrigin = PacketOrigin.Client)]
        public void PunkBusterOnMessageDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 1) {

                PunkBusterObject pbObject = PB.Parse(request.Words[1]);

                if (pbObject is PunkBusterPlayer) {
                    PunkBusterPlayer player = pbObject as PunkBusterPlayer;

                    Player statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name);

                    if (statePlayer != null) {
                        statePlayer.SlotID = player.SlotID;
                        statePlayer.IP = player.IP;
                    }
                }
                else if (pbObject is PunkBusterBeginPlayerList) {
                    
                }
                else if (pbObject is PunkBusterEndPlayerList) {
                    this.OnGameEvent(GameEventType.GamePlayerlistUpdated);
                }
            }
        }

        [DispatchPacket(MatchText = "player.onKill", PacketOrigin = PacketOrigin.Server)]
        public virtual void PlayerOnKillDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 11) {

                bool headshot = false;

                if (bool.TryParse(request.Words[4], out headshot) == true) {

                    this.OnGameEvent(GameEventType.GamePlayerKill, new GameEventData() {
                        Kills = new List<Kill>() {
                            new Kill() {
                                HumanHitLocation = headshot == true ? FrostbiteGame.Headshot : FrostbiteGame.Bodyshot,
                                Killer = this.State.PlayerList.Find(x => x.Name == request.Words[1]),
                                Target = this.State.PlayerList.Find(x => x.Name == request.Words[2]),
                                DamageType = new Item() {
                                    Name = request.Words[3]
                                },
                                KillerLocation = new Point3D(request.Words[5], request.Words[7], request.Words[6]),
                                TargetLocation = new Point3D(request.Words[8], request.Words[10], request.Words[9])
                            }
                        }
                    });
                }
            }
        }

        [DispatchPacket(MatchText = "server.onLoadingLevel", PacketOrigin = PacketOrigin.Client)]
        public void ServerOnLoadingLevelDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 4) {

                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Words[2], out currentRound) == true && int.TryParse(request.Words[3], out totalRounds) == true) {

                    this.State.Settings.RoundIndex = currentRound;
                    this.State.Settings.MaxRoundIndex = totalRounds;

                    // Maps are the same, only a round change
                    if (String.Compare(this.State.Settings.MapName, request.Words[1], StringComparison.OrdinalIgnoreCase) == 0)
                        this.OnGameEvent(GameEventType.GameRoundChanged);
                    else {
                        Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, request.Words[1], StringComparison.OrdinalIgnoreCase) == 0);

                        if (selectedMap != null)
                            this.State.Settings.GameModeName = selectedMap.GameMode.Name;
                        this.State.Settings.MapName = request.Words[1];
                        this.OnGameEvent(GameEventType.GameMapChanged);
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Move the PlayerJoin event to the onAuthenticated?
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [DispatchPacket(MatchText = "player.onJoin", PacketOrigin = PacketOrigin.Server)]
        public virtual void PlayerOnJoinDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 2) {

                Player player = new Player() {
                    Name = request.Words[1]
                };

                if (this.State.PlayerList.Find(x => x.Name == player.Name) == null) {
                    this.State.PlayerList.Add(player);
                }
            }
        }

        [DispatchPacket(MatchText = "player.onLeave", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnLeaveDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 2) {
                //request.Words.RemoveAt(1);

                Player player = new FrostbitePlayerList().Parse(request.Words.GetRange(2, request.Words.Count - 2)).FirstOrDefault();

                if (player != null) {
                    Player statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name);

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

                    this.State.PlayerList.RemoveAll(x => x.Name == player.Name);

                    this.OnGameEvent(GameEventType.GamePlayerLeave, new GameEventData() {
                        Players = new List<Player>() {
                            player
                        }
                    });
                }
            }
        }

        [DispatchPacket(MatchText = "player.onChat", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnChatDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            // player.onChat <source soldier name: string> <text: string> <target group: player subset>
            if (request.Words.Count >= 2) {
                Chat chat = FrostbiteChat.ParsePlayerChat(request.Words.GetRange(1, request.Words.Count - 1));

                // If it was directed towards a specific player.
                if (chat.Scope.Groups != null && chat.Scope.Groups.Any(group => group.Type == Grouping.Player) == true) {
                    chat.Scope.Players = new List<Player>() {
                        this.State.PlayerList.FirstOrDefault(player => player.Uid == (String)chat.Scope.Groups.First(group => @group.Type == Grouping.Player).Uid)
                    };
                }

                if (chat.Now.Players != null && chat.Now.Players.Count > 0 && this.State.PlayerList.Find(x => x.Name == chat.Now.Players.First().Name) != null) {
                    chat.Now.Players = new List<Player>() {
                        this.State.PlayerList.Find(x => x.Name == chat.Now.Players.First().Name)
                    };
                }
                else {
                    // Couldn't find the player, must be from the server.
                    chat.Origin = ChatOrigin.Server;
                }

                this.OnGameEvent(GameEventType.GameChat, new GameEventData() { Chats = new List<Chat>() { chat } });
            }
        }

        [DispatchPacket(MatchText = "player.onAuthenticated", PacketOrigin = PacketOrigin.Server)]
        public virtual void PlayerOnAuthenticatedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 3) {
                Player statePlayer = this.State.PlayerList.Find(x => x.Name == request.Words[1]);

                if (statePlayer != null) {
                    statePlayer.Uid = request.Words[2];
                }
                else {
                    statePlayer = new Player() {
                        Name = request.Words[1],
                        Uid = request.Words[2]
                    };

                    this.State.PlayerList.Add(statePlayer);
                }

                this.OnGameEvent(GameEventType.GamePlayerJoin, new GameEventData() { Players = new List<Player>() { statePlayer } });
            }
        }

        [DispatchPacket(MatchText = "player.onSpawn", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnSpawnDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            Spawn spawn = FrostbiteSpawn.Parse(request.Words.GetRange(1, request.Words.Count - 1));

            Player player = this.State.PlayerList.Find(x => x.Name == spawn.Player.Name);

            if (player != null) {
                player.Role = spawn.Role;
                player.Inventory = spawn.Inventory;

                this.OnGameEvent(GameEventType.GamePlayerSpawn, new GameEventData() { Spawns = new List<Spawn>() { spawn } });
            }
        }

        [DispatchPacket(MatchText = "player.onKicked", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnKickedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            Player player = this.State.PlayerList.Find(x => x.Name == request.Words[1]);

            if (player != null) {
                // Note that this is removed when the player.OnLeave event is fired.
                //this.State.PlayerList.RemoveAll(x => x.Name == request.Words[1]);

                this.OnGameEvent(GameEventType.GamePlayerKicked, new GameEventData() {
                    Kicks = new List<Kick>() {
                        new Kick() {
                            Target = player,
                            Reason = request.Words[2]
                        }
                    }
                });
            }
        }

        [DispatchPacket(MatchText = "player.onSquadChange", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnSquadChangeDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            Player player = this.State.PlayerList.Find(x => x.Name == request.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Words[2], out teamId) == true && int.TryParse(request.Words[3], out squadId) == true) {

                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Squad,
                    Uid = squadId
                });

                this.OnGameEvent(GameEventType.GamePlayerMoved, new GameEventData() {
                    Players = new List<Player>() {
                        player
                    }
                });
            }
        }

        [DispatchPacket(MatchText = "player.onTeamChange", PacketOrigin = PacketOrigin.Server)]
        public void PlayerOnTeamChangeDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            Player player = this.State.PlayerList.Find(x => x.Name == request.Words[1]);
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Words[2], out teamId) == true && int.TryParse(request.Words[3], out squadId) == true) {
                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Team,
                    Uid = teamId
                });

                player.ModifyGroup(new Grouping() {
                    Type = Grouping.Squad,
                    Uid = squadId
                });

                this.OnGameEvent(GameEventType.GamePlayerMoved, new GameEventData() {
                    Players = new List<Player>() {
                        player
                    }
                });
            }
        }

        [DispatchPacket(MatchText = "InvalidPasswordHash")]
        public void InvalidPasswordHashDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            this.Shutdown();
            //this.Client.ConnectionState = ConnectionState.Ready;
        }

        protected override void Dispatch(FrostbitePacket packet) {

            if (packet.Origin == PacketOrigin.Client && packet.IsResponse == true) {
                FrostbitePacket requestPacket = ((FrostbiteClient)this.Client).GetRequestPacket(packet);

                // If the request packet is valid and has at least one word.
                if (requestPacket != null && requestPacket.Words.Count >= 1) {

                    // If the sent command was successful
                    if (packet.Words.Count >= 1 && String.CompareOrdinal(packet.Words[0], FrostbitePacket.StringResponseOkay) == 0) {
                        this.Dispatch(new DispatchPacketAttribute() {
                            MatchText = requestPacket.Words[0],
                            PacketOrigin = requestPacket.Origin
                        }, requestPacket, packet);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(new DispatchPacketAttribute() {
                            MatchText = packet.Words[0],
                            PacketOrigin = packet.Origin
                        }, requestPacket, packet);
                    }
                }

            }
            else if (packet.Words.Count >= 1 && packet.Origin == PacketOrigin.Server && packet.IsResponse == false) {
                this.Dispatch(new DispatchPacketAttribute() {
                    MatchText = packet.Words[0],
                    PacketOrigin = packet.Origin
                }, packet, null);
            }
        }

        #endregion

        #region Packet Helpers

        #region Frostbite specific

        protected void SendResponse(FrostbitePacket request, params string[] words) {
            this.Send(new FrostbitePacket(request.Origin, true, request.SequenceId, words));
        }

        protected void SendRequest(params string[] words) {
            this.Send(new FrostbitePacket(PacketOrigin.Client, false, ((FrostbiteClient)this.Client).AcquireSequenceNumber, words));
        }

        protected virtual void SendEventsEnabledPacket() {
            this.Send(this.CreatePacket("eventsEnabled true"));
        }

        #endregion

        protected override FrostbitePacket CreatePacket(string format, params object[] args) {
            String packetText = format;

            try {
                packetText = String.Format(format, args);
            }
            catch {
                packetText = String.Empty;
            }

            return new FrostbitePacket(PacketOrigin.Client, false, null, packetText.Wordify());
        }

        protected override void Action(Chat chat) {
            if (chat.Now.Content != null) {
                foreach (String chatMessage in chat.Now.Content) {
                    String subset = String.Empty;

                    if (chat.Scope.Groups == null && chat.Scope.Players == null) {
                        subset = "all";
                    }
                    else if (chat.Scope.Players != null && chat.Scope.Players.Count > 0) {
                        subset = String.Format(@"player ""{0}""", chat.Scope.Players.First().Name);
                    }
                    else if (chat.Scope.Groups != null && chat.Scope.Groups.Any(group => @group.Type == Grouping.Team) == true) {
                        subset = String.Format("team {0}", chat.Scope.Groups.First(group => @group.Type == Grouping.Team).Uid);
                    }
                    else if (chat.Scope.Groups != null && chat.Scope.Groups.Any(group => @group.Type == Grouping.Team) == true && chat.Scope.Groups.Any(group => @group.Type == Grouping.Squad) == true) {
                        subset = String.Format("squad {0} {1}", chat.Scope.Groups.First(group => @group.Type == Grouping.Team).Uid, chat.Scope.Groups.First(group => @group.Type == Grouping.Squad).Uid);
                    }

                    if (chat.ActionType == NetworkActionType.NetworkSay) {
                        this.Send(this.CreatePacket("admin.say \"{0}\" {1}", chatMessage, subset));
                    }
                    else if (chat.ActionType == NetworkActionType.NetworkYell || chat.ActionType == NetworkActionType.NetworkYellOnly) {
                        this.Send(this.CreatePacket("admin.yell \"{0}\" 8000 {1}", chatMessage, subset));
                    }
                }
            }
        }

        protected override void Action(Kill kill) {
            if (kill.Target != null) {
                this.Send(this.CreatePacket("admin.killPlayer \"{0}\"", kill.Target.Name));

                if (string.IsNullOrEmpty(kill.Reason) == false) {
                    this.Send(this.CreatePacket("admin.say \"{0}\" player {1}", kill.Reason, kill.Target.Name));
                }
            }
        }

        protected override void Action(Kick kick) {
            if (kick.Target != null) {
                if (string.IsNullOrEmpty(kick.Reason) == false) {
                    this.Send(this.CreatePacket("admin.kickPlayer \"{0}\" \"{1}\"", kick.Target.Name, kick.Reason));
                }
                else {
                    this.Send(this.CreatePacket("admin.kickPlayer \"{0}\"", kick.Target.Name));
                }
            }
        }

        protected override void Action(Ban ban) {
            if (ban.ActionType == NetworkActionType.NetworkBan) {
                if (ban.Time.Context == TimeSubsetContext.Permanent) {
                    if (ban.Reason.Length == 0) {
                        this.Send(this.CreatePacket("banList.add guid \"{0}\" perm", ban.Scope.Players.First().Uid));
                    }
                    else {
                        this.Send(this.CreatePacket("banList.add guid \"{0}\" perm \"{1}\"", ban.Scope.Players.First().Uid, ban.Reason));
                    }
                }
                else if (ban.Time.Context == TimeSubsetContext.Time && ban.Time.Length.HasValue == true) {
                    if (ban.Reason.Length == 0) {
                        this.Send(this.CreatePacket("banList.add guid \"{0}\" seconds {1}", ban.Scope.Players.First().Uid, ban.Time.Length.Value.TotalSeconds));
                    }
                    else {
                        this.Send(this.CreatePacket("banList.add guid \"{0}\" seconds {1} \"{2}\"", ban.Scope.Players.First().Uid, ban.Time.Length.Value.TotalSeconds, ban.Reason));
                    }
                }
            }
            else if (ban.ActionType == NetworkActionType.NetworkUnban) {
                this.Send(this.CreatePacket("banList.remove guid \"{0}\"", ban.Scope.Players.First().Uid));
            }

            this.Send(this.CreatePacket("banList.save"));
        }

        protected override void Action(Map map) {

            if (map.ActionType == NetworkActionType.NetworkMapAppend) {
                this.Send(this.CreatePacket("mapList.append \"{0}\" {1}", map.Name, map.Rounds));

                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list rounds"));
            }
            // Added by Imisnew2 - You should check this phogue!
            else if (map.ActionType == NetworkActionType.NetworkMapChangeMode) {
                if (map.GameMode != null) {
                    this.Send(this.CreatePacket("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                }
            }
            else if (map.ActionType == NetworkActionType.NetworkMapInsert) {
                this.Send(this.CreatePacket("mapList.insert {0} \"{1}\" {2}", map.Index, map.Name, map.Rounds));

                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list rounds"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemove) {
                var matchingMaps = this.State.MapList.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                foreach (Map match in matchingMaps) {
                    this.Send(this.CreatePacket("mapList.remove {0}", match.Index));
                }

                this.Send(this.CreatePacket("mapList.save"));

                this.Send(this.CreatePacket("mapList.list rounds"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRemoveIndex) {
                this.Send(this.CreatePacket("mapList.remove {0}", map.Index));

                this.Send(this.CreatePacket("mapList.list rounds"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNextIndex) {
                this.Send(this.CreatePacket("mapList.nextLevelIndex {0}", map.Index));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapRestart || map.ActionType == NetworkActionType.NetworkMapRoundRestart) {
                this.Send(this.CreatePacket("admin.restartRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapNext || map.ActionType == NetworkActionType.NetworkMapRoundNext) {
                this.Send(this.CreatePacket("admin.runNextRound"));
            }
            else if (map.ActionType == NetworkActionType.NetworkMapClear) {
                this.Send(this.CreatePacket("mapList.clear"));

                this.Send(this.CreatePacket("mapList.save"));
            }
        }

        public override void Login(string password) {
            this.Send(this.CreatePacket("login.hashed"));
        }
        
        #endregion

    }
}
