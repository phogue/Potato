namespace Procon.Net.Shared {
    /// <summary>
    /// Wraps a packet, so protocols can implement additional information
    /// attached throughout the client/protocol implementation without
    /// altering the basic packet.
    /// </summary>
    public interface IPacketWrapper {
        /// <summary>
        /// The underlying simple packet class 
        /// </summary>
        IPacket Packet { get; set; }
    }
}
