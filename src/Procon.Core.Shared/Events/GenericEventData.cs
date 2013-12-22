using Procon.Net;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Events {
    public static class GenericEventData {

        /// <summary>
        /// Parses a gameEventData object, returning a generic event data. We do this instead of
        /// inheritance in case we need to extend events from NLP or another library in the future.
        /// </summary>
        /// <param name="gameEventData"></param>
        /// <returns>A new GenericEventData object with the gameEventData included.</returns>
        public static CommandData Parse(GameEventData gameEventData) {
            return new CommandData() {
                Chats = gameEventData.Chats,
                Players = gameEventData.Players,
                Kills = gameEventData.Kills,
                Moves = gameEventData.Moves,
                Spawns = gameEventData.Spawns,
                Kicks = gameEventData.Kicks,
                Bans = gameEventData.Bans,
                Settings = gameEventData.Settings

                // Maps?
            };
        }
    }
}
