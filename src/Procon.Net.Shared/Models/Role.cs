using System;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class Role : NetworkModel {

        public String Name { get; set; }

        public Role() : base() {
            this.Name = String.Empty;
        }
    }
}
