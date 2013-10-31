using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Protocols.Source.Logging.BroadcastService {

    public class SourceBroadcastService : UdpClient<SourceBroadcastServicePacket> {

        public UdpClient BroadcastClient { get; set; }
        public ushort BroadcastPort { get; set; }
        public IPEndPoint BroadcastEndpoint { get; set; }

        protected Socket BroadcastSocket { get; set; }

        public SourceBroadcastService(ushort listenPort, ushort broadcastPort)
            : base("", listenPort) {
                this.PacketSerializer = new SourceBroadcastServicePacketSerializer();
                this.BroadcastPort = broadcastPort;
        }

        // Override and do *not* call base AttemptConnection 
        public override void Connect() {

            // Setup broadcaster to pass the packets on as multicast
            try {
                this.BroadcastEndpoint = new IPEndPoint(IPAddress.Parse("224.10.10.10"), this.BroadcastPort);

                this.BroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                this.BroadcastSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, true);
                this.BroadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.BroadcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, true);
                this.BroadcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.10.10.10")));
                
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }

            // Setup listener to accept source logging packets
            try {
                this.ConnectionState = Net.ConnectionState.ConnectionConnecting;
                this.RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.Port);

                this.Client = new UdpClient(this.RemoteIpEndPoint);
                this.Client.DontFragment = true; // ?

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

        protected override void OnPacketReceived(SourceBroadcastServicePacket packet) {
            base.OnPacketReceived(packet);

            // We've got a packet, now broadcast it..
            
            byte[] serializedPacket = this.PacketSerializer.Serialize(packet);

            this.BroadcastSocket.BeginSendTo(serializedPacket, 0, serializedPacket.Length, SocketFlags.None, this.BroadcastEndpoint, this.SendAsynchronousCallback, null);
        }

        protected override void SendAsynchronousCallback(IAsyncResult ar) {
            SourceBroadcastServicePacket packet = (SourceBroadcastServicePacket)ar.AsyncState;

            try {
                if (this.BroadcastSocket != null) {
                    this.BroadcastSocket.EndSendTo(ar);

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


    }
}
