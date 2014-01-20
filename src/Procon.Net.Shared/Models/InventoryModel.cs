using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A players inventory
    /// </summary>
    [Serializable]
    public sealed class InventoryModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public InventoryModel() {
            this.Now.Items = new List<ItemModel>();
        }
    }
}
