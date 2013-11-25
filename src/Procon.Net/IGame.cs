using Procon.Net.Protocols.Objects;

namespace Procon.Net {
    public interface IGame {

        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        IClient Client { get; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        GameState State { get; }

        // todo more, but I needed the interface for just this right now.
    }
}
