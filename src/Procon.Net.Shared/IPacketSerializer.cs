namespace Procon.Net.Shared {
    public interface IPacketSerializer {
        
        /// <summary>
        /// The minimum packet size requires to be passed into the packet serializer. Anything smaller
        /// and it the full header of a packet wouldn't be available, therefore we wouldn't know
        /// how many bytes the full packet is.
        /// </summary>
        uint PacketHeaderSize { get; set; }

        /// <summary>
        /// Deserializes an array of bytes into a Packet of type P
        /// </summary>
        /// <param name="packetData">The array to deserialize to a packet. Must be exact length of bytes.</param>
        /// <returns>A new packet with data extracted from packetDate</returns>
        IPacketWrapper Deserialize(byte[] packetData);

        /// <summary>
        /// Serializes a packet into an array of bytes to send to the server.
        /// </summary>
        /// <param name="packet">The packe to serialize</param>
        /// <returns>An array of bytes to send to the server.</returns>
        byte[] Serialize(IPacketWrapper packet);

        /// <summary>
        /// Fetches the full packet size by reading the header of a packet.
        /// </summary>
        /// <param name="packetData">The possibly incomplete packet data, or as much data as we have recieved from the server.</param>
        /// <returns>The total size, in bytes, that is requires for the header + data to be deserialized.</returns>
        long ReadPacketSize(byte[] packetData);
    }
}
