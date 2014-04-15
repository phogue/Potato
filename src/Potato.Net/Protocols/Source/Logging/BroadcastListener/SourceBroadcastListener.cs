using System;
using System.Net;
using System.Net.Sockets;

namespace Potato.Net.Protocols.Source.Logging.BroadcastListener {

    public class SourceBroadcastListener : Potato.Net.UdpClient {

        public ushort SourceLogServicePort { get; set; }
        public ushort SourceLogListenPort { get; set; }

        public SourceBroadcastListener(ushort port)
            : base("", port) {
                this.PacketSerializer = new SourceBroadcastListenerPacketSerializer();
        }

        // Override and do *not* call base AttemptConnection 
        public override void Connect() {
            try {
                this.ConnectionState = Net.ConnectionState.ConnectionConnecting;
                this.RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.Port);

                this.Client = new System.Net.Sockets.UdpClient();
                //this.m_udpClient.DontFragment = true; // ?
                this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.Client.ExclusiveAddressUse = false;
                this.Client.Client.Bind(this.RemoteIpEndPoint);

                this.Client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, true);
                this.Client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.10.10.10")));

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

        protected override void ReadCallback(IAsyncResult ar) {

            try {
                this.ReceivedBuffer = this.Client.EndReceive(ar, ref this.RemoteIpEndPoint);

                SourceBroadcastListenerPacket completedPacket = this.PacketSerializer.Deserialize(this.ReceivedBuffer) as SourceBroadcastListenerPacket;
                // completedPacket.RemoteEndPoint = this.m_remoteIpEndPoint;

                this.BeforePacketDispatch(completedPacket);
                
                this.OnPacketReceived(completedPacket);

                this.BeginRead();
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

    }
}
