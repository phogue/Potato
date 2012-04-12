using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Protocols.Source.Logging.BroadcastListener {

    public class SourceBroadcastListener : UDPClient<SourceBroadcastListenerPacket> {

        public ushort SourceLogServicePort { get; set; }
        public ushort SourceLogListenPort { get; set; }

        public SourceBroadcastListener(ushort port)
            : base("", port) {
                this.PacketSerializer = new SourceBroadcastListenerPacketSerializer();
        }

        // Override and do *not* call base AttemptConnection 
        public override void AttemptConnection() {
            try {
                this.ConnectionState = Net.ConnectionState.Connecting;
                this.m_remoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.Port);

                this.m_udpClient = new UdpClient();
                //this.m_udpClient.DontFragment = true; // ?
                this.m_udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.m_udpClient.ExclusiveAddressUse = false;
                this.m_udpClient.Client.Bind(this.m_remoteIpEndPoint);

                this.m_udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, true);
                this.m_udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.10.10.10")));

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

        protected override void ReceiveCallback(IAsyncResult ar) {

            try {
                this.a_bReceivedBuffer = this.m_udpClient.EndReceive(ar, ref this.m_remoteIpEndPoint);

                SourceBroadcastListenerPacket completedPacket = this.PacketSerializer.Deserialize(this.a_bReceivedBuffer);
                // completedPacket.RemoteEndPoint = this.m_remoteIpEndPoint;

                bool isProcessed = false;
                this.OnBeforePacketDispatch(completedPacket, out isProcessed);

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
