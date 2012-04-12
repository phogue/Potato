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
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.CallOfDuty {
    using Procon.Net.Attributes;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.CallOfDuty.Objects;

    public abstract class CallOfDutyGame : GameImplementation<CallOfDutyClient, CallOfDutyPacket> {

        private static readonly Dictionary<Regex, string> PacketTypes = new Dictionary<Regex, string>() {
            {
                new Regex(@"^map: (?<map>.*?)num[ ]+score[ ]+ping[ ]+guid[ ]+name[ ]+team[ ]+lastmsg[ ]+address[ ]+qport[ ]+rate", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline), "teamstatus"
            },
            {
                new Regex(@"^Server info settings:", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline), "serverinfo"
            }
        };

        protected string LogUri {
            set {
                if (this.LogFile != null) {
                    this.LogFile.LogAddress = value;
                }
            }
        }

        protected CallOfDutyLogfile LogFile { get; set; }

        #region Events
        /*
        //public new delegate void PlayerlistCommandHandler(CallOfDutyGame sender, CallOfDutyPlayerList players);
        public override event PlayerlistCommandHandler PlayerlistCommand;

        /*
        public new delegate void PunkBusterPlayerlistEventHandler(Frostbite15Game sender, PunkBusterPlayer player);
        public new event PunkBusterPlayerlistEventHandler PunkBusterPlayerlistEvent;

        public new delegate void PunkBusterBeginPlayerlistEventHandler(Frostbite15Game sender, PunkBusterBeginPlayerList beginList);
        public new event PunkBusterBeginPlayerlistEventHandler PunkBusterBeginPlayerlistEvent;

        public new delegate void PunkBusterEndPlayerlistEventHandler(Frostbite15Game sender, PunkBusterEndPlayerList endList);
        public new event PunkBusterEndPlayerlistEventHandler PunkBusterEndPlayerlistEvent;

        public new delegate void PlayerOnKillEventHandler(Frostbite15Game sender, Kill kill);
        public new event PlayerOnKillEventHandler PlayerOnKillEvent;

        public new delegate void PlayerOnJoinEventHandler(Frostbite15Game sender, Frostbite15Player player);
        public new event PlayerOnJoinEventHandler PlayerOnJoinEvent;

        public new delegate void PlayerOnLeaveEventHandler(Frostbite15Game sender, Frostbite15Player player);
        public new event PlayerOnLeaveEventHandler PlayerOnLeaveEvent;

        public new delegate void PlayerOnChatEventHandler(Frostbite15Game sender, Frostbite15Chat chat);
        public new event PlayerOnChatEventHandler PlayerOnChatEvent;

        public new delegate void PlayerOnAuthenticatedEventHandler(Frostbite15Game sender, Frostbite15Player player);
        public new event PlayerOnAuthenticatedEventHandler PlayerOnAuthenticatedEvent;

        public new delegate void ServerInfoEventHandler(Frostbite15Game sender, Frostbite15ServerInfo info);
        public new event ServerInfoEventHandler ServerInfoEvent;

        public new delegate void ServerOnLoadingLevelEventHandler(Frostbite15Game sender, string levelFileName);
        public new event ServerOnLoadingLevelEventHandler ServerOnLoadingLevelEvent;
        */

        #endregion

        public CallOfDutyGame(string hostName, ushort port) : base(hostName, port) {

            this.State.Variables.MaxConsoleLines = 100;
        }

        protected override Client<CallOfDutyPacket> CreateClient(string hostName, ushort port) {
            return new CallOfDutyClient(hostName, port);
        }

        public override void Synchronize() {
            this.Send(this.Create("teamstatus"));
            this.Send(this.Create("serverinfo"));

            if (this.LogFile != null) {
                this.LogFile.Fetch();
            }
        }

        protected override void AssignEvents() {
            base.AssignEvents();

            this.LogFile = new CallOfDutyLogfile() {
                Interval = 3
            };
            this.LogFile.ChatEntry += new CallOfDutyLogfile.ChatEntryHandler(LogFile_ChatEntry);
            this.LogFile.KillEntry += new CallOfDutyLogfile.KillEntryHandler(LogFile_KillEntry);
        }

        private void LogFile_KillEntry(CallOfDutyLogfile sender, DateTime eventTime, CallOfDutyKill kill) {
            if (kill.Killer != null && kill.Target != null) {
                kill.Killer = this.State.PlayerList.Find(x => x.UID == kill.Killer.UID);

                kill.Target = this.State.PlayerList.Find(x => x.UID == kill.Target.UID);

                this.ThrowGameEvent(GameEventType.PlayerKill, kill);
            }
        }

        private void LogFile_ChatEntry(CallOfDutyLogfile sender, DateTime eventTime, CallOfDutyChat chat) {

            if (chat.Author != null) {
                chat.Author = this.State.PlayerList.Find(x => x.UID == chat.Author.UID);

                this.ThrowGameEvent(GameEventType.Chat, chat);
            }
        }

        #region Dispatching

        protected override void Dispatch(CallOfDutyPacket packet) {

            Match match = null;
            foreach (KeyValuePair<Regex, string> packetType in CallOfDutyGame.PacketTypes) {
                if ((match = packetType.Key.Match(packet.Message)).Success == true) {
                    this.Dispatch(packetType.Value, null, packet);
                }
            }
        }

        [DispatchPacket(MatchText = "serverinfo")]
        public void ServerInfoDispatchHandler(CallOfDutyPacket request, CallOfDutyPacket response) {

            CallOfDutyServerInfo info = new CallOfDutyServerInfo().Parse(response.Message);

            this.State.Variables.MapName        = info.mapname;
            this.State.Variables.MaxPlayerCount = info.sv_maxclients;
            this.State.Variables.Ranked         = info.sv_ranked > 0;
            this.State.Variables.ServerName     = info.sv_hostname;
            this.State.Variables.FriendlyFire   = info.scr_team_fftype > 0;
            this.State.Variables.GameModeName   = info.g_gametype;

            this.ThrowGameEvent(GameEventType.ServerInfoUpdated);
        }

        [DispatchPacket(MatchText = "teamstatus")]
        public void PlayerlistDispatchHandler(CallOfDutyPacket request, CallOfDutyPacket response) {

            CallOfDutyPlayerList players = new CallOfDutyPlayerList().Parse(response.Message);

            if (players.Subset.Context == PlayerSubsetContext.All) {

                // 1. Remove all names in the state list that are not found in the new list (players that have left)
                this.State.PlayerList.RemoveAll(x => players.Select(y => y.Name).Contains(x.Name) == false);

                // 2. Add or update any new players
                foreach (CallOfDutyPlayer player in players) {
                    CallOfDutyPlayer statePlayer = null;

                    if ((statePlayer = this.State.PlayerList.Find(x => x.Name == player.Name) as CallOfDutyPlayer) == null) {
                        this.State.PlayerList.Add(player);
                    }
                    else {
                        // Already exists, update with any new information we have.
                        statePlayer.Kills = player.Kills;
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        statePlayer.Squad = player.Squad;
                        statePlayer.Team = player.Team;
                        statePlayer.GUID = player.GUID;
                    }
                }

                this.State.Variables.PlayerCount = players.Count;

                this.ThrowGameEvent(GameEventType.PlayerlistUpdated);
            }
        }

        #endregion

        #region Packet Helpers

        protected override CallOfDutyPacket Create(string format, params object[] args) {
            return new CallOfDutyPacket(PacketOrigin.Client, false, this.Password, String.Format(format, args));
        }

        public override void Login(string password) {
            this.Client.ConnectionState = Net.ConnectionState.LoggedIn;
            this.Send(this.Create("g_logsync 1"));
            this.Send(this.Create("g_logTimeStampInSeconds 1"));

            this.Synchronize();
        }

        protected override void Action(Kick kick) {
            if (kick.Target != null) {
                if (kick.Reason != null && kick.Reason.Length > 0) {
                    this.Send(this.Create("clientkick {0} \"{1}\"", kick.Target.SlotID, kick.Reason));
                }
                else {
                    this.Send(this.Create("clientkick {0}", kick.Target.SlotID));
                }
            }
        }

        protected override void Action(Chat chat) {
            if (chat.Subset.Context == PlayerSubsetContext.All) {
                this.Send(this.Create("say \"{0}\"", chat.Text));
            }
            else if (chat.Subset.Context == PlayerSubsetContext.Player && chat.Subset.Player != null) {
                this.Send(this.Create("tell {0} \"{1}\"", chat.Subset.Player.SlotID, chat.Text));
            }
        }

        /*
        public override void Kick(Kick player) {
            if (player.Reason != null && player.Reason.Length > 0) {
                this.Send(this.Create("clientkick {0} \"{1}\"", ((CallOfDutyPlayer)player.Player).SlotID, player.Reason));
            }
            else {
                this.Send(this.Create("clientkick {0}", ((CallOfDutyPlayer)player.Player).SlotID));
            }
        }
        
        public override void Say(string format, params object[] args) {
            this.Send(this.Create("say \"" + String.Format(format, args) + "\""));
        }

        public override void PlayerStatus() {
            this.Send(this.Create("teamstatus"));
        }
        */
        #endregion
    }
}
