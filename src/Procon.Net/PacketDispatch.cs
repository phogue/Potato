using System;

namespace Procon.Net {
    public sealed class PacketDispatch {

        /// <summary>
        /// The name of the packet we should check against
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Where the packet originated from (initiated from client, recieved response or initiated by server, expecting response)
        /// </summary>
        public PacketOrigin Origin { get; set; }

        // TODO: Add version information and allow multiples
        // Also include: "Fallback" so if no function matching the specified version
        // exist procon will fallback on the method with this flag (set to true by default
        // need to specify false on newer methods)
    }
}
