using System;

namespace Procon.Net.Models {
    [Serializable]
    public sealed class Item : NetworkObject {

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }
    }
}
