using System;
using System.Collections.Generic;

namespace Procon.Net.Models {
    /// <summary>
    /// An item or weapon used by a player in game.
    /// </summary>
    [Serializable]
    public sealed class Item : NetworkModel {

        /// <summary>
        /// The name of the item, used by the game.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A human readable version of the name.
        /// </summary>
        public String FriendlyName { get; set; }

        /// <summary>
        /// List of tags to categorize the item by
        /// </summary>
        public List<String> Tags { get; set; }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public Item() {
            this.Name = String.Empty;
            this.FriendlyName = String.Empty;
            this.Tags = new List<String>();
        }
    }
}
