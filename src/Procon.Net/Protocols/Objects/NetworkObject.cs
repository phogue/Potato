using System;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public abstract class NetworkObject : ICloneable {

        /// <summary>
        /// When this object was created.
        /// </summary>
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
