using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// Decoration for a protocol type
    /// </summary>
    public class ProtocolDeclarationAttribute : Attribute, IProtocolType {
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
    }
}
