using System;

namespace Procon.Net {
    public class GameDeclarationAttribute : Attribute, IGameType {
        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        public string Name { get; set; }
    }
}
