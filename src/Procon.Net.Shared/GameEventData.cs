using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    [Serializable]
    public class GameEventData {
        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        public List<Chat> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        public List<Kill> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        public List<Move> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        public List<Spawn> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        public List<Kick> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        public List<Ban> Bans { get; set; }

        /// <summary>
        /// The settings attached to this event, if any.
        /// </summary>
        public List<Settings> Settings { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this event, if any.
        /// </summary>
        public List<Point3D> Points { get; set; }

        /// <summary>
        /// List of items attached to this event, if any.
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
