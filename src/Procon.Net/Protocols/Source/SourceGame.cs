using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Attributes;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.Source.Objects;

    public class SourceGame : GameImplementation<SourceClient, SourcePacket> {

        public ushort SourceLogServicePort { get; set; }
        public ushort? SourceLogListenPort { get; set; }

        public SourceGame(string hostName, ushort port)
            : base(hostName, port) {
                this.SourceLogServicePort = 8787;
        }

        protected override Client<SourcePacket> CreateClient(string hostName, ushort port) {
            return new SourceClient(hostName, port);
        }

        public override void Synchronize() {
            this.SendRequest("status");
        }

        #region Dispatching

        protected override void Dispatch(SourcePacket packet) {
            if (packet.Origin == PacketOrigin.Client && packet.IsResponse == true) {
                SourcePacket requestPacket = ((SourceClient)this.Client).GetRequestPacket(packet);

                if (packet.ResponseType == SourceResponseType.SERVERDATA_AUTH_RESPONSE) {
                    this.ServerAuthDispatchHandler(requestPacket, packet);
                }
                // If the request packet is valid and has at least one word.
                else if (requestPacket != null && requestPacket.String1Words.Count >= 1) {

                    // If the sent command was successful
                    if (packet.String1Words.Count >= 1 && packet.RequestType != SourceRequestType.SERVERDATA_ALLBAD) {
                        this.Dispatch(requestPacket.String1Words[0], requestPacket, packet);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(packet.String1Words[0], requestPacket, packet);
                    }
                }
            }
            else if (packet.Origin == PacketOrigin.Server && packet.IsResponse == false) {
                this.ParseLogEvent(packet);
                //this.Dispatch(packet.String1Words[0], packet, null);
            }
        }
        
        private static readonly Dictionary<Regex, Type> EntryTypes = new Dictionary<Regex, Type>() {
            // 050. Connection
            // "Name<uid><wonid><>" connected, address "ip:port"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" connected, address ""(?<ip>.*)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 052. Disconnection
            // "Name<uid><wonid><team>" disconnected
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" disconnected$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceDisconnection) },

            // 052b. Kick
            // Kick: "Name<uid><wonid><>" was kicked by "Console" (message "")
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" was kicked by ""Console"" (message ""(?<message>.*?)"")$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 053. Suicides
            // "Name<uid><wonid><team>" committed suicide with "weapon"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" committed suicide with ""(?<damage_type>.*?)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 054. Team Selection
            // "Name<uid><wonid><team>" joined team "team"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" joined team ""(?<new_team>.*?)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 055. Role Selection
            // "Name<uid><wonid><team>" changed role to "role"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" changed role to ""(?<new_role>.*?)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 056. Change Name
            // "Name<uid><wonid><team>" changed name to "Name"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]*?)>"" changed name to ""(?<new_name>.*?)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },

            // 057. Kills
            // "Name<uid><wonid><team>" killed "Name<uid><wonid><team>" with "weapon"
            { new Regex(@": ""(?<killer_name>.*?)<(?<killer_userid>[0-9]{1,3})><(?<killer_uniqueid>STEAM_[0-9:]+?)><(?<killer_team>[A-Za-z]*?)>"" killed ""(?<victim_name>.*?)<(?<victim_userid>[0-9]{1,3})><(?<victim_uniqueid>STEAM_[0-9:]+?)><(?<victim_team>[A-Za-z]*?)>"" with ""(?<damage_type>.*?)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },



            // 063. Chat
            // "Name<uid><wonid><team>" say "message"
            // "Name<uid><wonid><team>" say_team "message"
            { new Regex(@": ""(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]+?)>"" (?<context>say|say_team) ""(?<text>.*)""$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(SourceChat) },
            // { new Regex(@"^.*?(?<Command>K);(?<V_GUID>[0-9]*?);(?<V_ID>[0-9]*?);(?<V_TeamName>[a-zA-Z]*);(?<V_Name>.*);(?<K_GUID>[0-9]*?);(?<K_ID>[0-9]*?);(?<K_TeamName>[a-zA-Z]*);(?<K_Name>.*);(?<Weapon>[a-zA-Z0-9_]*);(?<Damage>[0-9]*?);(?<DamageType>[a-zA-Z_]*);(?<HitLocation>[a-zA-Z_]*)[\r]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(CallOfDutyKill) }
        };
        
        protected void ParseLogEvent(SourcePacket packet) {

            foreach (KeyValuePair<Regex, Type> command in SourceGame.EntryTypes) {

                Match matchedCommand = command.Key.Match(packet.String1);

                if (matchedCommand.Success == true) {
                    ISourceObject newObject = ((ISourceObject)Activator.CreateInstance(command.Value)).Parse(matchedCommand);

                    if (newObject is SourceChat) {
                        this.PostProcessChatHandler(packet, (SourceChat)newObject);
                    }
                    else if (newObject is SourceDisconnection) {
                        this.PostProcessDisconnectionHandler(packet, (SourceDisconnection)newObject);
                    }
                    //else if (newObject is CallOfDutyKill && this.KillEntry != null) {
                    //    this.KillEntry(this, entryTime, (CallOfDutyKill)newObject);
                    //}
                }
            }
        }

        protected void PostProcessChatHandler(SourcePacket packet, SourceChat chat) {
            // Find the full details of the player..
            chat.Author = this.State.PlayerList.Where(x => x.GUID == chat.Author.GUID).FirstOrDefault();

            this.ThrowGameEvent(GameEventType.Chat, chat);
        }

        protected void PostProcessDisconnectionHandler(SourcePacket packet, SourceDisconnection disconnection) {
            // Find the full details of the player..
            disconnection.Player = this.State.PlayerList.Where(x => x.GUID == disconnection.Player.GUID).FirstOrDefault();

            // Remove the player from the playerlist..
            this.State.PlayerList.RemoveAll(x => x.GUID == disconnection.Player.GUID);

            this.ThrowGameEvent(
                GameEventType.PlayerLeave,
                disconnection.Player
            );
        }

        // Log entry..
        // : "(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]+?)>"

        // Source login is a little odd that it sends a meaningless packet before the actual response
        // to the authentication. This makes requestPacket and the queue in SourceClient
        // null as it's got the response it was looking for already..
        protected void ServerAuthDispatchHandler(SourcePacket request, SourcePacket response) {
            if (response.RequestId >= 0) {
                // Login Success
                this.Client.ConnectionState = ConnectionState.LoggedIn;

                this.SendRequest("log");

                this.Synchronize();
            }
            else if (response.RequestId == -1) {

                // Error?

            }
        }

        [DispatchPacket(MatchText = "say")]
        public void ConsoleSayHandler(SourcePacket request, SourcePacket response) {

            SourceChat chat = new SourceChat().ParseConsoleSay(request.String1Words);

            this.ThrowGameEvent(GameEventType.Chat, chat);
        }

        [DispatchPacket(MatchText = "log")]
        public void ConsoleLogHandler(SourcePacket request, SourcePacket response) {

            if (response.String1.Contains("not currently logging") == true) {
                // Enable logging if it is off.
                this.SendRequest("log on");
            }
        }

        [DispatchPacket(MatchText = "status")]
        public void ConsoleDispatchHandler(SourcePacket request, SourcePacket response) {

            SourceServerInfo info = new SourceServerInfo().ParseStatusHeader(response.String1);

            this.State.Variables.ServerName     = info.hostname;
            this.State.Variables.MapName        = info.host_map;
            this.State.Variables.MaxPlayerCount = info.maxplayers;
            this.State.Variables.PlayerCount    = info.currentplayers;
            this.State.Variables.Version        = info.version;

            this.ThrowGameEvent(GameEventType.ServerInfoUpdated);

            SourcePlayerList players = new SourcePlayerList().Parse(response.String1);

            if (players.Subset.Context == PlayerSubsetContext.All) {

                // 1. Remove all names in the state list that are not found in the new list (players that have left)
                this.State.PlayerList.RemoveAll(x => players.Select(y => y.GUID).Contains(x.GUID) == false);

                // 2. Add or update any new players
                foreach (Player player in players) {
                    Player statePlayer = null;

                    if ((statePlayer = this.State.PlayerList.Find(x => x.GUID == player.GUID) as Player) == null) {
                        this.State.PlayerList.Add(player);
                    }
                    else {
                        // Already exists, update with any new information we have.
                        statePlayer.Ping = player.Ping;
                        statePlayer.Name = player.Name;
                    }
                }

                this.State.Variables.PlayerCount = players.Count;

                this.ThrowGameEvent(GameEventType.PlayerlistUpdated);

            }

            // Regex.

            // hostname[: ]*(?<hostname>.*)[\r\n]*version[: ]*(?<version>.*)[\r\n]*udp/ip.*[\r\n]*map[: ]*(?<map>[a-zA-Z0-9_]*) .*[\r\n]*players[: ]*(?<current_players>[0-9]*) \((?<max_players>[0-9]*).*[\r\n]*

            //if (request.Words.Count >= 3) {
            //    this.ThrowGameEvent(GameEventType.Chat, new FrostbiteChat().ParseAdminSay(request.Words.GetRange(1, request.Words.Count - 1)));
            //}
        }

        #endregion

        #region Packet Helpers

        #region Source specific

        protected void SendResponse(SourcePacket request, string format, params object[] args) {
            this.Send(new SourcePacket(request.Origin, true, request.RequestId, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty));
        }

        protected void SendRequest(string format, params object[] args) {
            this.Send(new SourcePacket(PacketOrigin.Client, false, (int)((SourceClient)this.Client).AcquireSequenceNumber, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty));
        }

        #endregion

        protected override SourcePacket Create(string format, params object[] args) {
            return new SourcePacket(PacketOrigin.Client, false, null, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty);
        }

        public override void Login(string password) {
            ((SourceClient)this.Client).SourceLogServicePort = this.SourceLogServicePort;

            if (this.SourceLogListenPort == null) {
                ((SourceClient)this.Client).SourceLogListenPort = (ushort)(this.SourceLogServicePort + 1);
            }
            else {
                ((SourceClient)this.Client).SourceLogListenPort = (ushort)this.SourceLogListenPort;
            }

            this.Send(new SourcePacket(PacketOrigin.Client, false, (int)((SourceClient)this.Client).AcquireSequenceNumber, SourceRequestType.SERVERDATA_AUTH, this.Password, String.Empty));
        }

        protected override void Action(Chat chat) {
            if (chat.Subset != null) {
                string subset = String.Empty;

                if (chat.Subset.Context == PlayerSubsetContext.All) {
                    this.SendRequest("say {0}", chat.Text);
                }
                /*
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
                */
            }
        }

        protected override void Action(Kick kick) {
            if (kick.Target != null) {
                if (kick.Reason != null && kick.Reason.Length > 0) {
                    this.SendRequest("kickid {0} {1}", kick.Target.GUID, kick.Reason);
                }
                else {
                    this.SendRequest("kickid {0}", kick.Target.GUID);
                }
            }
        }

        protected override void Action(Kill kill) {
            if (kill.Target != null) {

                this.SendRequest("kill {0}", kill.Target.Name);

                /*
                if (kick.Reason != null && kick.Reason.Length > 0) {
                    this.Send(this.Create("kick {0} \"{1}\"", kick.Target.GUID, kick.Reason));
                }
                else {
                    this.Send(this.Create("kick {0}", kick.Target.GUID));
                }
                */
            }
        }

        #endregion

        public override void Shutdown() {
            base.Shutdown();
        }

    }
}
