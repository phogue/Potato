// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite {
    using Procon.Net.Attributes;
    using Procon.Net.Utils;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;
    using Procon.Net.Protocols.Frostbite.Objects;
    using Procon.Net.Protocols.Objects;

    public abstract class FrostbiteGame : GameImplementation<FrostbiteClient, FrostbitePacket>
    {
        #region Frostbite specific keys used to access game server information.

        protected static readonly string C_MAP_PACK         = "frostbite.MapPack";
        protected static readonly string C_MOD              = "frostbite.Mod";
        protected static readonly string C_HARDCORE         = "frostbite.Hardcore";
        protected static readonly string C_RANK_LIMIT       = "frostbite.RankLimit";
        protected static readonly string C_KILL_CAM         = "frostbite.KillCam";
        protected static readonly string C_MINI_MAP         = "frostbite.MiniMap";
        protected static readonly string C_CROSS_HAIR       = "frostbite.CrossHair";
        protected static readonly string C_IDLE_TIMEOUT     = "frostbite.IdleTimeout";
        protected static readonly string C_PROFANITY_FILTER = "frostbite.ProfanityFilter";

        #endregion

        private static readonly HitLocation HEADSHOT = HitLocation.Head         | HitLocation.Neck;
        private static readonly HitLocation BODYSHOT = HitLocation.LeftHand     | HitLocation.LeftFoot     | HitLocation.RightHand     | HitLocation.RightFoot     |
                                                       HitLocation.LowerLeftArm | HitLocation.LowerLeftLeg | HitLocation.LowerRightArm | HitLocation.LowerRightLeg | 
                                                       HitLocation.UpperLeftArm | HitLocation.UpperLeftLeg | HitLocation.UpperRightArm | HitLocation.UpperRightLeg |
                                                       HitLocation.LowerTorso   | HitLocation.UpperTorso;

        protected List<String> ServerInfoParameters = new List<String>();

        public FrostbiteGame(string hostName, ushort port) : base(hostName, port) {
            State.Variables[C_MAP_PACK]     = new DataVariable(C_MAP_PACK,     null, true,  "Variables.Frostbite.MAP_PACK",     "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_MOD]          = new DataVariable(C_MOD,          null, true,  "Variables.Frostbite.MOD",          "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_HARDCORE]     = new DataVariable(C_HARDCORE,     null, false, "Variables.Frostbite.HARDCORE",     "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_RANK_LIMIT]   = new DataVariable(C_RANK_LIMIT,   null, false, "Variables.Frostbite.RANK_LIMIT",   "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_KILL_CAM]     = new DataVariable(C_KILL_CAM,     null, false, "Variables.Frostbite.KILL_CAM",     "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_MINI_MAP]     = new DataVariable(C_MINI_MAP,     null, false, "Variables.Frostbite.MINI_MAP",     "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_CROSS_HAIR]   = new DataVariable(C_CROSS_HAIR,   null, false, "Variables.Frostbite.CROSS_HAIR",   "Variables.Frostbite.MAP_PACK_Description");
            State.Variables[C_IDLE_TIMEOUT] = new DataVariable(C_IDLE_TIMEOUT, null, false, "Variables.Frostbite.IDLE_TIMEOUT", "Variables.Frostbite.MAP_PACK_Description");


            State.Variables.MaxConsoleLines = 100;
        }
        
        protected override Client<FrostbitePacket> CreateClient(string hostName, ushort port) {
            return new FrostbiteClient(hostName, port);
        }

        private DateTime nextLargeSync = DateTime.Now;
        public override void Synchronize() {
            this.Send(this.Create("admin.listPlayers all"));
            this.Send(this.Create("serverInfo"));

            if (DateTime.Now >= nextLargeSync) {
                this.Send(this.Create("punkBuster.pb_sv_command pb_sv_plist"));
                // this.Send(this.Create("mapList.list rounds")); BF3 doesn't take the "rounds" on the end.
                this.Send(this.Create("mapList.list"));
                this.Send(this.Create("banList.list"));

                nextLargeSync = DateTime.Now.AddSeconds(120);
            }
        }

        public string GeneratePasswordHash(byte[] a_salt, string data) {
            byte[] a_combined = new byte[a_salt.Length + data.Length];
            a_salt.CopyTo(a_combined, 0);
            Encoding.ASCII.GetBytes(data).CopyTo(a_combined, a_salt.Length);

            return MD5.Data(a_combined).ToUpper();
        }

        public byte[] HashToByteArray(string hexString) {
            byte[] a_returnHash = new byte[hexString.Length / 2];

            for (int i = 0; i < a_returnHash.Length; i++) {
                a_returnHash[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return a_returnHash;
        }

        #region Config

        protected override void ExecuteGameConfigGamemode(XElement gamemode) {
            this.State.GameModePool.Add(new FrostbiteGameMode().Deserialize(gamemode));
        }

        #endregion

        #region Dispatching

        [DispatchPacket(MatchText = "serverInfo")]
        public void ServerInfoDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {

                FrostbiteServerInfo info = new FrostbiteServerInfo().Parse(response.Words.GetRange(1, response.Words.Count - 1), this.ServerInfoParameters);

                this.State.Variables.ServerName      = info.ServerName;
                this.State.Variables.MapName         = info.Map;
                this.State.Variables.GameModeName    = info.GameMode;
                // this.State.Variables.ConnectionState = ConnectionState.Connected; String b = info.ConnectionState;
                this.State.Variables.PlayerCount     = info.PlayerCount;
                this.State.Variables.MaxPlayerCount  = info.MaxPlayerCount;
                this.State.Variables.RoundIndex      = info.CurrentRound;
                this.State.Variables.MaxRoundIndex   = info.TotalRounds;
                this.State.Variables.Ranked          = info.Ranked;
                this.State.Variables.AntiCheat       = info.PunkBuster;
                this.State.Variables.Passworded      = info.Passworded;
                this.State.Variables.UpTime          = info.ServerUptime;
                this.State.Variables.RoundTime       = info.RoundTime;
                this.State.Variables.DataSet(C_MOD, info.GameMod.ToString());
                this.State.Variables.DataSet(C_MAP_PACK, info.Mappack);

                if (info.GameMod == GameMods.None) {
                    this.ExecuteGameConfig(this.GameType.ToString().ToLower());
                }
                else {
                    this.ExecuteGameConfig(String.Format("{0}_{1}", this.GameType, info.GameMod).ToLower());
                }
                
                this.ThrowGameEvent(GameEventType.ServerInfoUpdated);
            }
        }

        [DispatchPacket(MatchText = "login.plainText")]
        public void LoginPlainTextDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {
                if (request.Words.Count >= 2 && response.Words.Count == 1 && response.Words[0] == "OK") {
                    this.Client.ConnectionState = ConnectionState.LoggedIn;

                    this.SendEventsEnabledPacket();

                    this.nextLargeSync = DateTime.Now;
                    this.Synchronize();
                }
            }
        }
        
        [DispatchPacket(MatchText = "login.hashed")]
        public void LoginHashedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (response != null) {
                if (request.Words.Count == 1 && response.Words.Count >= 2) {
                    this.SendRequest("login.hashed", this.GeneratePasswordHash(this.HashToByteArray(response.Words[1]), this.Password));
                }
                else if (request.Words.Count >= 2 && response.Words.Count == 1) {
                    this.Client.ConnectionState = ConnectionState.LoggedIn;

                    this.SendEventsEnabledPacket();

                    this.nextLargeSync = DateTime.Now;
                    this.Synchronize();
                }
            }
        }

        protected void AdminListPlayersFinalize(FrostbitePlayerList players) {
            if (players.Subset.Context == PlayerSubsetContext.All) {

                // 1. Remove all names in the state list that are not found in the new list (players that have left)
                this.State.PlayerList.RemoveAll(x => players.Select(y => y.Name).Contains(x.Name) == false);

                // 2. Add or update any new players
                foreach (FrostbitePlayer player in players) {
                    FrostbitePlayer statePlayer = null;

                    if ((statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name) as FrostbitePlayer) == null) {
                        this.State.PlayerList.Add(player);
                    }
                    else {
                        // Already exists, update with any new information we have.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Score = player.Score;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        statePlayer.Squad = player.Squad;
                        statePlayer.Team = player.Team;
                        statePlayer.GUID = player.GUID;
                    }
                }

                this.ThrowGameEvent(GameEventType.PlayerlistUpdated);
            }
        }

        [DispatchPacket(MatchText = "admin.listPlayers")]
        public virtual void AdminListPlayersResponseDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            FrostbitePlayerList players = new FrostbitePlayerList() {
                Subset = new FrostbitePlayerSubset().Parse(request.Words.GetRange(1, request.Words.Count - 1))
            }.Parse(response.Words.GetRange(1, response.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        [DispatchPacket(MatchText = "admin.say")]
        public void AdminSayDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 3) {
                this.ThrowGameEvent(GameEventType.Chat, new FrostbiteChat().ParseAdminSay(request.Words.GetRange(1, request.Words.Count - 1)));
            }

        }

        [DispatchPacket(MatchText = "version")]
        public void VersionDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 3) {
                this.State.Variables.Version = request.Words[2];
            }
        }

        [DispatchPacket(MatchText = "mapList.list")]
        public virtual void MapListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {

                FrostbiteMapList maps = new FrostbiteMapList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

                Map mapInfo = null;
                foreach (Map map in maps) {
                    if ((mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, true) == 0)) != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode     = mapInfo.GameMode;
                    }
                }
                this.State.MapList = maps;

                this.ThrowGameEvent(
                    GameEventType.MaplistUpdated
                );
            }
        }

        [DispatchPacket(MatchText = "banList.list")]
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

                    this.Send(this.Create("banList.list {0}", startOffset + 100));
                }
                else {
                    // We have recieved the whole banlist in 100 ban increments.. throw event.
                    this.ThrowGameEvent(
                        GameEventType.BanlistUpdated
                    );
                }
            }
        }

        [DispatchPacket(MatchText = "banList.add")]
        public void BanListAddDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {
                FrostbiteBan ban = new FrostbiteBan().ParseBanAdd(request.Words.GetRange(1, request.Words.Count - 1));

                this.State.BanList.Add(ban);

                this.ThrowGameEvent(
                    GameEventType.PlayerBanned,
                    ban
                );
            }
        }

        [DispatchPacket(MatchText = "banList.remove")]
        public void BanListRemoveDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {
                FrostbiteBan ban = new FrostbiteBan().ParseBanRemove(request.Words.GetRange(1, request.Words.Count - 1));

                // -- Added by Imisnew2
                // Quit being so fail phogue.  Null == Null is true.  Srsly.  Gotta check for null before you check the match!
                Ban stateBan = this.State.BanList.Find(x => (x.Target.Name != null && x.Target.Name == ban.Target.Name)
                                                         || (x.Target.GUID != null && x.Target.GUID == ban.Target.GUID));
                this.State.BanList.Remove(stateBan);

                this.ThrowGameEvent(
                    GameEventType.PlayerUnbanned,
                    stateBan
                );
            }
        }

        #region Variables

        [DispatchPacket(MatchText = "vars.serverName")]
        public void VarsServerNameDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.ServerName = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.gamePassword")]
        public void VarsGamePasswordDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.Password = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.punkBuster")]
        public void VarsGamePunkbusterDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Variables.AntiCheat = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.hardCore")]
        public void VarsHardcoreDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_HARDCORE, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.ranked")]
        public void VarsRankedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Variables.Ranked = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.rankLimit")]
        public void VarsRankLimitDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_RANK_LIMIT, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.teamBalance")]
        public void VarsTeamBalanceDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Variables.AutoBalance = boolOut;
            }
        }

        [DispatchPacket(MatchText = "vars.friendlyFire")]
        public void VarsFriendlyFireDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            bool boolOut = false;
            if (response.Words.Count >= 2 && bool.TryParse(response.Words[1], out boolOut)) {
                this.State.Variables.FriendlyFire = boolOut;
            }
        }
        
        [DispatchPacket(MatchText = "vars.bannerUrl")]
        public void VarsBannerUrlDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.BannerUrl = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.serverDescription")]
        public void VarsServerDescriptionDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.ServerDescription = response.Words[1];
            }
        }

        [DispatchPacket(MatchText = "vars.killCam")]
        public void VarsKillCamDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_KILL_CAM, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.miniMap")]
        public void VarsMiniMapDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_MINI_MAP, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.crossHair")]
        public void VarsCrossHairDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_CROSS_HAIR, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.idleTimeout")]
        public void VarsIdleTimeoutDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_IDLE_TIMEOUT, response.Words[1]);
            }
        }

        [DispatchPacket(MatchText = "vars.profanityFilter")]
        public void VarsProfanityFilterDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (response.Words.Count >= 2) {
                this.State.Variables.DataSet(C_PROFANITY_FILTER, response.Words[1]);
            }
        }

        #endregion

        [DispatchPacket(MatchText = "punkBuster.onMessage")]
        public void PunkBusterOnMessageDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 1) {

                PunkBusterObject pbObject = PB.Parse(request.Words[1]);

                if (pbObject is PunkBusterPlayer) {
                    PunkBusterPlayer player = pbObject as PunkBusterPlayer;

                    FrostbitePlayer statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name) as FrostbitePlayer;

                    if (statePlayer != null) {
                        statePlayer.SlotID = player.SlotID;
                        statePlayer.IP = player.IP;
                    }
                }
                else if (pbObject is PunkBusterBeginPlayerList) {
                    
                }
                else if (pbObject is PunkBusterEndPlayerList) {
                    this.ThrowGameEvent(GameEventType.PlayerlistUpdated);
                }
            }
        }

        [DispatchPacket(MatchText = "player.onKill")]
        public void PlayerOnKillDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 11) {

                bool headshot = false;

                if (bool.TryParse(request.Words[4], out headshot) == true) {

                    this.ThrowGameEvent(
                        GameEventType.PlayerKill,
                        new Kill() {
                            HitLocation = headshot == true ? FrostbiteGame.HEADSHOT : FrostbiteGame.BODYSHOT,
                            Killer = this.State.PlayerList.Find(x => x.Name == request.Words[1]),
                            Target = this.State.PlayerList.Find(x => x.Name == request.Words[2]),
                            DamageType = new Item() {
                                Name = request.Words[3]
                            },
                            KillerLocation = new Point3D(request.Words[5], request.Words[7], request.Words[6]),
                            TargetLocation = new Point3D(request.Words[8], request.Words[10], request.Words[9])
                        }
                    );
                }
            }
        }

        [DispatchPacket(MatchText = "server.onLoadingLevel")]
        public void ServerOnLoadingLevelDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 4) {

                int currentRound = 0, totalRounds = 0;

                if (int.TryParse(request.Words[2], out currentRound) == true && int.TryParse(request.Words[3], out totalRounds) == true) {

                    this.State.Variables.RoundIndex = currentRound;
                    this.State.Variables.MaxRoundIndex = totalRounds;

                    // Maps are the same, only a round change
                    if (String.Compare(this.State.Variables.MapName, request.Words[1], true) == 0)
                        this.ThrowGameEvent(GameEventType.RoundChanged);
                    else {
                        Map selectedMap = this.State.MapPool.Find(x => String.Compare(x.Name, request.Words[1], true) == 0);

                        if (selectedMap != null)
                            this.State.Variables.GameModeName = selectedMap.GameMode.Name;
                        this.State.Variables.MapName = request.Words[1];
                        this.ThrowGameEvent(GameEventType.MapChanged);
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Move the PlayerJoin event to the onAuthenticated?
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [DispatchPacket(MatchText = "player.onJoin")]
        public void PlayerOnJoinDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 2) {

                FrostbitePlayer player = new FrostbitePlayer() {
                    Name = request.Words[1]
                };

                if (this.State.PlayerList.Find(x => x.Name == player.Name) == null) {
                    this.State.PlayerList.Add(player);
                }
            }
        }

        [DispatchPacket(MatchText = "player.onLeave")]
        public void PlayerOnLeaveDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 2) {
                //request.Words.RemoveAt(1);

                FrostbitePlayer player = (FrostbitePlayer)(new FrostbitePlayerList().Parse(request.Words.GetRange(2, request.Words.Count - 2)).FirstOrDefault());

                if (player != null) {
                    FrostbitePlayer statePlayer = null;

                    if ((statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name) as FrostbitePlayer) != null) {
                        // Already exists, update with any new information we have.
                        // Note: We must keep the same Player object which is why we update and swap
                        // instead of just assigning.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        statePlayer.Squad = player.Squad;
                        statePlayer.Team = player.Team;
                        statePlayer.GUID = player.GUID;

                        player = statePlayer;
                    }

                    this.State.PlayerList.RemoveAll(x => x.Name == player.Name);

                    this.ThrowGameEvent(
                        GameEventType.PlayerLeave,
                        player
                    );
                }
            }
        }

        [DispatchPacket(MatchText = "player.onChat")]
        public void PlayerOnChatDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            // player.onChat <source soldier name: string> <text: string> <target group: player subset>
            if (request.Words.Count >= 2) {
                FrostbiteChat chat = new FrostbiteChat().ParsePlayerChat(request.Words.GetRange(1, request.Words.Count - 1));

                if (chat.Subset.Context == PlayerSubsetContext.Player && chat.Subset.Player != null) {
                    chat.Subset.Player = this.State.PlayerList.Find(x => x.Name == chat.Subset.Player.Name);
                }

                if (this.State.PlayerList.Find(x => x.Name == chat.Author.Name) != null) {
                    chat.Author = this.State.PlayerList.Find(x => x.Name == chat.Author.Name);

                    this.ThrowGameEvent(
                        GameEventType.Chat,
                        chat
                    );
                }
                else {
                    // Couldn't find the player, must be from the server.
                    chat.Origin = ChatOrigin.Server;

                    this.ThrowGameEvent(
                        GameEventType.Chat,
                        chat
                    );
                }
            }
        }

        [DispatchPacket(MatchText = "player.onAuthenticated")]
        public void PlayerOnAuthenticatedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            if (request.Words.Count >= 3) {
                FrostbitePlayer statePlayer = this.State.PlayerList.Find(x => x.Name == request.Words[1]) as FrostbitePlayer;

                if (statePlayer != null) {
                    statePlayer.GUID = request.Words[2];
                }
                else {
                    statePlayer = new FrostbitePlayer() {
                        Name = request.Words[1],
                        GUID = request.Words[2]
                    };

                    this.State.PlayerList.Add(statePlayer);
                }

                this.ThrowGameEvent(
                    GameEventType.PlayerJoin,
                    statePlayer
                );
            }
        }

        [DispatchPacket(MatchText = "player.onSpawn")]
        public void PlayerOnSpawnDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            FrostbiteSpawn spawn = new FrostbiteSpawn().Parse(request.Words.GetRange(1, request.Words.Count - 1));

            FrostbitePlayer player = this.State.PlayerList.Find(x => x.Name == spawn.Player.Name) as FrostbitePlayer;

            if (player != null) {
                player.Role = spawn.Role;
                player.Inventory = spawn.Inventory;

                this.ThrowGameEvent(
                    GameEventType.PlayerSpawn,
                    spawn
                );
            }
        }

        [DispatchPacket(MatchText = "player.onKicked")]
        public void PlayerOnKickedDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            FrostbitePlayer player = this.State.PlayerList.Find(x => x.Name == request.Words[1]) as FrostbitePlayer;

            if (player != null) {
                // Note that this is removed when the player.OnLeave event is fired.
                //this.State.PlayerList.RemoveAll(x => x.Name == request.Words[1]);

                this.ThrowGameEvent(
                    GameEventType.PlayerKicked,
                    new Kick() {
                        Target = player,
                        Reason = request.Words[2]
                    }
                );
            }
        }

        [DispatchPacket(MatchText = "player.onSquadChange")]
        public void PlayerOnSquadChangeDispatchHandler(FrostbitePacket request, FrostbitePacket response) {

            FrostbitePlayer player = this.State.PlayerList.Find(x => x.Name == request.Words[1]) as FrostbitePlayer;
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Words[2], out teamId) == true && int.TryParse(request.Words[3], out squadId) == true) {

                player.Squad = FrostbiteConverter.SquadIdToSquad(squadId);

                this.ThrowGameEvent(
                    GameEventType.PlayerMoved,
                    player
                );
            }
        }

        [DispatchPacket(MatchText = "player.onTeamChange")]
        public void PlayerOnTeamChangeDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            FrostbitePlayer player = this.State.PlayerList.Find(x => x.Name == request.Words[1]) as FrostbitePlayer;
            int teamId = 0, squadId = 0;

            if (player != null && int.TryParse(request.Words[2], out teamId) == true && int.TryParse(request.Words[3], out squadId) == true) {

                player.Team = FrostbiteConverter.TeamIdToTeam(teamId);
                player.Squad = FrostbiteConverter.SquadIdToSquad(squadId);
                
                this.ThrowGameEvent(
                    GameEventType.PlayerMoved,
                    player
                );
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
                    if (packet.Words.Count >= 1 && String.Compare(packet.Words[0], FrostbitePacket.STRING_RESPONSE_OKAY) == 0) {
                        this.Dispatch(requestPacket.Words[0], requestPacket, packet);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(packet.Words[0], requestPacket, packet);
                    }
                }

            }
            else if (packet.Words.Count >= 1 && packet.Origin == PacketOrigin.Server && packet.IsResponse == false) {
                this.Dispatch(packet.Words[0], packet, null);
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
            this.Send(this.Create("eventsEnabled true"));
        }

        #endregion

        protected override FrostbitePacket Create(string format, params object[] args) {
            return new FrostbitePacket(PacketOrigin.Client, false, null, String.Format(format, args).Wordify());
        }

        protected override void Action(Chat chat) {
            if (chat.Subset != null) {
                string subset = String.Empty;

                if (chat.Subset.Context == PlayerSubsetContext.All) {
                    subset = "all";
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Player && chat.Subset.Player != null) {
                    subset = String.Format("player {0}", chat.Subset.Player.Name);
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Team) {
                    subset = String.Format("team {0}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team));
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Squad) {
                    subset = String.Format("squad {0} {1}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team), FrostbiteConverter.SquadToSquadId(chat.Subset.Squad));
                }

                if (chat.ChatActionType == ChatActionType.Say) {
                    this.Send(this.Create("admin.say \"{0}\" {1}", chat.Text, subset));
                }
                else if (chat.ChatActionType == ChatActionType.Yell || chat.ChatActionType == ChatActionType.YellOnly) {
                    this.Send(this.Create("admin.yell \"{0}\" 8000 {1}", chat.Text, subset));
                }
            }
        }

        // Added by Imisnew2 - You should check this phogue!
        protected override void Action(Move move)
        {
            if (move.Target != null) {
                if (move.MoveActionType == MoveActionType.ForceMove || move.MoveActionType == MoveActionType.ForceRotate) {
                    this.Send(this.Create("admin.movePlayer \"{0}\" {1} {2} true",
                        move.Target.Name,
                        FrostbiteConverter.TeamToTeamId(move.Destination.Team), 
                        FrostbiteConverter.SquadToSquadId(move.Destination.Squad)));
                }
                else if  (move.MoveActionType == MoveActionType.Move || move.MoveActionType == MoveActionType.Rotate) {
                    this.Send(this.Create("admin.movePlayer \"{0}\" {1} {2} false",
                        move.Target.Name,
                        FrostbiteConverter.TeamToTeamId(move.Destination.Team),
                        FrostbiteConverter.SquadToSquadId(move.Destination.Squad)));
                }
            }
        }

        protected override void Action(Kill kill) {
            if (kill.Target != null) {
                this.Send(this.Create("admin.killPlayer \"{0}\"", kill.Target.Name));

                if (kill.Reason != null && kill.Reason.Length > 0) {
                    this.Send(this.Create("admin.say \"{0}\" player {1}", kill.Reason, kill.Target.Name));
                }
            }
        }

        protected override void Action(Kick kick) {
            if (kick.Target != null) {
                if (kick.Reason != null && kick.Reason.Length > 0) {
                    this.Send(this.Create("admin.kickPlayer \"{0}\" \"{1}\"", kick.Target.Name, kick.Reason));
                }
                else {
                    this.Send(this.Create("admin.kickPlayer \"{0}\"", kick.Target.Name));
                }
            }
        }

        protected override void Action(Ban ban) {
            if (ban.BanActionType == BanActionType.Ban) {
                if (ban.Time.Context == TimeSubsetContext.Permanent) {
                    if (ban.Reason.Length == 0) {
                        this.Send(this.Create("banList.add guid \"{0}\" perm", ban.Target.GUID));
                    }
                    else {
                        this.Send(this.Create("banList.add guid \"{0}\" perm \"{1}\"", ban.Target.GUID, ban.Reason));
                    }
                }
                else if (ban.Time.Context == TimeSubsetContext.Time && ban.Time.Length.HasValue == true) {
                    if (ban.Reason.Length == 0) {
                        this.Send(this.Create("banList.add guid \"{0}\" seconds {1}", ban.Target.GUID, ban.Time.Length.Value.TotalSeconds));
                    }
                    else {
                        this.Send(this.Create("banList.add guid \"{0}\" seconds {1} \"{2}\"", ban.Target.GUID, ban.Time.Length.Value.TotalSeconds, ban.Reason));
                    }
                }
            }
            else if (ban.BanActionType == BanActionType.Unban) {
                this.Send(this.Create("banList.remove guid \"{0}\"", ban.Target.GUID));
            }

            this.Send(this.Create("banList.save"));
        }

        protected override void Action(Map map) {

            if (map.MapActionType == MapActionType.Append) {
                this.Send(this.Create("mapList.append \"{0}\" {1}", map.Name, map.Rounds));

                this.Send(this.Create("mapList.save"));

                this.Send(this.Create("mapList.list rounds"));
            }
            // Added by Imisnew2 - You should check this phogue!
            else if (map.MapActionType == MapActionType.ChangeMode) {
                if (map.GameMode != null) {
                    this.Send(this.Create("admin.setPlaylist \"{0}\"", map.GameMode.Name));
                }
            }
            else if (map.MapActionType == MapActionType.Insert) {
                this.Send(this.Create("mapList.insert {0} \"{1}\" {2}", map.Index, map.Name, map.Rounds));

                this.Send(this.Create("mapList.save"));

                this.Send(this.Create("mapList.list rounds"));
            }
            else if (map.MapActionType == MapActionType.Remove) {
                var matchingMaps = this.State.MapList.Where(x => x.Name == map.Name).OrderByDescending(x => x.Index);

                foreach (Map match in matchingMaps) {
                    this.Send(this.Create("mapList.remove {0}", match.Index));
                }

                this.Send(this.Create("mapList.save"));

                this.Send(this.Create("mapList.list rounds"));
            }
            else if (map.MapActionType == MapActionType.RemoveIndex) {
                this.Send(this.Create("mapList.remove {0}", map.Index));

                this.Send(this.Create("mapList.list rounds"));
            }
            else if (map.MapActionType == MapActionType.NextMapIndex) {
                this.Send(this.Create("mapList.nextLevelIndex {0}", map.Index));
            }
            else if (map.MapActionType == MapActionType.RestartMap || map.MapActionType == MapActionType.RestartRound) {
                this.Send(this.Create("admin.restartRound"));
            }
            else if (map.MapActionType == MapActionType.NextMap || map.MapActionType == MapActionType.NextRound) {
                this.Send(this.Create("admin.runNextRound"));
            }
            else if (map.MapActionType == MapActionType.Clear) {
                this.Send(this.Create("mapList.clear"));

                this.Send(this.Create("mapList.save"));
            }
        }

        public override void Login(string password) {
            this.Send(this.Create("login.hashed"));
        }
        
        #endregion

    }
}
