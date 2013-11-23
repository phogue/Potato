using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.CallOfDuty {
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.CallOfDuty.Objects;

    public abstract class CallOfDutyGame : GameImplementation<CallOfDutyPacket> {

        private static readonly Dictionary<Regex, string> PacketTypes = new Dictionary<Regex, string>() {
            {
                new Regex(@"^map: (?<map>.*?)num[ ]+score[ ]+ping[ ]+Uid[ ]+name[ ]+team[ ]+lastmsg[ ]+address[ ]+qport[ ]+rate", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline), "teamstatus"
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

        protected CallOfDutyGame(string hostName, ushort port) : base(hostName, port) {

            this.State.Settings.MaxConsoleLines = 100;

            this.AppendDispatchHandlers(new Dictionary<PacketDispatch, PacketDispatchHandler>() {
                {
                    new PacketDispatch() { Name = "serverinfo"},
                    new PacketDispatchHandler(this.ServerInfoDispatchHandler)
                }, {
                    new PacketDispatch() { Name = "teamstatus"},
                    new PacketDispatchHandler(this.PlayerlistDispatchHandler)
                }
            });
        }

        protected override IClient CreateClient(string hostName, ushort port) {
            return new CallOfDutyClient(hostName, port);
        }

        public override void Synchronize() {
            this.Send(this.CreatePacket("teamstatus"));
            this.Send(this.CreatePacket("serverinfo"));

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

        private void LogFile_KillEntry(CallOfDutyLogfile sender, DateTime eventTime, Kill kill) {
            /*
            if (kill.Killer != null && kill.Target != null) {
                kill.Killer = this.State.PlayerList.Find(x => x.Uid == kill.Killer.Uid);

                kill.Target = this.State.PlayerList.Find(x => x.Uid == kill.Target.Uid);

                this.OnGameEvent(GameEventType.GamePlayerKill, new GameEventData() { Kills = new List<Kill>() { kill } });
            }
            */
        }

        private void LogFile_ChatEntry(CallOfDutyLogfile sender, DateTime eventTime, Chat chat) {

            if (chat.Now.Players != null) {
                // Fill it with the completed player object.
                chat.Now.Players = new List<Player>() {
                    this.State.PlayerList.Find(x => x.Uid == chat.Now.Players.First().Uid)
                };

                this.OnGameEvent(GameEventType.GameChat, new GameEventData() { Chats = new List<Chat>() { chat } });
            }
        }

        #region Dispatching

        protected override void Dispatch(CallOfDutyPacket packet) {

            Match match = null;
            foreach (KeyValuePair<Regex, string> packetType in CallOfDutyGame.PacketTypes) {
                if ((match = packetType.Key.Match(packet.Message)).Success == true) {
                    this.Dispatch(new PacketDispatch() {
                        Name = packetType.Value
                    }, null, packet);
                }
            }
        }

        public void ServerInfoDispatchHandler(CallOfDutyPacket request, CallOfDutyPacket response) {

            CallOfDutyServerInfo info = new CallOfDutyServerInfo().Parse(response.Message);

            this.State.Settings.MapName        = info.mapname;
            this.State.Settings.MaxPlayerCount = info.sv_maxclients;
            this.State.Settings.RankedEnabled         = info.sv_ranked > 0;
            this.State.Settings.ServerName     = info.sv_hostname;
            this.State.Settings.FriendlyFireEnabled   = info.scr_team_fftype > 0;
            this.State.Settings.GameModeName   = info.g_gametype;

            this.OnGameEvent(GameEventType.GameSettingsUpdated);
        }

        public void PlayerlistDispatchHandler(CallOfDutyPacket request, CallOfDutyPacket response) {

            CallOfDutyPlayerList players = new CallOfDutyPlayerList().Parse(response.Message);

            // if there are not limits on the context to fetch a player list..
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
                        statePlayer.Deaths = player.Deaths;
                        statePlayer.ClanTag = player.ClanTag;
                        statePlayer.Ping = player.Ping;
                        // statePlayer.Squad = player.Squad;
                        statePlayer.Uid = player.Uid;

                        statePlayer.ModifyGroup(player.Groups.FirstOrDefault(group => group.Type == Grouping.Team));
                    }
                }

                this.State.Settings.PlayerCount = players.Count;

                this.OnGameEvent(GameEventType.GamePlayerlistUpdated);
            }
        }

        #endregion

        #region Packet Helpers

        protected override CallOfDutyPacket CreatePacket(string format, params object[] args) {
            return new CallOfDutyPacket(PacketOrigin.Client, PacketType.Request, this.Password, String.Format(format, args));
        }

        public override void Login(string password) {
            this.Client.ConnectionState = Net.ConnectionState.ConnectionLoggedIn;
            this.Send(this.CreatePacket("g_logsync 1"));
            this.Send(this.CreatePacket("g_logTimeStampInSeconds 1"));

            this.Synchronize();
        }

        protected override void Action(Kick kick) {
            String reason = kick.Scope.Content != null ? kick.Scope.Content.FirstOrDefault() : String.Empty;

            foreach (Player player in kick.Scope.Players) {
                this.Send(String.IsNullOrEmpty(reason) == false ? this.CreatePacket("clientkick {0} \"{1}\"", player.SlotId, reason) : this.CreatePacket("clientkick {0}", player.SlotId));
            }
        }

        protected override void Action(Chat chat) {

            if (chat.Now.Content != null) {
                foreach (String chatMessage in chat.Now.Content) {
                    if (chat.Scope.Players == null || chat.Scope.Players.Count == 0) {
                        this.Send(this.CreatePacket("say \"{0}\"", chatMessage));
                    }
                    else if (chat.Scope.Players != null && chat.Scope.Players.Count > 0) {
                        foreach (Player chatTarget in chat.Scope.Players) {
                            this.Send(this.CreatePacket("tell {0} \"{1}\"", chatTarget.SlotId, chatMessage));
                        }
                    }
                }
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
