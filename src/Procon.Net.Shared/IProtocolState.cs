using System.Collections.Generic;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Truths;

namespace Procon.Net.Shared {
    /// <summary>
    /// The current snapshot of the server with all details Procon has captured or inferred.
    /// </summary>
    public interface IProtocolState {
        /// <summary>
        /// All current information about each player in the server
        /// </summary>
        List<PlayerModel> Players { get; set; }

        /// <summary>
        /// The current maplist
        /// </summary>
        List<MapModel> Maps { get; set; }

        /// <summary>
        /// The current banlist
        /// </summary>
        List<BanModel> Bans { get; set; }

        /// <summary>
        /// List of available maps for this game
        /// </summary>
        List<MapModel> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        List<GameModeModel> GameModePool { get; set; }

        /// <summary>
        /// List of potential groups available 
        /// </summary>
        List<GroupingModel> Groupings { get; set; }

        /// <summary>
        /// List of potential items available in this game.
        /// </summary>
        List<ItemModel> Items { get; set; }

        /// <summary>
        /// Various settings that are sent by the server.
        /// </summary>
        Settings Settings { get; set; }

        /// <summary>
        /// A tree of truths describing everything Procon knows about the game running.
        /// </summary>
        ITruth Support { get; set; }
    }
}
