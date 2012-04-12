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
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public class UDPClient<P> : Client<P>
        where P : Packet {

        protected UdpClient m_udpClient;
        protected IPEndPoint m_remoteIpEndPoint;

        private byte[] a_bReceivedBuffer = new byte[Client<P>.BUFFER_SIZE];
        //private byte[] a_bPacketStream;

        public UDPClient(string hostname, UInt16 port)
            : base(hostname, port) {
        }

        public override void AttemptConnection() {
            try {
                this.ConnectionState = Net.ConnectionState.Connecting;

                this.m_udpClient = new UdpClient(this.Hostname, this.Port);
                this.m_udpClient.DontFragment = true; // ?
                this.m_remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                this.m_udpClient.BeginReceive(this.ReceiveCallback, null);

                this.ConnectionState = Net.ConnectionState.Ready;
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        protected override IAsyncResult BeginRead() {
            if (this.m_udpClient != null) {
                return this.m_udpClient.BeginReceive(this.ReceiveCallback, null);
            }

            return null;
        }

        #region Send/Recieve Packets

        protected virtual void SendAsyncCallback(IAsyncResult ar) {

            P packet = (P)ar.AsyncState;

            try {
                if (this.m_udpClient != null) {
                    this.m_udpClient.EndSend(ar);

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

        public override void Send(P packet) {
            if (packet != null) {
                try {

                    bool isProcessed = false;

                    this.OnBeforePacketSend(packet, out isProcessed);

                    if (isProcessed == false && this.m_udpClient != null) {

                        byte[] a_bBytePacket = this.PacketSerializer.Serialize(packet);

                        this.m_udpClient.BeginSend(a_bBytePacket, a_bBytePacket.Length, this.SendAsyncCallback, packet);
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

        protected virtual void ReceiveCallback(IAsyncResult ar) {

            try {
                if (this.m_udpClient != null) {

                    this.a_bReceivedBuffer = this.m_udpClient.EndReceive(ar, ref this.m_remoteIpEndPoint);

                    P completedPacket = this.PacketSerializer.Deserialize(this.a_bReceivedBuffer);
                    this.RemoteEndPoint = completedPacket.RemoteEndPoint = this.m_remoteIpEndPoint;

                    bool isProcessed = false;
                    this.OnBeforePacketDispatch(completedPacket, out isProcessed);

                    this.OnPacketReceived(completedPacket);

                    this.BeginRead();

                    //if (this.m_udpClient != null) {
                    //    IAsyncResult result = this.m_udpClient.BeginReceive(this.ReceiveCallback, null);
                    //}
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

            /*
                int iBytesRead = this.a_bReceivedBuffer.Length;

                if (iBytesRead > 0) {

                    // Create or resize our packet stream to hold the new data.
                    if (this.a_bPacketStream == null) {
                        this.a_bPacketStream = new byte[iBytesRead];
                    }
                    else {
                        Array.Resize(ref this.a_bPacketStream, this.a_bPacketStream.Length + iBytesRead);
                    }

                    Array.Copy(this.a_bReceivedBuffer, 0, this.a_bPacketStream, this.a_bPacketStream.Length - iBytesRead, iBytesRead);

                    UInt32 ui32PacketSize = this.DecodePacketSize(this.a_bPacketStream);

                    while (this.a_bPacketStream.Length >= ui32PacketSize && this.a_bPacketStream.Length > this.GetPacketHeaderSize()) {

                        // Copy the complete packet from the beginning of the stream.
                        byte[] a_bCompletePacket = new byte[ui32PacketSize];
                        Array.Copy(this.a_bPacketStream, a_bCompletePacket, ui32PacketSize);

                        Packet completedPacket = this.CreatePacket(a_bCompletePacket);
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

                            ui32PacketSize = this.DecodePacketSize(this.a_bPacketStream);
                        }
                        catch (Exception e) {
                            // TO DO: May be bad, who knows now.
                            // this.ClearConnection();
                        }
                    }

                    // If we've recieved the maxmimum garbage, scrap it all and shutdown the connection.
                    // We went really wrong somewhere..
                    if (this.a_bReceivedBuffer.Length >= Client.MAX_GARBAGE_BYTES) {
                        this.a_bReceivedBuffer = null; // GC.collect()
                        this.Shutdown(new Exception("Exceeded maximum garbage packet"));
                    }
                }

                if (iBytesRead == 0) {
                    this.Shutdown();
                    return;
                }


                if (this.m_udpClient != null) {

                    IAsyncResult result = this.m_udpClient.BeginReceive(this.ReceiveCallback, null);
                    //IAsyncResult result = this.m_nwsStream.BeginRead(this.a_bReceivedBuffer, 0, this.a_bReceivedBuffer.Length, this.ReceiveCallback, this);

                    //if (result.AsyncWaitHandle.WaitOne(180000, false) == false) {
                    //    this.Shutdown(new Exception("Failed to recieve after two minutes"));
                    //}
                }

            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
            */
        }

        #endregion

        #region Shutdown/Disconnection

        public void Shutdown(Exception e) {
            if (this.m_udpClient != null) {
                this.ShutdownConnection();

                this.OnConnectionFailure(e);
            }
        }

        public void Shutdown(SocketException se) {
            if (this.m_udpClient != null) {
                this.ShutdownConnection();

                this.OnSocketException(se);
            }
        }

        public override void Shutdown() {
            if (this.m_udpClient != null) {
                this.ShutdownConnection();
            }
        }

        private void ShutdownConnection() {

            lock (new object()) {

                this.ConnectionState = Net.ConnectionState.Disconnecting;

                if (this.m_udpClient != null) {

                    try {
                        if (this.m_udpClient != null) {
                            this.m_udpClient.Close();
                            this.m_udpClient = null;
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
