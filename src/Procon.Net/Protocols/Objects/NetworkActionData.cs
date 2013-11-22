using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public class NetworkActionData {

        /// <summary>
        /// List of players that have an effect with this action.
        /// </summary>
        public List<Player> Players { get; set; } 

        /// <summary>
        /// A list of strings attached to this network action.
        /// </summary>
        public List<String> Content { get; set; }

        /// <summary>
        /// The groups attached to this action.
        /// </summary>
        public List<Grouping> Groups { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this action, if any.
        /// </summary>
        public List<Point3D> Points { get; set; }

        /// <summary>
        /// List of items attached to this action, if any.
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
