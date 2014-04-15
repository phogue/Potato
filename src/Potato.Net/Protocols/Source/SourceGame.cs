using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Potato.Net.Protocols.Source {
    using Potato.Net.Protocols.Objects;
    using Potato.Net.Protocols.Source.Objects;

    public class SourceGame : Game {

        public ushort SourceLogServicePort { get; set; }
        public ushort? SourceLogListenPort { get; set; }

        public SourceGame(string hostName, ushort port) : base(hostName, port) {
            this.SourceLogServicePort = 8787;

            this.PacketDispatcher.Append(new Dictionary<PacketDispatch, PacketDispatcher.PacketDispatchHandler>() {
                {
                    new PacketDispatch() { Name = "say" },
                    new PacketDispatcher.PacketDispatchHandler(this.ConsoleSayHandler)
                }, {
                    new PacketDispatch() { Name = "log" },
                    new PacketDispatcher.PacketDispatchHandler(this.ConsoleLogHandler)
                }, {
                    new PacketDispatch() { Name = "status" },
                    new PacketDispatcher.PacketDispatchHandler(this.ConsoleDispatchHandler)
                }, {
                    new PacketDispatch() { Name = "login" },
                    new PacketDispatcher.PacketDispatchHandler(this.ServerAuthDispatchHandler)
                }, {
                    new PacketDispatch() { Name = "log" },
                    new PacketDispatcher.PacketDispatchHandler(this.LogRequestDispatchHandler)
                }
            });
        }

        protected override IPacketDispatcher CreatePacketDispatcher() {
            return new SourcePacketDispatcher() {
                // Note. PacketQueue should be set here once Client is converted over.
            };
        }

        protected override IClient CreateClient(string hostName, ushort port) {
            return new SourceClient(hostName, port);
        }

        public override void Synchronize() {
            this.SendRequest("status");
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
            // { new Regex(@"^.*?(?<Command>K);(?<V_Uid>[0-9]*?);(?<V_ID>[0-9]*?);(?<V_TeamName>[a-zA-Z]*);(?<V_Name>.*);(?<K_Uid>[0-9]*?);(?<K_ID>[0-9]*?);(?<K_TeamName>[a-zA-Z]*);(?<K_Name>.*);(?<Weapon>[a-zA-Z0-9_]*);(?<Damage>[0-9]*?);(?<DamageType>[a-zA-Z_]*);(?<HitLocation>[a-zA-Z_]*)[\r]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(CallOfDutyKill) }
        };
        
        protected void LogRequestDispatchHandler(Packet request, Packet response) {

            SourcePacket sourceRequest = request as SourcePacket;

            if (sourceRequest != null) {
                foreach (KeyValuePair<Regex, Type> command in SourceGame.EntryTypes) {

                    Match matchedCommand = command.Key.Match(sourceRequest.String1);

                    if (matchedCommand.Success == true) {

                        NetworkObject newObject = ((ISourceObject)Activator.CreateInstance(command.Value)).Parse(matchedCommand);

                        if (newObject is Chat) {
                            this.PostProcessChatHandler(sourceRequest, (Chat)newObject);
                        }
                        //else if (newObject is SourceDisconnection) {
                        //    this.PostProcessDisconnectionHandler(packet, (SourceDisconnection)newObject);
                        //}
                    }
                }
            }
        }

        protected void PostProcessChatHandler(SourcePacket packet, Chat chat) {
            if (chat.Now.Players != null) {
                // Fill it with the completed player object.
                chat.Now.Players = new List<Player>() {
                    this.State.PlayerList.Find(x => x.Uid == chat.Now.Players.First().Uid)
                };

                this.OnGameEvent(GameEventType.GameChat, new GameEventData() { Chats = new List<Chat>() { chat } });
            }
        }

        protected void PostProcessDisconnectionHandler(SourcePacket packet, SourceDisconnection disconnection) {
            // Find the full details of the player..
            disconnection.Player = this.State.PlayerList.FirstOrDefault(x => x.Uid == disconnection.Player.Uid);

            // Remove the player from the playerlist..
            this.State.PlayerList.RemoveAll(x => x.Uid == disconnection.Player.Uid);

            this.OnGameEvent(
                GameEventType.GamePlayerLeave,
                new GameEventData() { Players = new List<Player>() { disconnection.Player } }
            );
        }

        // Log entry..
        // : "(?<name>.*?)<(?<userid>[0-9]{1,3})><(?<uniqueid>STEAM_[0-9:]+?)><(?<team>[A-Za-z]+?)>"

        // Source login is a little odd that it sends a meaningless packet before the actual response
        // to the authentication. This makes requestPacket and the queue in SourceClient
        // null as it's got the response it was looking for already..
        protected void ServerAuthDispatchHandler(Packet request, Packet response) {
            if (response.RequestId >= 0) {
                // Login Success
                this.Client.ConnectionState = ConnectionState.ConnectionLoggedIn;

                this.SendRequest("log");

                this.Synchronize();
            }
            else if (response.RequestId == -1) {

                // Error?

            }
        }

        public void ConsoleSayHandler(Packet request, Packet response) {
            SourcePacket sourceRequest = request as SourcePacket;

            if (sourceRequest != null) {
                Chat chat = new SourceChat().ParseConsoleSay(sourceRequest.Words);

                this.OnGameEvent(GameEventType.GameChat, new GameEventData() { Chats = new List<Chat>() { chat } });
            }
        }

        public void ConsoleLogHandler(Packet request, Packet response) {
            SourcePacket sourceResponse = request as SourcePacket;

            if (sourceResponse != null) {
                if (sourceResponse.String1.Contains("not currently logging") == true) {
                    // Enable logging if it is off.
                    this.SendRequest("log on");
                }
            }
        }

        public void ConsoleDispatchHandler(Packet request, Packet response) {
            SourcePacket sourceResponse = request as SourcePacket;

            if (sourceResponse != null) {
                SourceServerInfo info = new SourceServerInfo().ParseStatusHeader(sourceResponse.String1);

                this.State.Settings.ServerName = info.hostname;
                this.State.Settings.MapName = info.host_map;
                this.State.Settings.MaxPlayerCount = info.maxplayers;
                this.State.Settings.PlayerCount = info.currentplayers;
                this.State.Settings.ServerVersion = info.version;

                this.OnGameEvent(GameEventType.GameSettingsUpdated);

                SourcePlayerList players = new SourcePlayerList().Parse(sourceResponse.String1);

                // If there are no limitations on the subset.
                if (players.Subset.Count == 0) {

                    // 1. Remove all names in the state list that are not found in the new list (players that have left)
                    this.State.PlayerList.RemoveAll(x => players.Select(y => y.Uid).Contains(x.Uid) == false);

                    // 2. Add or update any new players
                    foreach (Player player in players) {
                        Player statePlayer = this.State.PlayerList.Find(x => x.Uid == player.Uid);

                        if (statePlayer == null) {
                            this.State.PlayerList.Add(player);
                        }
                        else {
                            // Already exists, update with any new information we have.
                            statePlayer.Ping = player.Ping;
                            statePlayer.Name = player.Name;
                        }
                    }

                    this.State.Settings.PlayerCount = players.Count;

                    this.OnGameEvent(GameEventType.GamePlayerlistUpdated);
                }
            }

            // Regex.

            // hostname[: ]*(?<hostname>.*)[\r\n]*version[: ]*(?<version>.*)[\r\n]*udp/ip.*[\r\n]*map[: ]*(?<map>[a-zA-Z0-9_]*) .*[\r\n]*players[: ]*(?<current_players>[0-9]*) \((?<max_players>[0-9]*).*[\r\n]*

            //if (request.Words.Count >= 3) {
            //    this.ThrowGameEvent(GameEventType.Chat, new FrostbiteChat().ParseAdminSay(request.Words.GetRange(1, request.Words.Count - 1)));
            //}
        }

        protected void SendResponse(SourcePacket request, string format, params object[] args) {
            this.Send(new SourcePacket(request.Origin, PacketType.Response, request.RequestId, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty));
        }

        protected void SendRequest(string format, params object[] args) {
            this.Send(new SourcePacket(PacketOrigin.Client, PacketType.Request, ((SourceClient)this.Client).AcquireSequenceNumber, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty));
        }

        protected override Packet CreatePacket(string format, params object[] args) {
            return new SourcePacket(PacketOrigin.Client, PacketType.Request, null, SourceRequestType.SERVERDATA_EXECCOMMAND, String.Format(format, args), String.Empty);
        }

        protected override void Login(string password) {
            ((SourceClient)this.Client).SourceLogServicePort = this.SourceLogServicePort;

            if (this.SourceLogListenPort == null) {
                ((SourceClient)this.Client).SourceLogListenPort = (ushort)(this.SourceLogServicePort + 1);
            }
            else {
                ((SourceClient)this.Client).SourceLogListenPort = (ushort)this.SourceLogListenPort;
            }

            this.Send(new SourcePacket(PacketOrigin.Client, PacketType.Request, ((SourceClient)this.Client).AcquireSequenceNumber, SourceRequestType.SERVERDATA_AUTH, this.Password, String.Empty));
        }

        protected override void Action(Chat chat) {
            if (chat.Now.Content != null) {
                foreach (String chatMessage in chat.Now.Content) {
                    this.SendRequest("say {0}", chatMessage);
                }
            }
        }

        protected override void Action(Kick kick) {
            String reason = kick.Scope.Content != null ? kick.Scope.Content.FirstOrDefault() : String.Empty;

            foreach (Player player in kick.Scope.Players) {
                this.Send(String.IsNullOrEmpty(reason) == false ? this.CreatePacket("kickid {0} {1}", player.SlotId, reason) : this.CreatePacket("kickid {0}", player.SlotId));
            }
        }

        protected override void Action(Kill kill) {
            if (kill.Scope.Players != null && kill.Scope.Players.Count > 0) {

                this.SendRequest("kill {0}", kill.Scope.Players.First().Name);

                /*
                if (kick.Reason != null && kick.Reason.Length > 0) {
                    this.Send(this.Create("kick {0} \"{1}\"", kick.Target.Uid, kick.Reason));
                }
                else {
                    this.Send(this.Create("kick {0}", kick.Target.Uid));
                }
                */
            }
        }
    }
}
