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

using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public abstract class Client<P>
        where P : Packet {

        public String Hostname { get; protected set; }
        public UInt16 Port { get; protected set; }

        public IPEndPoint LocalEndPoint { get; protected set; }
        public IPEndPoint RemoteEndPoint { get; protected set; }

        protected PacketSerializer<P> PacketSerializer { get; set; }

        // Maximum amount of data to accept before scrapping the whole lot and trying again.
        // Test maximizing this to see if plugin descriptions are causing some problems.
        protected static readonly UInt32 MAX_GARBAGE_BYTES = 4194304;
        protected static readonly UInt16 BUFFER_SIZE = 16384;

        protected byte[] a_bReceivedBuffer;
        protected byte[] a_bPacketStream;

        #region Events

        public delegate void PrePacketDispatchedHandler(Client<P> sender, P packet, out bool isProcessed);
        public event PrePacketDispatchedHandler BeforePacketDispatch;
        public event PrePacketDispatchedHandler BeforePacketSend;

        public delegate void PacketDispatchHandler(Client<P> sender, P packet);
        public event PacketDispatchHandler PacketSent;
        public event PacketDispatchHandler PacketReceived;

        public delegate void SocketExceptionHandler(Client<P> sender, SocketException se);
        public event SocketExceptionHandler SocketException;

        public delegate void FailureHandler(Client<P> sender, Exception exception);
        public event FailureHandler ConnectionFailure;

        public delegate void ConnectionStateChangedHandler(Client<P> sender, ConnectionState newState);
        public event ConnectionStateChangedHandler ConnectionStateChanged;

        #endregion

        private ConnectionState m_connectionState;
        public ConnectionState ConnectionState {
            get {
                return this.m_connectionState;
            }
            set {
                this.m_connectionState = value;

                if (this.ConnectionStateChanged != null) {
                    this.ConnectionStateChanged(this, this.m_connectionState);
                }
            }
        }

        public Client(string hostname, UInt16 port) {
            this.Hostname = hostname;
            this.Port = port;
        }

        /*
        #region Packet Management

        protected virtual P CreatePacket(byte[] packet) {
            return null;
        }

        protected virtual UInt32 DecodePacketSize(byte[] packet) {
            return 0;
        }

        protected virtual UInt32 GetPacketHeaderSize() {
            return 0;
        }

        #endregion
        */
        public virtual void AttemptConnection() {

        }

        public virtual void Shutdown() {

        }

        public virtual void Send(P packet) {

        }

        protected virtual IAsyncResult BeginRead() {
            return null;
        }

        #region Pre Dispatch

        protected virtual void OnBeforePacketDispatch(P packet, out bool isProcessed) {
            if (this.BeforePacketDispatch != null) {
                this.BeforePacketDispatch(this, packet, out isProcessed);
            }
            else {
                isProcessed = false;
            }
        }

        protected virtual void OnBeforePacketSend(P packet, out bool isProcessed) {
            if (this.BeforePacketSend != null) {
                this.BeforePacketSend(this, packet, out isProcessed);
            }
            else {
                isProcessed = false;
            }
        }

        #endregion

        #region Dispatch

        protected virtual void OnPacketSent(P packet) {

            if (this.PacketSent != null) {
                this.PacketSent(this, packet);
            }
        }

        protected virtual void OnPacketReceived(P packet) {

            if (this.PacketReceived != null) {
                this.PacketReceived(this, packet);
            }
        }

        #endregion

        #region Connection

        public static IPAddress ResolveHostName(string hostName) {
            IPAddress ipReturn = IPAddress.None;

            if (IPAddress.TryParse(hostName, out ipReturn) == false) {

                ipReturn = IPAddress.None;

                try {
                    IPHostEntry iphHost = Dns.GetHostEntry(hostName);

                    if (iphHost.AddressList.Length > 0) {
                        ipReturn = iphHost.AddressList[0];
                    }
                    // ELSE return IPAddress.None..
                }
                catch (Exception) { } // Returns IPAddress.None..
            }

            return ipReturn;
        }

        protected virtual void OnConnectionFailure(Exception e) {
            if (this.ConnectionFailure != null) {
                this.ConnectionFailure(this, e);
            }
        }

        protected virtual void OnSocketException(SocketException se) {
            if (this.SocketException != null) {
                this.SocketException(this, se);
            }
        }

        #endregion

        #region Event Firing

        protected void FirePacketRecieved(P packet) {
            if (this.PacketReceived != null) {
                this.PacketReceived(this, packet);
            }
        }

        #endregion

    }
}
