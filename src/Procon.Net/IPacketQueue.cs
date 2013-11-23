namespace Procon.Net {
    public interface IPacketQueue {

        /// <summary>
        /// Clears the current queue
        /// </summary>
        void Clear();

        /// <summary>
        /// Validates that packets are not 'lost' after being sent. If this is the case then the connection is shutdown
        /// to then be rebooted at a later time.
        /// 
        /// If a packet exists in our outgoing "SentPackets"
        /// </summary>
        bool RestartConnectionOnQueueFailure();

        /// <summary>
        /// Fetches the packet that initiated the request.
        /// </summary>
        /// <param name="recievedPacket">The response packet</param>
        /// <returns>The request packet</returns>
        Packet GetRequestPacket(Packet recievedPacket);

        /// <summary>
        /// Dequeues the current packet. If a packet is returned then it should be sent
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        Packet PacketReceived(Packet packet);

        /// <summary>
        /// Enqueues a packet, also pops a packet for sending if a packet is waiting.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        Packet PacketSend(Packet packet);
    }
}
