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
using System.ComponentModel;
using System.Linq;

namespace Procon.Core.Interfaces.Layer {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net;
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Utils;

    public class LayerGame : GameImplementation<LayerClient, LayerPacket>, ILayer
    {
        // Public Properties
        public  new String Hostname
        {
            get { return mHostname; }
            set {
                if (mHostname != value) {
                    mHostname = value;
                    OnPropertyChanged(this, "Hostname");
                }
            }
        }
        private String mHostname;

        public  new UInt16 Port
        {
            get { return mPort; }
            set {
                if (mPort != value) {
                    mPort = value;
                    OnPropertyChanged(this, "Port");
                }
            }
        }
        private UInt16 mPort;

        public  Boolean IsEncrypted
        {
            get { return mIsEncrypted; }
            set {
                if (mIsEncrypted != value) {
                    mIsEncrypted = value;
                    OnPropertyChanged(this, "IsEncrypted");
                }
            }
        }
        private Boolean mIsEncrypted;

        public  Boolean IsCompressed
        {
            get { return mIsCompressed; }
            set {
                if (mIsCompressed != value) {
                    mIsCompressed = value;
                    OnPropertyChanged(this, "IsCompressed");
                }
            }
        }
        private Boolean mIsCompressed;

        public  String Salt
        {
            get { return mSalt; }
            set {
                if (mSalt != value) {
                    mSalt = value;
                    OnPropertyChanged(this, "Salt");
                }
            }
        }
        private String mSalt;

        public  String Username
        {
            get { return mUsername; }
            set {
                if (mUsername != value) {
                    mUsername = value;
                    OnPropertyChanged(this, "Username");
                }
            }
        }
        private String mUsername;

        public ConnectionState ConnectionState
        {
            get { return (Client != null) ? Client.ConnectionState : ConnectionState.Disconnected; }
        }
        
        public  SecurityController Security
        {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;



        /// <summary>
        /// Initializes an instance of the LayerGame class.
        /// </summary>
        public LayerGame(LayerClient client) : base(client) {
        }
        /// <summary>
        /// Initializes an instance of the LayerGame class.
        /// </summary>
        public LayerGame(String hostName, UInt16 port) : base(hostName, port) {
            Hostname = hostName;
            Port     = port;
        }



        #region INotifyPropertyChanged

        /// <summary>
        /// Is fired whenever a property in this instance changes.  If a property of this
        /// instance is bound to in the UI, this will tell the UI to update the value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// This should be used to fire the PropertyChanged handler, so that the UI
        /// (and other instances) can be notified when a property changes.
        /// </summary>
        protected void OnPropertyChanged(object sender, String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
        }

        #endregion INotifyPropertyChanged
        #region ILayer

        /// <summary>
        /// Attempt to establish a connection to the layer.
        /// </summary>
        public void Begin()
        {
            AttemptConnection();
        }

        /// <summary>
        /// Sends a request to any connected clients, if the layer is a LayerListener.
        /// Sends a request to the connected interface, if the layer is a LayerGame.
        /// </summary>
        public void Request(Context context, CommandName command, EventName @event, params Object[] arguments)
        {
            this.Send(
                new LayerPacket() {
                    Context = context,
                    Request = new LayerCommand() {
                        Command   = command,
                        Event     = @event,
                        Arguments = new List<Object>(arguments)
                    }
                }
            );
        }

        /// <summary>
        /// Returns a Context object given a hostname and port of a connection.
        /// </summary>
        public Context ServerContext(String hostname, UInt16 port)
        {
            return new Context() {
                ContextType = ContextType.Connection,
                Hostname    = hostname,
                Port        = port
            };
        }

        #endregion
        #region Game

        /// <summary>
        /// Synchronizes the connection and client.
        /// </summary>
        public override void Synchronize() {
            // Unnecessary for LayerGame?
        }

        #endregion
        #region GameImplementation

        /// <summary>
        /// Assigns events listeners for various activities in sub-classes.
        /// </summary>
        protected override void AssignEvents()
        {
            base.AssignEvents();
            this.Client.ConnectionStateChanged += Client_ConnectionStateChanged;
        }

        /// <summary>
        /// Creates a client used to transmit information back and forth to the layer.
        /// </summary>
        protected override Client<LayerPacket> CreateClient(String hostName, UInt16 port)
        {
            return new LayerClient(hostName, port);
        }



        /// <summary>
        /// Invokes a command requested by the packet specified.
        /// Internally LayerGame only dispatches Commands.  It will ignore Events and just bubble them.
        /// </summary>
        protected override void Dispatch(LayerPacket packet)
        {
            if (packet.Response == null)
                Dispatch(packet.Request.Command.ToString(), packet, null);
            else
                Dispatch(packet.Response.Command.ToString(), packet, packet);
        }

        /// <summary>
        /// Fires ProcessLayerEvent whenever the method to invoke could not be found.
        /// </summary>
        protected override void DispatchFailed(String identifer, LayerPacket request, LayerPacket response)
        {
            if (this.Client.ConnectionState == ConnectionState.LoggedIn)
                if (request.Response == null)
                    OnProcessingLayerEvent(Username, request.Context, request.Request.Command, request.Request.Event, request.Request.Arguments.ToArray());
                else
                    OnProcessingLayerEvent(Username, request.Context, request.Response.Command, request.Response.Event, request.Response.Arguments.ToArray());
        }

        

        /// <summary>
        /// Creates a raw packet that could be sent to a layer.
        /// </summary>
        protected override LayerPacket Create(String format, params Object[] args) {
            // TODO: Maybe a packet to send a raw command to pass on to the server?
            return new LayerPacket();
        }
        
        /// <summary>
        /// Includes encryption and compression details before sending the packet to the layer.
        /// </summary>
        public override void Send(Packet packet) {

            // Pass details of encryption/compression as well as
            // a password and salt required to encrypt/decrypt
            (packet as LayerPacket).IsEncrypted  = IsEncrypted;
            (packet as LayerPacket).IsCompressed = IsCompressed;
            base.Send(packet);
        }

        /// <summary>
        /// Sends an general action to the layer.
        /// </summary>
        public override void Action(ProtocolObject action) {
            this.Send(
                new LayerPacket() {
                    Request = new LayerCommand() {
                        Command   = CommandName.Action,
                        Arguments = new List<Object>() { action }
                    }
                }
            );
        }

        /// <summary>
        /// Sends a Login request to the layer.
        /// </summary>
        public override void Login(String password) {
            this.Send(
                new LayerPacket() {
                    Request = new LayerCommand() {
                        Command   = CommandName.Login, // TODO: Check if this is slow.
                        Arguments = new List<object>() { }
                    }
                }
            );
        }

        #endregion
        #region Events

        // -- When Processing a Layer Event --

        public delegate void ProcessLayerEventHandler(String username, Context context, CommandName command, EventName @event, Object[] parameters);
        public event         ProcessLayerEventHandler ProcessLayerEvent;
        protected void OnProcessingLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters)
        {
            if (ProcessLayerEvent != null)
                ProcessLayerEvent(username, context, command, @event, parameters);
        }

        #endregion



        /// <summary>
        /// Listens for whenever the connection state to the layer changes.
        /// </summary>
        private void Client_ConnectionStateChanged(Client<LayerPacket> sender, ConnectionState newState) {
            OnPropertyChanged(this, "ConnectionState");
        }


        
        /// <summary>
        /// Sends a Salt login request to the layer.
        /// </summary>
        public void Login(LayerCommand request, Login salt) {
            this.Send(
                new LayerPacket() {
                    Request  = request,
                    Response = new LayerCommand() {
                        Command   = CommandName.Salt,
                        Arguments = new List<Object>() { salt }
                    }
                }
            );
        }

        /// <summary>
        /// Sends a Hashed login request to the layer.
        /// </summary>
        public void Hashed(Login hashed) {
            this.Send(
                new LayerPacket() {
                    Request = new LayerCommand() {
                        Command = CommandName.Hashed, // TODO: Check if this is slow.
                        Arguments = new List<Object>() { hashed }
                    }
                }
            );
        }

        /// <summary>
        /// Sends an Authenticated request to the layer.
        /// </summary>
        public void Authenticated(LayerCommand request, Login authenticated) {
            this.Send(
                new LayerPacket() {
                    Request  = request,
                    Response = new LayerCommand() {
                        Command   = CommandName.Authenticated,
                        Arguments = new List<Object>() { authenticated }
                    }
                }
            );
        }



        // Step 1) Client -> Server
        [DispatchPacket(MatchText = "Login")]
        public void LoginDispatchHandler(LayerPacket request, LayerPacket response) {
            // Request from a client for some salt
            string salt = SHA1.String((DateTime.Now.Ticks * new Random().NextDouble()).ToString());

            // Server -> Client, send them the salt.
            this.Login(request.Request, new Login() { Salt = salt });
        }

        // Step 2) Server -> Client
        [DispatchPacket(MatchText = "Salt")]
        public void SaltDispatchHandler(LayerPacket request, LayerPacket response) {
            Login details = request.Response.Arguments.Where(x => x is Login).FirstOrDefault() as Login;

            details.Username = this.Username;
            details.Hash = SHA1.String(this.Password + details.Salt);

            // Client -> Server, give the server our SHA1 salted password.
            this.Hashed(details);
        }

        // Step 3) Client -> Server
        [DispatchPacket(MatchText = "Hashed")]
        public void HashedDispatchHandler(LayerPacket request, LayerPacket response) {
            // Clone details so it will show as unauthenticated in the echo's request.
            Login details = (request.Request.Arguments.Where(x => x is Login).FirstOrDefault() as Login).Clone() as Login;

            details.Authenticated = this.Security.Authenticate(new LocalAccount() { Username = details.Username, Password = details.Hash }, details.Salt);

            // Server -> Client, send them the result of the authentication
            this.Authenticated(request.Request, details);

            if (details.Authenticated == true) {
                this.Username = details.Username;
  
                // Outgoing encryption
                // As the layer host we never bothered to learn their proper password.
                this.Password = this.Security.Password(new LocalAccount() { Username = details.Username });
                this.Salt = details.Salt;

                // Incoming decryption
                (this.Client as LayerClient).Password = this.Password;
                (this.Client as LayerClient).Salt = this.Salt;

                this.Client.ConnectionState = ConnectionState.LoggedIn;
            }
            else {
                this.Shutdown();
            }
        }

        // Step 4) Server -> Client
        [DispatchPacket(MatchText = "Authenticated")]
        public void AuthenticatedDispatchHandler(LayerPacket request, LayerPacket response) {
            Login details = request.Response.Arguments.Where(x => x is Login).FirstOrDefault() as Login;

            if (details.Authenticated == true) {
                // Outgoing encryption
                // Since this is the client, we'll know the password already
                this.Salt = details.Salt;

                // Incoming decryption
                (this.Client as LayerClient).Password = this.Password;
                (this.Client as LayerClient).Salt = this.Salt;

                this.Client.ConnectionState = ConnectionState.LoggedIn;
            }
            else {
                // if this is the case, the remote host has probably terminated the connection anyway
                // as it sends this final packet and terminates it itself.
                this.Shutdown();
            }
        }
    }
}
