using System;
using Procon.Net.Shared;

namespace Procon.Net.Protocols.Myrcon.Frostbite {
    public class FrostbitePacketDispatcher : PacketDispatcher {

        /// <summary>
        /// Handles a queue of packets, used to find the request packet.
        /// </summary>
        public IPacketQueue PacketQueue { get; set; }

        public override void Dispatch(IPacketWrapper wrapper) {

            if (wrapper.Packet.Origin == PacketOrigin.Client && wrapper.Packet.Type == PacketType.Response) {
                FrostbitePacket requestPacket = this.PacketQueue.GetRequestPacket(wrapper) as FrostbitePacket;

                // If the request packet is valid and has at least one word.
                if (requestPacket != null && requestPacket.Packet.Words.Count >= 1) {

                    // If the sent command was successful
                    if (wrapper.Packet.Words.Count >= 1 && String.CompareOrdinal(wrapper.Packet.Words[0], FrostbitePacket.StringResponseOkay) == 0) {
                        this.Dispatch(new PacketDispatch() {
                            Name = requestPacket.Packet.Words[0],
                            Origin = requestPacket.Packet.Origin
                        }, requestPacket, wrapper);
                    }
                    else { // The command sent failed for some reason.
                        this.Dispatch(new PacketDispatch() {
                            Name = wrapper.Packet.Words[0],
                            Origin = wrapper.Packet.Origin
                        }, requestPacket, wrapper);
                    }
                }

            }
            else if (wrapper.Packet.Words.Count >= 1 && wrapper.Packet.Origin == PacketOrigin.Server && wrapper.Packet.Type == PacketType.Request) {
                this.Dispatch(new PacketDispatch() {
                    Name = wrapper.Packet.Words[0],
                    Origin = wrapper.Packet.Origin
                }, wrapper, null);
            }
        }
    }
}
