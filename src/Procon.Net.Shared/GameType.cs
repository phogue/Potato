using System;

namespace Procon.Net.Shared {
    [Serializable]
    public class GameType : IGameType {
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
        
        public GameType() {
            this.Provider = String.Empty;
            this.Type = String.Empty;
            this.Name = String.Empty;
        }

        public GameType(IGameType from) {
            this.Provider = from.Provider;
            this.Type = from.Type;
            this.Name = from.Name;
        }
    }
}
