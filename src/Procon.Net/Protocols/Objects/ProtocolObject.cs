using System;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public class ProtocolObject : DataController, ICloneable
    {
        // Public Properties
        public DateTime Created { get; private set; }

        // Constructor
        public ProtocolObject() { this.Created = DateTime.Now; }

        #region ICloneable

        /// <summary>Returns a shallow copy of this object.</summary>
        public object Clone() { return this.MemberwiseClone(); }

        #endregion
    }
}
