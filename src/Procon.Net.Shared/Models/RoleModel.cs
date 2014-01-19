using System;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class RoleModel : NetworkModel {

        public String Name { get; set; }

        public RoleModel() : base() {
            this.Name = String.Empty;
        }
    }
}
