using System;

namespace Procon.Net.Shared {
    public interface IGameType {
        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        String Provider { get; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        String Type { get; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        String Name { get; }
    }
}
