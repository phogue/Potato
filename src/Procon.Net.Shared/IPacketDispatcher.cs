using System;
using System.Collections.Generic;

namespace Procon.Net.Shared {
    /// <summary>
    /// A dispatcher of packets, will inspect data within a packet and if it matches
    /// will call the respective handler.
    /// </summary>
    public interface IPacketDispatcher {
        /// <summary>
        /// Appends a dispatch handler, first checking if an existing dispatch exists for this exact
        /// packet. If it exists then it will be overridden.
        /// </summary>
        /// <param name="handlers">A dictionary of handlers to append to the dispatch handlers.</param>
        void Append(Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>> handlers);

        /// <summary>
        /// Dispatches a recieved packet. Each game implementation needs to supply its own dispatch
        /// method as the protocol may be very different and have additional requirements beyond a 
        /// simple text match.
        /// </summary>
        /// <param name="packet">The packet recieved from the game server.</param>
        void Dispatch(IPacketWrapper packet);
    }
}
