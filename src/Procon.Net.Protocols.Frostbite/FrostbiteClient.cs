using System;
using System.Linq;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite {
    public class FrostbiteClient : Procon.Net.TcpClient {

        /// <summary>
        /// Queue of packets
        /// </summary>
        public PacketQueue PacketQueue { get; set; }

        public FrostbiteClient(string hostname, ushort port) : base(hostname, port) {

            this.PacketQueue = new PacketQueue();

            this.PacketSerializer = new FrostbitePacketSerializer();
        }

        protected override void OnPacketReceived(Packet packet) {
            base.OnPacketReceived(packet);

            // Respond with "OK" to all server events.
            if (packet.Origin == PacketOrigin.Server && packet.Type == PacketType.Request) {
                base.Send(new FrostbitePacket(PacketOrigin.Server, PacketType.Response, packet.RequestId, FrostbitePacket.StringResponseOkay));
            }

            // Pop the next packet if a packet is waiting to be sent.
            Packet poppedPacket = null;
            if ((poppedPacket = this.PacketQueue.PacketReceived(packet)) != null) {
                this.Send(poppedPacket);
            }
            
            // Shutdown if we're just waiting for a response to an old packet.
            if (this.PacketQueue.RestartConnectionOnQueueFailure() == true) {
                this.Shutdown(new Exception("Failed to hear response to packet within two minutes, forced shutdown."));
            }
        }

        public override void Send(Packet packet) {

            if (packet.RequestId == null) {
                packet.RequestId = this.AcquireSequenceNumber;
            }

            // QueueUnqueuePacket

            if (packet.Origin == PacketOrigin.Server && packet.Type == PacketType.Response) {
                // I don't think this will ever be encountered since OnPacketReceived calls the base.Send.
                base.Send(packet);
            }
            else {
                // Null return because we're not popping a packet, just checking to see if this one needs to be queued.
                Packet poppedPacket = null;
                if ((poppedPacket = this.PacketQueue.PacketSend(packet)) != null) {
                    base.Send(poppedPacket);
                }

                // Shutdown if we're just waiting for a response to an old packet.
                if (this.PacketQueue.RestartConnectionOnQueueFailure() == true) {
                    this.Shutdown(new Exception("Failed to hear response to packet within two minutes, forced shutdown."));
                }
            }
        }

        protected override bool BeforePacketSend(Packet packet) {
            this.PacketQueue.BeforePacketSend(packet);

            return base.BeforePacketSend(packet);
        }

        protected override void ShutdownConnection() {
            base.ShutdownConnection();

            this.PacketQueue.Clear();
        }
    }
}
