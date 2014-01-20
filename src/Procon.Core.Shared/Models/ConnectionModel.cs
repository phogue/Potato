using System;
using Newtonsoft.Json;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Details of an open protocol
    /// </summary>
    [Serializable]
    public class ConnectionModel : CoreModel {
        /// <summary>
        /// The unique hash for this connection. This simplifies identifying a connection to a single string that can be compared.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// The protocol type of the connection this is describing
        /// </summary>
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// The host name end point of the established connection
        /// </summary>
        public String Hostname { get; set; }

        /// <summary>
        /// The host port end point of the established connection
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The password used to connect to the end point.
        /// </summary>
        [JsonIgnore]
        public String Password { get; set; }

        /// <summary>
        /// Proxy to the active protocol additional field. It's obsolete and will be replaced prior to release.
        /// </summary>
        public String Additional { get; set; }
    }
}
