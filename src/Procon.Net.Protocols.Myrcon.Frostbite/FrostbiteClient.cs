using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Myrcon.Frostbite {
    public class FrostbiteClient : Procon.Net.TcpClient {

        /// <summary>
        /// Queue of packets
        /// </summary>
        public IPacketQueue PacketQueue { get; set; }

        public FrostbiteClient(string hostname, ushort port) : base(hostname, port) {

            this.PacketQueue = new PacketQueue();

            this.PacketSerializer = new FrostbitePacketSerializer();
        }

        protected override void OnPacketReceived(IPacketWrapper wrapper) {
            base.OnPacketReceived(wrapper);

            // Respond with "OK" to all server events.
            if (wrapper.Packet.Origin == PacketOrigin.Server && wrapper.Packet.Type == PacketType.Request) {
                base.Send(new FrostbitePacket() {
                    Packet = {
                        Origin = PacketOrigin.Server,
                        Type = PacketType.Response,
                        RequestId = wrapper.Packet.RequestId,
                        Words = new List<String>() {
                            FrostbitePacket.StringResponseOkay
                        }
                    }
                });
            }

            // Pop the next packet if a packet is waiting to be sent.
            IPacketWrapper poppedWrapper = null;
            if ((poppedWrapper = this.PacketQueue.PacketReceived(wrapper)) != null) {
                this.Send(poppedWrapper);
            }
            
            // Shutdown if we're just waiting for a response to an old packet.
            if (this.PacketQueue.RestartConnectionOnQueueFailure() == true) {
                this.Shutdown(new Exception("Failed to hear response to packet within two minutes, forced shutdown."));
            }
        }

        public override IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper.Packet.RequestId == null) {
                wrapper.Packet.RequestId = this.AcquireSequenceNumber;
            }

            // QueueUnqueuePacket

            if (wrapper.Packet.Origin == PacketOrigin.Server && wrapper.Packet.Type == PacketType.Response) {
                // I don't think this will ever be encountered since OnPacketReceived calls the base.Send.
                sent = base.Send(wrapper);
            }
            else {
                // Null return because we're not popping a packet, just checking to see if this one needs to be queued.
                IPacketWrapper poppedWrapper = null;
                if ((poppedWrapper = this.PacketQueue.PacketSend(wrapper)) != null) {
                    sent = base.Send(poppedWrapper);
                }

                // Shutdown if we're just waiting for a response to an old packet.
                if (this.PacketQueue.RestartConnectionOnQueueFailure() == true) {
                    this.Shutdown(new Exception("Failed to hear response to packet within two minutes, forced shutdown."));
                }
            }

            return sent;
        }

        protected override void ShutdownConnection() {
            base.ShutdownConnection();

            this.PacketQueue.Clear();
        }
    }
}
