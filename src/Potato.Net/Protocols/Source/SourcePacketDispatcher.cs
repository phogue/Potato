using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Net.Protocols.Source {
    public class SourcePacketDispatcher : PacketDispatcher {

        /// <summary>
        /// Handles a queue of packets, used to find the request packet.
        /// </summary>
        public IPacketQueue PacketQueue { get; set; }

        public override void Dispatch(Packet packet) {
            SourcePacket sourcePacket = packet as SourcePacket;

            if (sourcePacket != null) {
                if (sourcePacket.Origin == PacketOrigin.Client && packet.Type == PacketType.Response) {
                    Packet requestPacket = this.PacketQueue.GetRequestPacket(sourcePacket);

                    if (sourcePacket.ResponseType == SourceResponseType.ServerDataAuthResponse) {
                        this.Dispatch(new PacketDispatch() { Name = "login" }, requestPacket, sourcePacket);
                    }
                    // If the request packet is valid and has at least one word.
                    else if (requestPacket != null && requestPacket.Words.Count >= 1) {

                        // If the sent command was successful
                        if (sourcePacket.Words.Count >= 1 && sourcePacket.RequestType != SourceRequestType.SERVERDATA_ALLBAD) {
                            this.Dispatch(new PacketDispatch() { Name = requestPacket.Words[0] }, requestPacket, sourcePacket);
                        }
                        else { // The command sent failed for some reason.
                            this.Dispatch(new PacketDispatch() { Name = sourcePacket.Words[0] }, requestPacket, sourcePacket);
                        }
                    }
                }
                else if (sourcePacket.Origin == PacketOrigin.Server && sourcePacket.Type == PacketType.Request) {
                    this.Dispatch(new PacketDispatch() { Name = "log" }, sourcePacket, null);
                }
            }
        }
    }
}
