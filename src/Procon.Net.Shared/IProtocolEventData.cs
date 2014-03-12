using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Holds data attached to a protocol event
    /// </summary>
    public interface IProtocolEventData {
        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        List<ChatModel> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        List<PlayerModel> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        List<KillModel> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        List<MoveModel> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        List<SpawnModel> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        List<KickModel> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        List<BanModel> Bans { get; set; }

        /// <summary>
        /// The maps attached to this event, if any.
        /// </summary>
        List<MapModel> Maps { get; set; }

        /// <summary>
        /// The settings attached to this event, if any.
        /// </summary>
        List<Settings> Settings { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this event, if any.
        /// </summary>
        List<Point3DModel> Points { get; set; }

        /// <summary>
        /// List of items attached to this event, if any.
        /// </summary>
        List<ItemModel> Items { get; set; }
    }
}
