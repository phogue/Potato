﻿using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// The original origin of the packet
    /// </summary>
    [Flags]
    public enum PacketOrigin {
        /// <summary>
        /// Unknown origin
        /// </summary>
        None,
        /// <summary>
        /// Packet was generated and sent by the server
        /// </summary>
        Server,
        /// <summary>
        /// Packet was generated and sent by the client
        /// </summary>
        Client
    }
}
