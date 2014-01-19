using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public class NetworkModel : ICloneable {

        /// <summary>
        /// When this object was created.
        /// </summary>
        [JsonIgnore]
        public DateTime Created { get; set; }

        /// <summary>
        /// Where this data originated from.
        /// </summary>
        public NetworkOrigin Origin { get; set; }

        /// <summary>
        /// The limiting factor(s) of the action
        /// </summary>
        public NetworkModelData Scope { get; set; }

        /// <summary>
        /// Any data that reflects what something looked like before the action was taken
        /// </summary>
        public NetworkModelData Then { get; set; }

        /// <summary>
        /// Any data showing the modifications or new data that was introduced with this action.
        /// </summary>
        public NetworkModelData Now { get; set; }

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
