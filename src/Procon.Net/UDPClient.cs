using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    [Serializable]
    public class UdpClient : Client {

        /// <summary>
        /// The 'open' client for the udp connection
        /// </summary>
        public System.Net.Sockets.UdpClient Client;

        /// <summary>
        /// The end point of the server we are communicating with
        /// </summary>
        public IPEndPoint RemoteIpEndPoint;

        public UdpClient(string hostname, UInt16 port) : base(hostname, port) {

        }

        protected virtual void ClearConnection() {
            this.ReceivedBuffer = new byte[this.BufferSize];
        }

        public override void Connect() {
            if (this.MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                this.MarkManager.Mark();

                try {
                    this.ConnectionState = Net.ConnectionState.ConnectionConnecting;

                    this.Client = new System.Net.Sockets.UdpClient(this.Hostname, this.Port) {
                        DontFragment = true
                    };

                    this.RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    this.Client.BeginReceive(this.ReadCallback, null);

                    this.ConnectionState = Net.ConnectionState.ConnectionReady;
                }
                catch (SocketException se) {
                    this.Shutdown(se);
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
        }

        public override IAsyncResult BeginRead() {
            return this.Client != null ? this.Client.BeginReceive(this.ReadCallback, null) : null;
        }

        protected virtual void SendAsynchronousCallback(IAsyncResult ar) {

            IPacketWrapper wrapper = (IPacketWrapper)ar.AsyncState;

            try {
                if (this.Client != null) {
                    this.Client.EndSend(ar);

                    this.OnPacketSent(wrapper);
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        public override void Send(IPacketWrapper wrapper) {
            if (wrapper != null) {
                if (this.BeforePacketSend(wrapper) == false && this.Client != null) {

                    byte[] bytePacket = this.PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        this.Client.BeginSend(bytePacket, bytePacket.Length, this.RemoteEndPoint, this.SendAsynchronousCallback, wrapper);
                    }
                }
            }
        }

        protected virtual void ReadCallback(IAsyncResult ar) {

            try {
                if (this.Client != null) {

                    this.ReceivedBuffer = this.Client.EndReceive(ar, ref this.RemoteIpEndPoint);

                    IPacketWrapper completedPacket = this.PacketSerializer.Deserialize(this.ReceivedBuffer);

                    if (completedPacket != null) {
                        this.RemoteEndPoint = completedPacket.Packet.RemoteEndPoint = this.RemoteIpEndPoint;

                        this.BeforePacketDispatch(completedPacket);

                        this.OnPacketReceived(completedPacket);
                    }

                    this.BeginRead();
                }
                else {
                    this.Shutdown(new Exception("No stream exists during receive"));
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        public override void Shutdown(Exception e) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnConnectionFailure(e);
            }
        }

        public override void Shutdown(SocketException se) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnSocketException(se);
            }
        }

        public override void Shutdown() {
            if (this.Client != null) {
                this.ShutdownConnection();
            }
        }

        protected override void ShutdownConnection() {

            lock (this.ShutdownConnectionLock) {

                this.ConnectionState = Net.ConnectionState.ConnectionDisconnecting;

                if (this.Client != null) {

                    try {
                        if (this.Client != null) {
                            this.Client.Close();
                            this.Client = null;
                        }
                    }
                    catch (SocketException se) {
                        this.OnSocketException(se);
                    }
                    catch (Exception) {

                    }
                    finally {
                        this.ConnectionState = Net.ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing connected, set to disconnected.
                    this.ConnectionState = Net.ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
