using System;
using Procon.Net;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class ConnectionModel : CoreModel {
        /// <summary>
        /// The unique hash for this connection. This simplifies identifying a connection to a single string that can be compared.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        public GameType GameType { get; set; }

        public String Hostname { get; set; }

        public ushort Port { get; set; }
    }
}
