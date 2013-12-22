using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Permissions are modeled on Teamspeak3's permissions.
    /// Keep it familier with what people know.
    /// </summary>
    [Serializable]
    public class PermissionModel : CommandAttribute, IDisposable {
        /// <summary>
        /// The power/value they have for this permission
        /// null means no value has been set for this group.
        /// </summary>
        public int? Authority { get; set; }

        public PermissionModel() {
            this.Authority = null;
        }

        public new void Dispose() {
            base.Dispose();

            this.Authority = null;
        }
    }
}
