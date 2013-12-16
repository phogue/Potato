using System;

namespace Procon.Net.Models {
    [Serializable]
    public sealed class Role : NetworkObject {

        public String Name { get; set; }

        public Role() : base() {
            this.Name = String.Empty;
        }
    }
}
