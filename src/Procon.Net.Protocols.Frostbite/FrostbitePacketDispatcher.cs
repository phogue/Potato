using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite {
    public class FrostbitePacketDispatcher : PacketDispatcher {

        /// <summary>
        /// Handles a queue of packets, used to find the request packet.
        /// </summary>
        public IPacketQueue PacketQueue { get; set; }

        public override void Dispatch(Packet packet) {

            if (packet.Origin == PacketOrigin.Client && packet.Type == PacketType.Response) {
                FrostbitePacket requestPacket = this.PacketQueue.GetRequestPacket(packet) as FrostbitePacket;

                // If the request packet is valid and has at least one word.
                if (requestPacket != null && requestPacket.Words.Count >= 1) {

                    // If the sent command was successful
                    if (packet.Words.Count >= 1 && String.CompareOrdinal(packet.Words[0], FrostbitePacket.StringResponseOkay) == 0) {
                        this.Dispatch(new PacketDispatch() {
                            Name = requestPacket.Words[0],
                            Origin = requestPacket.Origin
                        }, requestPacket, packet);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(new PacketDispatch() {
                            Name = packet.Words[0],
                            Origin = packet.Origin
                        }, requestPacket, packet);
                    }
                }

            }
            else if (packet.Words.Count >= 1 && packet.Origin == PacketOrigin.Server && packet.Type == PacketType.Request) {
                this.Dispatch(new PacketDispatch() {
                    Name = packet.Words[0],
                    Origin = packet.Origin
                }, packet, null);
            }
        }
    }
}
