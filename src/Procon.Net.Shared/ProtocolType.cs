using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// A protocol type wrapper describing what the protocol should connect to and who
    /// wrote the implementation
    /// </summary>
    [Serializable]
    public class ProtocolType : IProtocolType {
        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        public String Provider { get; set; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        public String Name { get; set; }
        
        /// <summary>
        /// Initalizes the protocol type with empty values.
        /// </summary>
        public ProtocolType() {
            this.Provider = String.Empty;
            this.Type = String.Empty;
            this.Name = String.Empty;
        }

        /// <summary>
        /// Initializes the protocol type from another type
        /// </summary>
        public ProtocolType(IProtocolType from) {
            this.Provider = from.Provider;
            this.Type = from.Type;
            this.Name = from.Name;
        }
    }
}
