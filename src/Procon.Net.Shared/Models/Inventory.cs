using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class Inventory : NetworkModel {

        /// <summary>
        /// List of items found in this inventory
        /// </summary>
        public List<Item> Items { get; set; }

        public Inventory() {
            this.Items = new List<Item>();
        }
    }
}
