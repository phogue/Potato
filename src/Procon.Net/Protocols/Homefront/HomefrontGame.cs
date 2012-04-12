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
using System.Security.Cryptography;
using System.Text;
using System.Timers;

namespace Procon.Net.Protocols.Homefront {
    using Procon.Net.Attributes;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.Homefront.Objects;

    [Game(GameType = GameType.Homefront)]
    public class HomefrontGame : GameImplementation<HomefrontClient, HomefrontPacket> {

        public HomefrontGame(string hostName, ushort port) : base(hostName, port) {

            this.State.Variables.MaxConsoleLines = 100;

            // Homefront demands a PING to be sent every 10 seconds.  We send one every nine to remain connected
            Timer ping = new Timer(9000);
            ping.Elapsed += new ElapsedEventHandler(ping_Elapsed);
            ping.Start();
        }

        private void ping_Elapsed(object sender, ElapsedEventArgs e) {
            this.Send(new HomefrontPacket(PacketOrigin.Client, false, MessageType.ClientPing, "PING"));
        }

        protected override Client<HomefrontPacket> CreateClient(string hostName, ushort port) {
            return new HomefrontClient(hostName, port);
        }

        protected override void AssignEvents() {
            base.AssignEvents();
        }

        private DateTime nextLargeSync = DateTime.Now;
        public override void Synchronize() {
            this.Send(this.Create("RETRIEVE PLAYERLIST"));

            if (DateTime.Now > nextLargeSync) {
                this.Send(this.Create("RETRIEVE BANLIST"));

                nextLargeSync = DateTime.Now.AddSeconds(300);
            }
        }

        #region RECV + Dispatch

        protected override void Dispatch(HomefrontPacket packet) {

            if (packet.MessageWords.Count >= 1) {

                bool containsColon = (packet.MessageWords.Where(x => x.Contains(':')).Select(x => x).Count() > 0);

                // If a colon is available some where along the packet words (HELLO 55555 - does not)
                if (containsColon == true) {
                    // Compress the first word until it contains a ':' or we have no more words.
                    while (packet.MessageWords.Count >= 2 && packet.MessageWords[0].Contains(':') == false) {
                        packet.MessageWords[0] = String.Join(" ", packet.MessageWords.GetRange(0, 2).ToArray());
                        packet.MessageWords.RemoveAt(1);
                    }
                }

                List<string> splitCommand = packet.MessageWords[0]
                                                  .Split(':')
                                                  .Where(x => String.IsNullOrEmpty(x) == false)
                                                  .ToList();
                packet.MessageWords.RemoveAt(0);
                packet.MessageWords.InsertRange(0, splitCommand);

                if (packet.MessageWords.Count >= 1) {
                    this.Dispatch(packet.MessageWords[0], packet, null);
                }
            }



            /* Until sId's are functional.
            if (packet.Origin == PacketOrigin.Client && packet.IsResponse == true) {

            }
            else if (packet.Origin == PacketOrigin.Server && packet.IsResponse == false) {

            }
            */
        }
        
        #endregion

        #region Dispatch Handlers

        [DispatchPacket(MatchText = "HELLO")]
        public void VersionDispatchHandler(HomefrontPacket request, HomefrontPacket response) {
            this.State.Variables.Version = request.MessageWords[1];
        }

        [DispatchPacket(MatchText = "LOADED")]
        public void LoadedDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            //this.Send(((HomefrontPacketFactory)this.PacketFactory).Login(String.Join(" ", new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(this.Password)).Select(x => x.ToString("X2")).ToArray())));

        }

        [DispatchPacket(MatchText = "BAN ITEM")]
        public void BanItemDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 3 && request.MessageWords[1].Length > 0) {

                if (this.State.BanList.Find(x => x.Target.Name == request.MessageWords[1]) == null) {
                    Ban ban = new Ban() {
                        Target = new Player() {
                            Name = request.MessageWords[1],
                            GUID = request.MessageWords[2]
                        }
                    };

                    this.State.BanList.Add(ban);

                    this.ThrowGameEvent(GameEventType.BanlistUpdated, ban);
                }
            }
        }

        [DispatchPacket(MatchText = "BAN ADDED")]
        public void BanAddedDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 2 && request.MessageWords[1].Length > 0) {

                if (this.State.BanList.Find(x => x.Target.Name == request.MessageWords[1]) == null) {
                    Ban ban = new Ban() {
                        Target = new Player() {
                            Name = request.MessageWords[1],
                            GUID = request.MessageWords[2]
                        }
                    };

                    this.State.BanList.Add(ban);

                    this.ThrowGameEvent(GameEventType.PlayerBanned, ban);
                }
            }
        }

        [DispatchPacket(MatchText = "BAN REMOVE")]
        public void BanRemovedDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 2 && request.MessageWords[1].Length > 0) {
                Ban ban = null;

                if ((ban = this.State.BanList.Find(x => x.Target.GUID == request.MessageWords[1])) != null) {
                    this.ThrowGameEvent(GameEventType.PlayerUnbanned, ban);
                }
            }
        }

        [DispatchPacket(MatchText = "BROADCAST")]
        public void BroadcastDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            HomefrontChat chat = new HomefrontChat().Parse(request.MessageWords);
            
            if (chat.Author != null) {
                Player player = this.State.PlayerList
                                       .Where(x => x != null && x.Name == chat.Author.Name)
                                       .FirstOrDefault();

                if (player != null) {
                    chat.Author = player;
                }

                this.ThrowGameEvent(GameEventType.Chat, chat);
            }
        }

        [DispatchPacket(MatchText = "LOGIN")]
        public void LoginDispatchHandler(HomefrontPacket request, HomefrontPacket response) {
            if (request.MessageWords.Count >= 2) {
                Player connected = new Player() {
                    Name = request.MessageWords[1]
                };

                if (this.State.PlayerList.Where(x => x.Name == connected.Name).FirstOrDefault() == null) {
                    this.State.PlayerList.Add(connected);

                    // TODO: Check if this is thrown before or after uid check
                    this.ThrowGameEvent(GameEventType.PlayerJoin, connected);
                }
            }
        }

        [DispatchPacket(MatchText = "LOGOUT")]
        public void LogoutDispatchHandler(HomefrontPacket request, HomefrontPacket response) {
            if (request.MessageWords.Count >= 2) {
                Player disconnected = this.State.PlayerList.Where(x => x != null && x.Name == request.MessageWords[1]).Select(x => x).FirstOrDefault();

                if (disconnected != null) {
                    this.State.PlayerList.Remove(disconnected);

                    this.ThrowGameEvent(GameEventType.PlayerLeave, disconnected);
                }
            }
        }

        // PLAYER: [int: Team] [string: Clan] [string: Name] [int: Kills] [int: Deaths]
        [DispatchPacket(MatchText = "PLAYER")]
        public void PlayerDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            HomefrontPlayer player = new HomefrontPlayer().Parse(request.MessageWords.GetRange(1, request.MessageWords.Count - 1));

            Player storedPlayer = this.State.PlayerList.Find(x => x.Name == player.Name);

            if (storedPlayer != null) {
                storedPlayer.GUID = player.GUID;
                storedPlayer.ClanTag = player.ClanTag;
                storedPlayer.Kills = player.Kills;
                storedPlayer.Deaths = player.Deaths;
                storedPlayer.Team = player.Team;
            }
            else if (player.Name != "GCDemoRecSpectator") {
                this.State.PlayerList.Add(player);
            }

            this.ThrowGameEvent(GameEventType.PlayerlistUpdated);
        }

        [DispatchPacket(MatchText = "KILL")]
        public void KillDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            HomefrontKill kill = new HomefrontKill().Parse(request.MessageWords);

            if (kill.Killer != null && kill.Target != null) {
                kill.Killer = this.State.PlayerList.Where(x => x.Name == kill.Killer.Name).FirstOrDefault();
                kill.Target = this.State.PlayerList.Where(x => x.Name == kill.Target.Name).FirstOrDefault();

                if (kill.Killer != null && kill.Target != null) {
                    this.ThrowGameEvent(GameEventType.PlayerKill, kill);
                }
            }
        }

        [DispatchPacket(MatchText = "TEAM CHANGE")]
        public void TeamChangeDispatchHandler(HomefrontPacket request, HomefrontPacket response) {
            if (request.MessageWords.Count >= 3) {
                Player changed = this.State.PlayerList.Where(x => x != null && x.Name == request.MessageWords[1]).FirstOrDefault();
                int destinationTeamId = 0;

                if (changed != null && int.TryParse(request.MessageWords[2], out destinationTeamId) == true) {

                    changed.Team = HomefrontConverter.TeamIdToTeam(destinationTeamId);

                    this.ThrowGameEvent(GameEventType.PlayerMoved, changed);
                }
            }
        }

        [DispatchPacket(MatchText = "CLAN CHANGE")]
        public void ClanChangeDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 3) {
                Player changed = this.State.PlayerList.Where(x => x != null && x.Name == request.MessageWords[1]).Select(x => x).FirstOrDefault();

                if (changed != null) {
                    changed.ClanTag = request.MessageWords[2];
                }
            }
        }

        [DispatchPacket(MatchText = "CHANGE LEVEL")]
        public void CHangeLevelDispatchHandler(HomefrontPacket request, HomefrontPacket response) {
            if (request.MessageWords.Count >= 2) {
                this.State.Variables.MapName = request.MessageWords[1];
            }
        }

        [DispatchPacket(MatchText = "UID")]
        public void UidDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 3) {
                Player uid = this.State.PlayerList.Where(x => x != null && x.Name == request.MessageWords[1]).FirstOrDefault();

                if (uid == null) {
                    Player player = new Player() {
                        Name = request.MessageWords[1],
                        GUID = request.MessageWords[2].Trim('<', '>')
                    };

                    this.State.PlayerList.Add(player);

                    this.ThrowGameEvent(GameEventType.PlayerJoin, player);
                }
                else {
                    uid.GUID = request.MessageWords[2].Trim('<', '>');
                }
            }
        }

        [DispatchPacket(MatchText = "AUTH")]
        public void AuthDispatchHandler(HomefrontPacket request, HomefrontPacket response) {

            if (request.MessageWords.Count >= 2) {

                bool result = false;

                if (bool.TryParse(request.MessageWords[1], out result) == true) {
                    if (result == true) {
                        this.Client.ConnectionState = Net.ConnectionState.LoggedIn;

                        this.Synchronize();
                    }
                    else {
                        this.Shutdown();
                    }
                }
                else {
                    this.Shutdown();
                }
            }
        }

        #endregion

        protected override HomefrontPacket Create(string format, params object[] args) {
            return new HomefrontPacket(PacketOrigin.Client, false, MessageType.ClientTransmission, String.Format(format, args));
        }

        public override void Login(string password) {
            this.Send(this.Create("PASS: \"{0}\"", String.Join(" ", new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(password)).Select(x => x.ToString("X2")).ToArray())));
        }

        protected override void Action(Kick kick) {
            if (kick.Target != null) {
                if (kick.Target.UID.Length > 0) {
                    this.Send(this.Create("admin kick \"{0}\"", kick.Target.UID));
                }
                else {
                    this.Send(this.Create("admin kick \"{0}\"", kick.Target.Name));
                }
            }
        }

        protected override void Action(Chat chat) {
            if (chat.Subset.Context == PlayerSubsetContext.All) {
                if (chat.ChatActionType == ChatActionType.Say) {
                    this.Send(this.Create("say {0}", chat.Text));
                }
                else if (chat.ChatActionType == ChatActionType.Yell || chat.ChatActionType == ChatActionType.YellOnly) {
                    this.Send(this.Create("admin bigsay {0}", chat.Text));
                }

                chat.Origin = ChatOrigin.Reflected;

                this.ThrowGameEvent(GameEventType.Chat, chat);
            }
        }

        protected override void Action(Kill kill) {
            if (kill.Target != null) {
                this.Send(this.Create("admin kill \"{0}\"", kill.Target.Name));
            }
        }

        protected override void Action(Map map) {
            if (map.MapActionType == MapActionType.NextMap || map.MapActionType == MapActionType.NextRound) {
                this.Send(this.Create("admin NextMap"));
            }
        }

        protected override void Action(Ban ban) {

            string uid = String.Empty;

            if (ban.Target != null) {
                if (ban.Target.UID.Length > 0) {
                    uid = ban.Target.UID;
                }
                else {
                    uid = ban.Target.Name;
                }
            }

            if (ban.BanActionType == BanActionType.Ban) {
                if (ban.Time.Context == TimeSubsetContext.Permanent) {
                    if (ban.Reason.Length > 0) {
                        this.Send(this.Create("admin kickban \"{0}\" \"\" \"{1}\"", uid, ban.Reason));
                    }
                    else {
                        this.Send(this.Create("admin kickban \"{0}\"", uid));
                    }
                }
            }
            else if (ban.BanActionType == BanActionType.Unban) {
                this.Send(this.Create("admin unban \"{0}\"", uid));
            }
        }

        protected override void Action(Move move) {
            if (move.Target != null) {
                if (move.MoveActionType == MoveActionType.Rotate || move.MoveActionType == MoveActionType.Rotate) {
                    this.Send(this.Create("admin forceteamswitch {0}", move.Target.Name));
                }
                else if (move.MoveActionType == MoveActionType.Move || move.MoveActionType == MoveActionType.ForceMove) {
                    if (move.Destination.Team == Team.NeutralOrSpectator) {
                        this.Send(this.Create("admin makespectate {0}", move.Target.Name));
                    }
                }
            }
        }

        /*
        public override void Kick(Kick player) {
            this.Send(this.Create("admin kick \"{0}\"", player.Player.Name));
        }

        public override void PermanentBan(Player player) {
            this.Send(this.Create("admin kickban \"{0}\"", player.Name));
        }

        public override void UnBan(Player player) {
            this.Send(this.Create("admin unban \"{0}\"", player.Name));
        }

        public override void Nextmap() {
            this.Send(this.Create("admin NextMap"));
        }

        public override void Say(string format, params object[] args) {
            this.Send(this.Create("say \"" + String.Format(format, args) + "\""));
        }
        */


    }
}
