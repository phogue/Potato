using System;
using System.Collections.Generic;
using Procon.Net.Shared;

namespace Procon.Core.Shared {
    /// <summary>
    /// Holds all information about a proxied request (via the command server)
    /// </summary>
    public interface ICommandRequest {
        /// <summary>
        /// Dictionary of base string variables attached to the request.
        /// </summary>
        Dictionary<String, String> Tags { get; set; }

        /// <summary>
        /// The full content of the original request
        /// </summary>
        List<String> Content { get; set; }

        /// <summary>
        /// Any packets attached to this request
        /// </summary>
        List<IPacket> Packets { get; set; }
    }
}
