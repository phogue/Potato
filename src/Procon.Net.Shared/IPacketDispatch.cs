using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// A simple predicate to determine if the packet matches criteria to dispatch
    /// </summary>
    public interface IPacketDispatch {
        /// <summary>
        /// The name of the packet we should check against
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Where the packet originated from (initiated from client, recieved response or initiated by server, expecting response)
        /// </summary>
        PacketOrigin Origin { get; set; }
    }
}
