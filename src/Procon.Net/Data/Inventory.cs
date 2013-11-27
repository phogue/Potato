using System;
using System.Collections.Generic;

namespace Procon.Net.Data {
    [Serializable]
    public sealed class Inventory : NetworkObject {

        /// <summary>
        /// List of items found in this inventory
        /// </summary>
        public List<Item> Items { get; set; }

        public Inventory() {
            this.Items = new List<Item>();
        }
    }
}
