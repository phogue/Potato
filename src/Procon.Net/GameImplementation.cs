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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;

namespace Procon.Net {
    using Procon.Net.Attributes;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Utils;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;

    public abstract class GameImplementation<C, P> : Game
        where C : Procon.Net.Client<P>
        where P : Packet {




        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Client<P> Client { get; protected set; }

        protected Dictionary<string, MethodInfo> m_dispatchHandlers;

        public override string Hostname { get { return this.Client != null ? this.Client.Hostname : String.Empty; } }
        public override ushort Port { get { return this.Client != null ? this.Client.Port : (ushort)0; } }

        /*
        public override ConnectionState ConnectionState {
            get {
                return this.Client != null ? this.Client.ConnectionState : Net.ConnectionState.Disconnected;
            }
        }
        */
        private GameType m_gameType = GameType.None;
        public GameType GameType {
            get {
                if (this.m_gameType == Protocols.GameType.None) {
                    GameAttribute attribute = (this.GetType().GetCustomAttributes(typeof(GameAttribute), false) as GameAttribute[]).FirstOrDefault();

                    if (attribute != null) {
                        this.m_gameType = attribute.GameType;
                    }
                }

                return this.m_gameType;
            }
        }

        #region Events

        public override event GameEventHandler GameEvent;
        public override event ClientEventHandler ClientEvent;

        protected ClientEventArgs ThrowClientEvent(ClientEventType eventType, Packet packet = null, Exception exception = null) {
            ClientEventArgs result = null;

            if (this.ClientEvent != null) {
                this.ClientEvent(this,
                    result = new ClientEventArgs() {
                        EventType = eventType,
                        ConnectionState = this.Client.ConnectionState,
                        ConnectionError = exception,
                        Packet = packet
                    }
                );
            }

            return result;
        }

        protected void ThrowGameEvent(GameEventType eventType) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Chat chat = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Chat = chat
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Player player = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Player = player
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Kill kill = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Kill = kill
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Kick kick = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Kick = kick
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Spawn spawn = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Spawn = spawn
                    }
                );
            }
        }

        protected void ThrowGameEvent(GameEventType eventType, Ban ban = null) {
            if (this.GameEvent != null) {
                this.GameEvent(
                    this,
                    new GameEventArgs() {
                        EventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Ban = ban
                    }
                );
            }
        }

        // public override event PacketDispatchHandler PacketSent;
        // public override event PacketDispatchHandler PacketReceived;
        // public override event SocketExceptionHandler SocketException;
        // public override event FailureHandler ConnectionFailure;
        // public override event ConnectionStateChangedHandler ConnectionStateChanged;

        #endregion

        public GameImplementation(Procon.Net.Client<P> client) : base() {
            this.Execute(client);
        }

        public GameImplementation(string hostName, ushort port) : base() {
            this.Execute(this.CreateClient(hostName, port));
        }
  
        private void Execute(Procon.Net.Client<P> client) {
            this.State = new GameState();
            this.Client = client;
            this.AssignEvents();

            this.m_dispatchHandlers = new Dictionary<string, MethodInfo>();

            foreach (MethodInfo method in this.GetType().GetMethods()) {

                ParameterInfo[] parameters = method.GetParameters(); ;

                if (parameters.Length == 2 && typeof(P).IsAssignableFrom(parameters[0].ParameterType) && typeof(P).IsAssignableFrom(parameters[1].ParameterType)) {

                    object[] attributes = method.GetCustomAttributes(typeof(DispatchPacketAttribute), false);

                    if (attributes.Length > 0) {

                        DispatchPacketAttribute attribute = (DispatchPacketAttribute)attributes[0];

                        if (this.m_dispatchHandlers.ContainsKey(attribute.MatchText) == false) {
                            this.m_dispatchHandlers.Add(attribute.MatchText, method);
                        }
                    }
                }
            }
        }

        protected abstract Client<P> CreateClient(string hostName, ushort port);
        protected abstract void Dispatch(P packet);

        public override void Send(Packet packet) {
            if (this.Client != null && (this.Client.ConnectionState == ConnectionState.Ready || this.Client.ConnectionState == ConnectionState.LoggedIn)) {
                this.Client.Send((P)packet);
            }
        }

        protected virtual void AssignEvents() {
            this.Client.PacketReceived += new Net.Client<P>.PacketDispatchHandler(Client_PacketReceived);
            this.Client.PacketSent += new Client<P>.PacketDispatchHandler(Client_PacketSent);
            this.Client.SocketException += new Client<P>.SocketExceptionHandler(Client_SocketException);
            this.Client.ConnectionStateChanged += new Client<P>.ConnectionStateChangedHandler(Client_ConnectionStateChanged);
            this.Client.ConnectionFailure += new Client<P>.FailureHandler(Client_ConnectionFailure);
        }
        
        private void Client_ConnectionFailure(Client<P> sender, Exception exception) {
            this.ThrowClientEvent(ClientEventType.ConnectionFailure, null, exception);
        }

        private void Client_ConnectionStateChanged(Client<P> sender, ConnectionState newState) {
            this.ThrowClientEvent(ClientEventType.ConnectionStateChange);

            this.State.Variables.ConnectionState = newState;

            if (newState == ConnectionState.Ready) {
                this.Login(this.Password);
            }
        }

        private void Client_SocketException(Client<P> sender, System.Net.Sockets.SocketException se) {
            this.ThrowClientEvent(ClientEventType.SocketException, null, se);
        }

        private void Client_PacketSent(Client<P> sender, P packet) {
            this.ThrowClientEvent(ClientEventType.PacketSent, packet, null);
        }

        private void Client_PacketReceived(Client<P> sender, P packet) {
            this.ThrowClientEvent(ClientEventType.PacketReceived, packet, null);

            this.Dispatch(packet);
        }
        
        protected void Dispatch(string identifer, P request, P response) {

            if (this.m_dispatchHandlers.ContainsKey(identifer) == true) {
                this.m_dispatchHandlers[identifer].Invoke(this, new object[] { request, response });
            }
            else {
                this.DispatchFailed(identifer, request, response);
            }
        }

        protected virtual void DispatchFailed(string identifer, P request, P response) {

        }

        public override void AttemptConnection() {
            if (this.Client != null && this.Client.ConnectionState == ConnectionState.Disconnected) {
                this.Client.AttemptConnection();
            }
        }

        #region Helper Methods

        protected abstract P Create(string format, params object[] args);

        public override void Raw(string format, params object[] args) {
            this.Send(this.Create(format, args));
        }

        public override void Login(string password) {
            
        }

        public override void Action(ProtocolObject action) {
            if (action is Chat) {
                this.Action(action as Chat);
            }
            else if (action is Kick) {
                this.Action(action as Kick);
            }
            else if (action is Ban) {
                this.Action(action as Ban);
            }
            else if (action is Map) {
                this.Action(action as Map);
            }
            else if (action is Kill) {
                this.Action(action as Kill);
            }
            else if (action is Move) {
                this.Action(action as Move);
            }
        }

        protected virtual void Action(Chat chat) {

        }

        protected virtual void Action(Kick kick) {

        }

        protected virtual void Action(Ban ban) {

        }

        protected virtual void Action(Map map) {

        }

        protected virtual void Action(Kill kill) {

        }

        protected virtual void Action(Move move) {

        }

        #endregion

        #region Game Config Loading

        protected virtual void ExecuteGameConfigGamemode(XElement gamemode) {
            this.State.GameModePool.Add(new GameMode().Deserialize(gamemode));
        }

        protected virtual void ExecuteGameConfigMap(XElement map) {
            this.State.MapPool.Add(
                new Map() {
                    GameMode = this.State.GameModePool.Find(x => String.Compare(x.Name, map.ElementValue("gamemode")) == 0)
                }.Deserialize(map)
            );
        }

        protected void ExecuteGameConfig(string game) {

            if (this.State.GameModePool.Count == 0 && this.State.MapPool.Count == 0 && Game.GameConfig != null) {
                foreach (XElement element in Game.GameConfig.Descendants(game).Descendants("gamemodes").Descendants("gamemode")) {
                    this.ExecuteGameConfigGamemode(element);
                }

                foreach (XElement element in Game.GameConfig.Descendants(game).Descendants("maps").Descendants("map")) {
                    this.ExecuteGameConfigMap(element);
                }

                this.ThrowGameEvent(GameEventType.GameConfigExecuted);
            }
        }
        
        #endregion

        public override void Shutdown() {
            if (this.Client != null) {
                this.Client.Shutdown();
            }
        }
    }
}
