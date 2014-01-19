using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class InventoryModel : NetworkModel {

        public InventoryModel() {
            this.Now.Items = new List<ItemModel>();
        }
    }
}
