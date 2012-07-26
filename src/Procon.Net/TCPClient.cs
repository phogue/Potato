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

    public abstract class TCPClient<P> : Client<P>
        where P : Packet {

        protected TcpClient m_tcpClient;
        protected NetworkStream m_nwsStream;
        
        //public string Hostname { get; private set; }
        //public UInt16 Port { get; private set; }

        private UInt32 m_sequenceNumber;
        public UInt32 AcquireSequenceNumber {
            get {
                lock (new object()) {
                    return ++this.m_sequenceNumber;
                }
            }
        }

        public TCPClient(string hostname, UInt16 port) : base(hostname, port) {
            this.ClearConnection();
        }

        protected virtual void ClearConnection() {
            this.a_bReceivedBuffer = new byte[TCPClient<P>.BUFFER_SIZE];
            this.a_bPacketStream = null;

            this.m_sequenceNumber = 0;
        }

        #region Send/Recieve Packets

        private void SendAsyncCallback(IAsyncResult ar) {

            P packet = (P)ar.AsyncState;

            try {
                if (this.m_nwsStream != null) {
                    this.m_nwsStream.EndWrite(ar);

                    this.OnPacketSent(packet);

                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        // Send straight away ignoring the queue
        public override void Send(P packet) {

            if (packet != null) {
                try {
                    bool isProcessed = false;

                    this.OnBeforePacketSend(packet, out isProcessed);

                    if (isProcessed == false && this.m_nwsStream != null) {

                        byte[] a_bBytePacket = this.PacketSerializer.Serialize(packet);

                        this.m_nwsStream.BeginWrite(a_bBytePacket, 0, a_bBytePacket.Length, this.SendAsyncCallback, packet);
                    }
                }
                catch (SocketException se) {
                    this.Shutdown(se);
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
        }
        
        private void ReceiveCallback(IAsyncResult ar) {

            try {
                if (this.m_nwsStream != null) {
                    int iBytesRead = this.m_nwsStream.EndRead(ar);

                    if (iBytesRead > 0) {

                        // Create or resize our packet stream to hold the new data.
                        if (this.a_bPacketStream == null) {
                            this.a_bPacketStream = new byte[iBytesRead];
                        }
                        else {
                            Array.Resize(ref this.a_bPacketStream, this.a_bPacketStream.Length + iBytesRead);
                        }

                        Array.Copy(this.a_bReceivedBuffer, 0, this.a_bPacketStream, this.a_bPacketStream.Length - iBytesRead, iBytesRead);

                        UInt32 ui32PacketSize = this.PacketSerializer.ReadPacketSize(this.a_bPacketStream);

                        while (this.a_bPacketStream.Length >= ui32PacketSize && this.a_bPacketStream.Length > this.PacketSerializer.PacketHeaderSize) {

                            // Copy the complete packet from the beginning of the stream.
                            byte[] a_bCompletePacket = new byte[ui32PacketSize];
                            Array.Copy(this.a_bPacketStream, a_bCompletePacket, ui32PacketSize);

                            P completedPacket = this.PacketSerializer.Deserialize(a_bCompletePacket);
                            //Packet completedPacket = new Packet(a_bCompletePacket);
                            //cbfConnection.m_ui32SequenceNumber = Math.Max(cbfConnection.m_ui32SequenceNumber, cpCompletePacket.SequenceNumber) + 1;

                            // Dispatch the completed packet.
                            try {
                                bool isProcessed = false;

                                this.OnBeforePacketDispatch(completedPacket, out isProcessed);

                                this.OnPacketReceived(completedPacket);

                                // Now remove the completed packet from the beginning of the stream
                                byte[] a_bUpdatedSteam = new byte[this.a_bPacketStream.Length - ui32PacketSize];
                                Array.Copy(this.a_bPacketStream, ui32PacketSize, a_bUpdatedSteam, 0, this.a_bPacketStream.Length - ui32PacketSize);
                                this.a_bPacketStream = a_bUpdatedSteam;

                                ui32PacketSize = this.PacketSerializer.ReadPacketSize(this.a_bPacketStream); // this.DecodePacketSize(this.a_bPacketStream);
                            }
                            catch (Exception) {
                                // TO DO: May be bad, who knows now.
                                this.ClearConnection();
                            }
                        }

                        // If we've recieved the maxmimum garbage, scrap it all and shutdown the connection.
                        // We went really wrong somewhere..
                        if (this.a_bReceivedBuffer.Length >= Client<P>.MAX_GARBAGE_BYTES) {
                            this.a_bReceivedBuffer = null; // GC.collect()
                            this.Shutdown(new Exception("Exceeded maximum garbage packet"));
                        }
                    }

                    if (iBytesRead == 0) {
                        this.Shutdown();
                        return;
                    }

                    if (this.m_nwsStream != null) {

                        IAsyncResult result = this.m_nwsStream.BeginRead(this.a_bReceivedBuffer, 0, this.a_bReceivedBuffer.Length, this.ReceiveCallback, this);

                        //if (result.AsyncWaitHandle.WaitOne(180000, false) == false) {
                        //    this.Shutdown(new Exception("Failed to recieve after two minutes"));
                        //}
                    }
                }
                else {
                    this.Shutdown(new Exception("No stream exists during receieve"));
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        #endregion

        #region Connecting

        protected override IAsyncResult BeginRead() {
            if (this.m_nwsStream != null) {
                return this.m_nwsStream.BeginRead(this.a_bReceivedBuffer, 0, this.a_bReceivedBuffer.Length, this.ReceiveCallback, this);
            }

            return null;
        }

        private void ConnectedCallback(IAsyncResult ar) {

            try {
                this.m_tcpClient.EndConnect(ar);
                this.m_tcpClient.NoDelay = true;

                this.ConnectionState = Net.ConnectionState.Connected;
                this.LocalEndPoint = (IPEndPoint)this.m_tcpClient.Client.LocalEndPoint;
                this.RemoteEndPoint = (IPEndPoint)this.m_tcpClient.Client.RemoteEndPoint;


                this.m_nwsStream = this.m_tcpClient.GetStream();
                this.BeginRead();
                //this.m_nwsStream.BeginRead(this.a_bReceivedBuffer, 0, this.a_bReceivedBuffer.Length, this.ReceiveCallback, this);

                this.ConnectionState = Net.ConnectionState.Ready;
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        public override void AttemptConnection() {
            if (this.Hostname != null && this.Port != 0) {
                try {
                    this.ClearConnection();

                    this.ConnectionState = Net.ConnectionState.Connecting;

                    this.m_tcpClient = new TcpClient();
                    this.m_tcpClient.NoDelay = true;

                    this.m_tcpClient.BeginConnect(this.Hostname, this.Port, this.ConnectedCallback, this);
                }
                catch (SocketException se) {
                    this.Shutdown(se);
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
        }

        #endregion

        #region Shutdown/Disconnection

        public void Shutdown(Exception e) {
            if (this.m_tcpClient != null) {
                this.ShutdownConnection();

                this.OnConnectionFailure(e);
            }
        }

        public void Shutdown(SocketException se) {
            if (this.m_tcpClient != null) {
                this.ShutdownConnection();

                this.OnSocketException(se);
            }
        }

        public override void Shutdown() {
            if (this.m_tcpClient != null) {
                this.ShutdownConnection();
            }
        }

        private void ShutdownConnection() {

            lock (new object()) {

                this.ConnectionState = Net.ConnectionState.Disconnecting;

                if (this.m_tcpClient != null) {

                    try {

                        this.ClearConnection();

                        if (this.m_nwsStream != null) {
                            this.m_nwsStream.Close();
                            this.m_nwsStream.Dispose();
                            this.m_nwsStream = null;
                        }

                        if (this.m_tcpClient != null) {
                            this.m_tcpClient.Close();
                            this.m_tcpClient = null;
                        }
                    }
                    catch (SocketException se) {
                        this.OnSocketException(se);
                    }
                    catch (Exception) {
                    
                    }
                    finally {
                        this.ConnectionState = Net.ConnectionState.Disconnected;
                    }
                }
            }
        }

        #endregion

    }
}
