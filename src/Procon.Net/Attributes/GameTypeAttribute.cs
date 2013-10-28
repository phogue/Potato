using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Net.Attributes {

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GameTypeAttribute : Attribute {

        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        public String Provider { get; set; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Overridden here simply so we can remove the attribute during serialization
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public override object TypeId {
            get {
                return base.TypeId;
            }
        }
    }
}
