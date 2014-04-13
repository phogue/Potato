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
using System.Collections.Generic;
using System.Linq;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;

namespace Procon.Net.Shared {
    /// <summary>
    /// The base implementation of a game to be used by more specialized protocols
    /// </summary>
    public abstract class Protocol : IProtocol {
        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        public IClient Client { get; protected set; }

        /// <summary>
        /// Manages actions and potential events to fire once an action has completed or expired.
        /// </summary>
        public IWaitingActions WaitingActions { get; set; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        public IProtocolState State { get; protected set; }

        public IProtocolSetup Options { get; protected set; }

        /// <summary>
        /// Describing attribute of this game.
        /// </summary>
        public virtual IProtocolType ProtocolType {
            get {
                if (this._gameType == null) {
                    IProtocolType attribute = this.GetType().GetCustomAttributes(typeof(IProtocolType), false).Cast<IProtocolType>().FirstOrDefault();

                    if (attribute != null) {
                        this._gameType = new ProtocolType(attribute);
                    }
                }

                return _gameType; 
            }
        }
        private IProtocolType _gameType;

        /// <summary>
        /// Handles all packet dispatching.
        /// </summary>
        public IPacketDispatcher PacketDispatcher { get; set; }

        protected Protocol() {
            this.Options = new ProtocolSetup();
            this.State = new ProtocolState();
            this.PacketDispatcher = new PacketDispatcher();

            this.WaitingActions = new WaitingActions() {
                Done = (action, requests, responses) => this.OnClientEvent(
                    ClientEventType.ClientActionDone,
                    new ClientEventData() {
                        Packets = responses
                    },
                    new ClientEventData() {
                        Actions = new List<INetworkAction>() {
                            action
                        },
                        Packets = requests
                    }
                ),
                Expired = (action, requests, responses) => this.OnClientEvent(
                    ClientEventType.ClientActionExpired,
                    new ClientEventData() {
                        Packets = responses
                    },
                    new ClientEventData() {
                        Actions = new List<INetworkAction>() {
                            action
                        },
                        Packets = requests
                    }
                ),
            };
        }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        public virtual event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;

        protected void OnProtocolEvent(ProtocolEventType eventType, IProtocolStateDifference difference, IProtocolEventData now = null, IProtocolEventData then = null) {
            var handler = this.ProtocolEvent;

            // Apply any differences to our state object.
            this.State.Apply(difference);

            if (handler != null) {
                handler(
                    this,
                    new ProtocolEventArgs() {
                        ProtocolEventType = eventType,
                        ProtocolType = this.ProtocolType as ProtocolType, // Required for serialization. How to get around?
                        StateDifference = difference ?? new ProtocolStateDifference(),
                        Now = now ?? new ProtocolEventData(),
                        Then = then ?? new ProtocolEventData()
                    }
                );
            }
        }

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        public virtual event Action<IProtocol, IClientEventArgs> ClientEvent;

        protected void OnClientEvent(ClientEventType eventType, IClientEventData now = null, IClientEventData then = null) {
            var handler = this.ClientEvent;
            if (handler != null) {
                handler(this, new ClientEventArgs() {
                    EventType = eventType,
                    ConnectionState = this.Client.ConnectionState,
                    Now = now ?? new ClientEventData(),
                    Then = then ?? new ClientEventData()
                });
            }
        }

        /// <summary>
        /// Initiates the login to the game server.
        /// </summary>
        /// <param name="password"></param>
        protected abstract void Login(string password);

        /// <summary>
        /// Process a generic network action
        /// </summary>
        /// <param name="action"></param>
        public virtual List<IPacket> Action(INetworkAction action) {
            List<IPacket> packets = null;
            List<IPacketWrapper> wrappers = this.DispatchAction(action);

            if (wrappers != null) {
                // Fetch all of the packets that are not null
                packets = wrappers.Where(wrapper => wrapper != null).Select(wrapper => wrapper.Packet).ToList();

                // Defer this completed action for now.
                this.WaitingActions.Wait(action, packets);

                // Now send the packets
                foreach (IPacketWrapper wrapper in wrappers) {
                    this.Send(wrapper);
                }
            }

            return packets;
        }

        protected virtual List<IPacketWrapper> DispatchAction(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            switch (action.ActionType) {
                case NetworkActionType.NetworkTextSay:
                case NetworkActionType.NetworkTextYell:
                case NetworkActionType.NetworkTextYellOnly:
                    wrappers = this.ActionChat(action);
                    break;

                case NetworkActionType.NetworkPlayerKill:
                    wrappers = this.ActionKill(action);
                    break;

                case NetworkActionType.NetworkPlayerKick:
                    wrappers = this.ActionKick(action);
                    break;

                case NetworkActionType.NetworkPlayerBan:
                case NetworkActionType.NetworkPlayerUnban:
                    wrappers = this.ActionBan(action);
                    break;

                case NetworkActionType.NetworkMapAppend:
                case NetworkActionType.NetworkMapChangeMode:
                case NetworkActionType.NetworkMapClear:
                case NetworkActionType.NetworkMapInsert:
                case NetworkActionType.NetworkMapNext:
                case NetworkActionType.NetworkMapNextIndex:
                case NetworkActionType.NetworkMapRemove:
                case NetworkActionType.NetworkMapRemoveIndex:
                case NetworkActionType.NetworkMapRestart:
                case NetworkActionType.NetworkMapRoundNext:
                case NetworkActionType.NetworkMapRoundRestart:
                    wrappers = this.ActionMap(action);
                    break;

                case NetworkActionType.NetworkPlayerMoveRotate:
                case NetworkActionType.NetworkPlayerMoveRotateForce:
                case NetworkActionType.NetworkPlayerMove:
                    wrappers = this.ActionMove(action);
                    break;
                case NetworkActionType.NetworkPacketSend:
                    wrappers.AddRange(action.Now.Content.Select(text => this.CreatePacket(text)));

                    wrappers.AddRange(action.Now.Packets.Select(this.WrapPacket));
                    break;
            }

            return wrappers;
        }

        protected abstract List<IPacketWrapper> ActionChat(INetworkAction action);

        protected abstract List<IPacketWrapper> ActionKill(INetworkAction action);

        protected abstract List<IPacketWrapper> ActionKick(INetworkAction action);

        protected abstract List<IPacketWrapper> ActionBan(INetworkAction action);

        protected abstract List<IPacketWrapper> ActionMove(INetworkAction action);

        protected abstract List<IPacketWrapper> ActionMap(INetworkAction action);

        protected virtual List<IPacketWrapper> ActionRaw(INetworkAction action) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            wrappers.AddRange(action.Now.Content.Select(text => this.CreatePacket(text)));

            wrappers.AddRange(action.Now.Packets.Select(this.WrapPacket));

            return wrappers;
        }

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="wrapper"></param>
        public virtual IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper != null) {
                if (this.Client != null && (this.Client.ConnectionState == ConnectionState.ConnectionReady || this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                    sent = this.Client.Send(wrapper);
                }
            }

            return sent;
        }

        public virtual IProtocolSetupResult Setup(IProtocolSetup setup) {
            this.Options = setup;
            this.Client.Setup(ClientSetup.FromProtocolSetup(setup));

            // Handle client events, most of which are proxies to events we fire.
            this.Client.PacketReceived += (sender, wrapper) => {
                this.PacketDispatcher.Dispatch(wrapper);

                // Alert the deferrer that we have a new packet that's been dispatched
                this.WaitingActions.Mark(wrapper.Packet);

                this.OnClientEvent(ClientEventType.ClientPacketReceived, new ClientEventData() {
                    Packets = new List<IPacket>() {
                        wrapper.Packet
                    }
                });
            };

            this.Client.ConnectionStateChanged += (sender, state) => {
                this.OnClientEvent(ClientEventType.ClientConnectionStateChange);

                this.State.Settings.Current.ConnectionState = state;

                this.OnProtocolEvent(ProtocolEventType.ProtocolSettingsUpdated, new ProtocolStateDifference() {
                    Modified = {
                        Settings = this.State.Settings
                    }
                });

                if (state == ConnectionState.ConnectionReady) {
                    this.Login(this.Options.Password);
                }
            };

            this.Client.PacketSent += (sender, wrapper) => this.OnClientEvent(ClientEventType.ClientPacketSent, new ClientEventData() {
                Packets = new List<IPacket>() {
                    wrapper.Packet
                }
            });

            this.Client.SocketException += (sender, se) => this.OnClientEvent(ClientEventType.ClientSocketException, new ClientEventData() {
                Exceptions = new List<String>() {
                    se.ToString()
                }
            });

            this.Client.ConnectionFailure += (sender, exception) => this.OnClientEvent(ClientEventType.ClientConnectionFailure, new ClientEventData() {
                Exceptions = new List<String>() {
                    exception.ToString()
                }
            });

            return new ProtocolSetupResult();
        }

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        public void AttemptConnection() {
            if (this.Client != null && this.Client.ConnectionState == ConnectionState.ConnectionDisconnected) {
                this.Client.Connect();
            }
        }

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        public virtual void Shutdown() {
            if (this.Client != null) {
                this.Client.Shutdown();
            }
        }

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        public virtual void Synchronize() {
            if (this.Client != null && this.Client.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                this.Client.Poke();
            }
        }

        /// <summary>
        /// Create a packet from a string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper CreatePacket(String format, params object[] args);

        /// <summary>
        /// Wraps a completed packet in a packet wrapper, allowing it to be sent to the server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper WrapPacket(IPacket packet);
    }
}
