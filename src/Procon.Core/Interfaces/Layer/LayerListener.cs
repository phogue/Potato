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
using System.Net;
using System.Net.Sockets;

namespace Procon.Core.Interfaces.Layer {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security;
    using Procon.Net;

    public class LayerListener : ILayer
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

        public  ConnectionState ConnectionState
        {
            get { return mConnectionState; }
            set {
                if (mConnectionState != value) {
                    mConnectionState = value;
                    OnPropertyChanged(this, "ConnectionState");
                }
            }
        }
        private ConnectionState mConnectionState;

        public  List<LayerGame> Clients
        {
            get { return mClients; }
            private set {
                if (mClients != value) {
                    mClients = value;
                    OnPropertyChanged(this, "Clients");
                }
            }
        }
        private List<LayerGame> mClients;

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

        // Private Variables
        private TcpListener mTcpListener;



        /// <summary>
        /// Initializes an instance of the LayerListener class.
        /// </summary>
        public LayerListener() {
            Clients = new List<LayerGame>();
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
        /// Begins listening for clients to connect to the layer and sets the connection state to Ready.
        /// </summary>
        public void Begin()
        {
            try
            {
                IPAddress ipBinding = ResolveHostName(Hostname);

                mTcpListener = new TcpListener(ipBinding, Port);
                mTcpListener.Start();
                mTcpListener.BeginAcceptTcpClient(AcceptClient, null);

                ConnectionState = Net.ConnectionState.Ready;
            }
            catch (SocketException se) { Shutdown(se); }
            catch (Exception e)        { Shutdown(e); }
        }

        /// <summary>
        /// Stops listening for clients connecting to the layer and sets the connection state to Disconnected.
        /// </summary>
        public void Shutdown()
        {
            if (mTcpListener != null)
                try
                {
                    foreach (LayerGame client in Clients)
                        client.Shutdown();

                    this.mTcpListener.Stop();
                    this.mTcpListener = null;
                }
                catch (Exception) { }
            ConnectionState = Net.ConnectionState.Disconnected;
        }

        /// <summary>
        /// Sends a request to any connected clients, if the layer is a LayerListener.
        /// Sends a request to the connected interface, if the layer is a LayerGame.
        /// </summary>
        public void Request(Context context, CommandName command, EventName @event, params Object[] arguments) {
            this.Broadcast(
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
        public Context ServerContext(String hostname, UInt16 port) {
            return new Context() {
                ContextType = ContextType.Connection,
                Hostname    = hostname,
                Port        = port
            };
        }

        #endregion
        #region Events

        // -- When Processing a Layer Event --
        public event LayerGame.ProcessLayerEventHandler ProcessLayerEvent;
        protected void OnProcessingLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters)
        {
            if (ProcessLayerEvent != null)
                ProcessLayerEvent(username, context, command, @event, parameters);
        }

        // -- When a Socket Exception Occurs --
        public delegate void SocketExceptionHandler(LayerListener sender, SocketException se);
        public event         SocketExceptionHandler SocketException;
        protected void OnSocketException(LayerListener sender, SocketException se)
        {
            if (SocketException != null)
                SocketException(sender, se);
        }

        // -- When a Connection Failure Occurs --
        public delegate void FailureHandler(LayerListener sender, Exception e);
        public event         FailureHandler ConnectionFailure;
        protected void OnConnectionFailure(LayerListener sender, Exception e)
        {
            if (ConnectionFailure != null)
                ConnectionFailure(sender, e);
        }

        // -- Adding a Client --
        public delegate void ClientAddedHandler(LayerListener parent, LayerGame item);
        public event         ClientAddedHandler ClientAdded;
        protected void OnClientAdded(LayerListener parent, LayerGame item)
        {
            if (ClientAdded != null)
                ClientAdded(parent, item);
        }

        // -- Removing a Client --
        public delegate void ClientRemovedHandler(LayerListener parent, LayerGame item);
        public event         ClientRemovedHandler ClientRemoved;
        protected void OnClientRemoved(LayerListener parent, LayerGame item)
        {
            if (ClientRemoved != null)
                ClientRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Attempts to resolve the hostname by first trying the hostname as an IP, then trying to
        /// resolve the hostname using the built in DNS resolution.  Returns no IP if failed.
        /// </summary>
        private IPAddress ResolveHostName(String hostName)
        {
            IPAddress ip = IPAddress.None;
            if (!IPAddress.TryParse(hostName, out ip))
                try
                {
                    IPHostEntry entry = Dns.GetHostEntry(hostName);
                    if (entry.AddressList.Length > 0)
                        ip = entry.AddressList[0];
                }
                catch (Exception) { }
            return ip;
        }

        /// <summary>
        /// Accepts an incomming connection as a layer client then returns to accepting new clients.
        /// </summary>
        private void AcceptClient(IAsyncResult ar)
        {
            try
            {
                LayerGame newClient = new LayerGame(new LayerClient(mTcpListener.EndAcceptTcpClient(ar))) {
                    Security     = Security,
                    IsCompressed = IsCompressed,
                    IsEncrypted  = IsEncrypted
                };
                newClient.ProcessLayerEvent += OnProcessingLayerEvent;
                Clients.Add(newClient);
                OnClientAdded(this, newClient);
                
                mTcpListener.BeginAcceptTcpClient(AcceptClient, this);
            }
            catch (SocketException se) { Shutdown(se); }
            catch (Exception e)        { Shutdown(e); }
        }

        /// <summary>
        /// Closes the connection, firing the SocketException event on it's way out.
        /// </summary>
        private void Shutdown(SocketException se)
        {
            Shutdown();
            OnSocketException(this, se);
        }

        /// <summary>
        /// Closes the connection, firing the ConnectionFailure event on it's way out.
        /// </summary>
        private void Shutdown(Exception e) {
            Shutdown();
            OnConnectionFailure(this, e);
        }

        /// <summary>
        /// Sends the packet specified to all connected clients, provided they have authenticated
        /// </summary>
        private void Broadcast(LayerPacket packet)
        {
            foreach (LayerGame client in Clients.Where(x => x.Client.ConnectionState == ConnectionState.LoggedIn))
                client.Send(packet);
        }
    }
}
