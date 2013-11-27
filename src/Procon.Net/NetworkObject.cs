using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Net {

    [Serializable]
    public abstract class NetworkObject : ICloneable {

        /// <summary>
        /// When this object was created.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTime Created { get; set; }

        protected NetworkObject() {
            this.Created = DateTime.Now;
        }

        /// <summary>
        /// Returns a shallow copy of this object.
        /// </summary>
        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
