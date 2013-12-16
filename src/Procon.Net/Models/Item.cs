using System;

namespace Procon.Net.Models {
    [Serializable]
    public sealed class Item : NetworkModel {

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }
    }
}
