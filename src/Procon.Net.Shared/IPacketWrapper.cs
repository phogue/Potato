namespace Procon.Net.Shared {
    public interface IPacketWrapper {

        /// <summary>
        /// The underlying simple packet class 
        /// </summary>
        IPacket Packet { get; set; }
    }
}
