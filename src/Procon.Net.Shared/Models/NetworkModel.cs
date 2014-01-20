using System;
using Newtonsoft.Json;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Base model for objects within Procon.Net.*
    /// </summary>
    [Serializable]
    public class NetworkModel : INetworkModel, ICloneable {
        [JsonIgnore]
        public DateTime Created { get; set; }

        public NetworkOrigin Origin { get; set; }

        public NetworkModelData Scope { get; set; }

        public NetworkModelData Then { get; set; }

        public NetworkModelData Now { get; set; }

        /// <summary>
        /// Initializes the model with the default values.
        /// </summary>
        public NetworkModel() {
            this.Created = DateTime.Now;

            this.Scope = new NetworkModelData();
            this.Then = new NetworkModelData();
            this.Now = new NetworkModelData();
        }

        /// <summary>
        /// Returns a shallow copy of this object.
        /// </summary>
        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
