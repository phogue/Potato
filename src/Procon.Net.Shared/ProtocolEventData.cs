using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Holds data attached to a protocol event
    /// </summary>
    [Serializable]
    public class ProtocolEventData : IProtocolEventData {
        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        public List<ChatModel> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        public List<PlayerModel> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        public List<KillModel> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        public List<MoveModel> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        public List<SpawnModel> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        public List<KickModel> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        public List<BanModel> Bans { get; set; }

        /// <summary>
        /// The settings attached to this event, if any.
        /// </summary>
        public List<Settings> Settings { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this event, if any.
        /// </summary>
        public List<Point3dModel> Points { get; set; }

        /// <summary>
        /// List of items attached to this event, if any.
        /// </summary>
        public List<ItemModel> Items { get; set; }
    }
}
