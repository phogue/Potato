﻿using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// The type of packet, originally sent or recieved.
    /// </summary>
    [Flags]
    public enum PacketType {
        /// <summary>
        /// Unknown direction
        /// </summary>
        None,
        /// <summary>
        /// Packet is a request for action to be taken by the client or server
        /// </summary>
        Request,
        /// <summary>
        /// Packet is a response to an action taken by the client or server
        /// </summary>
        Response
    }
}
