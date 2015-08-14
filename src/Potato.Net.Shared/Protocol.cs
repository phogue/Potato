#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Actions.Deferred;

namespace Potato.Net.Shared {
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
                if (_gameType == null) {
                    var attribute = GetType().GetCustomAttributes(typeof(IProtocolType), false).Cast<IProtocolType>().FirstOrDefault();

                    if (attribute != null) {
                        _gameType = new ProtocolType(attribute);
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
            Options = new ProtocolSetup();
            State = new ProtocolState();
            PacketDispatcher = new PacketDispatcher();

            WaitingActions = new WaitingActions() {
                Done = (action, requests, responses) => OnClientEvent(
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
                Expired = (action, requests, responses) => OnClientEvent(
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
        /// Helper to apply the difference generated from an event.
        /// </summary>
        /// <param name="difference">The difference object to apply to the protocols state</param>
        protected void ApplyProtocolStateDifference(IProtocolStateDifference difference) {
            if (State != null) {
                // Apply any differences to our state object.
                State.Apply(difference);
            }
        }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        public virtual event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;

        protected void OnProtocolEvent(ProtocolEventType eventType, IProtocolStateDifference difference, IProtocolEventData now = null, IProtocolEventData then = null) {
            var handler = ProtocolEvent;

            if (handler != null) {
                handler(
                    this,
                    new ProtocolEventArgs() {
                        ProtocolEventType = eventType,
                        ProtocolType = ProtocolType as ProtocolType, // Required for serialization. How to get around?
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
            var handler = ClientEvent;
            if (handler != null) {
                handler(this, new ClientEventArgs() {
                    EventType = eventType,
                    ConnectionState = Client.ConnectionState,
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
            var wrappers = DispatchAction(action);

            if (wrappers != null) {
                // Fetch all of the packets that are not null
                packets = wrappers.Where(wrapper => wrapper != null).Select(wrapper => wrapper.Packet).ToList();

                // Defer this completed action for now.
                WaitingActions.Wait(action, packets);

                // Now send the packets
                foreach (var wrapper in wrappers) {
                    Send(wrapper);
                }
            }

            return packets;
        }

        protected virtual List<IPacketWrapper> DispatchAction(INetworkAction action) {
            var wrappers = new List<IPacketWrapper>();

            switch (action.ActionType) {
                case NetworkActionType.NetworkTextSay:
                case NetworkActionType.NetworkTextYell:
                case NetworkActionType.NetworkTextYellOnly:
                    wrappers = ActionChat(action);
                    break;

                case NetworkActionType.NetworkPlayerKill:
                    wrappers = ActionKill(action);
                    break;

                case NetworkActionType.NetworkPlayerKick:
                    wrappers = ActionKick(action);
                    break;

                case NetworkActionType.NetworkPlayerBan:
                case NetworkActionType.NetworkPlayerUnban:
                    wrappers = ActionBan(action);
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
                    wrappers = ActionMap(action);
                    break;

                case NetworkActionType.NetworkPlayerMoveRotate:
                case NetworkActionType.NetworkPlayerMoveRotateForce:
                case NetworkActionType.NetworkPlayerMove:
                case NetworkActionType.NetworkPlayerMoveForce:
                    wrappers = ActionMove(action);
                    break;
                case NetworkActionType.NetworkPacketSend:
                    if (action.Now.Content != null) {
                        wrappers.AddRange(action.Now.Content.Select(text => CreatePacket(text)));
                    }

                    if (action.Now.Packets != null) {
                        wrappers.AddRange(action.Now.Packets.Select(WrapPacket));
                    }
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
            var wrappers = new List<IPacketWrapper>();

            wrappers.AddRange(action.Now.Content.Select(text => CreatePacket(text)));

            wrappers.AddRange(action.Now.Packets.Select(WrapPacket));

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
                if (Client != null && (Client.ConnectionState == ConnectionState.ConnectionReady || Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                    sent = Client.Send(wrapper);
                }
            }

            return sent;
        }

        public virtual IProtocolSetupResult Setup(IProtocolSetup setup) {
            Options = setup;
            Client.Setup(ClientSetup.FromProtocolSetup(setup));

            // Handle client events, most of which are proxies to events we fire.
            Client.PacketReceived += (sender, wrapper) => {
                PacketDispatcher.Dispatch(wrapper);

                // Alert the deferrer that we have a new packet that's been dispatched
                WaitingActions.Mark(wrapper.Packet);

                OnClientEvent(ClientEventType.ClientPacketReceived, new ClientEventData() {
                    Packets = new List<IPacket>() {
                        wrapper.Packet
                    }
                });
            };

            Client.ConnectionStateChanged += (sender, state) => {
                OnClientEvent(ClientEventType.ClientConnectionStateChange);

                State.Settings.Current.ConnectionState = state;

                IProtocolStateDifference difference = new ProtocolStateDifference() {
                    Modified = {
                        Settings = State.Settings
                    }
                };

                ApplyProtocolStateDifference(difference);

                OnProtocolEvent(ProtocolEventType.ProtocolSettingsUpdated, difference);

                if (state == ConnectionState.ConnectionReady) {
                    Login(Options.Password);
                }
            };

            Client.PacketSent += (sender, wrapper) => OnClientEvent(ClientEventType.ClientPacketSent, new ClientEventData() {
                Packets = new List<IPacket>() {
                    wrapper.Packet
                }
            });

            Client.SocketException += (sender, se) => OnClientEvent(ClientEventType.ClientSocketException, new ClientEventData() {
                Exceptions = new List<string>() {
                    se.ToString()
                }
            });

            Client.ConnectionFailure += (sender, exception) => OnClientEvent(ClientEventType.ClientConnectionFailure, new ClientEventData() {
                Exceptions = new List<string>() {
                    exception.ToString()
                }
            });

            return new ProtocolSetupResult();
        }

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        public void AttemptConnection() {
            if (Client != null && Client.ConnectionState == ConnectionState.ConnectionDisconnected) {
                Client.Connect();
            }
        }

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        public virtual void Shutdown() {
            if (Client != null) {
                Client.Shutdown();
            }
        }

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        public virtual void Synchronize() {
            if (Client != null && Client.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                Client.Poke();
            }
        }

        /// <summary>
        /// Create a packet from a string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper CreatePacket(string format, params object[] args);

        /// <summary>
        /// Wraps a completed packet in a packet wrapper, allowing it to be sent to the server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper WrapPacket(IPacket packet);
    }
}
