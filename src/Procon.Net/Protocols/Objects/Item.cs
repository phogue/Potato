using System;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Item : NetworkObject {

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }
    }
}
