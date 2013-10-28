using System;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Role : NetworkObject {

        public String Name { get; set; }

        public Role() : base() {
            this.Name = String.Empty;
        }
    }
}
