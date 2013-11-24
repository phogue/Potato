using System.Collections.Generic;

namespace Procon.Net {
    public interface IPacketDispatcher {

        /// <summary>
        /// Appends a dispatch handler, first checking if an existing dispatch exists for this exact
        /// packet. If it exists then it will be overridden.
        /// </summary>
        /// <param name="handlers">A dictionary of handlers to append to the dispatch handlers.</param>
        void Append(Dictionary<PacketDispatch, PacketDispatcher.PacketDispatchHandler> handlers);

        /// <summary>
        /// Dispatches a recieved packet. Each game implementation needs to supply its own dispatch
        /// method as the protocol may be very different and have additional requirements beyond a 
        /// simple text match.
        /// </summary>
        /// <param name="packet">The packet recieved from the game server.</param>
        void Dispatch(Packet packet);
    }
}
