using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Shared.Actions;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Procon.Fuzzy to use for map information
    /// </summary>
    public class MapThingReference : IThingReference {
        /// <summary>
        /// The map attached to this thing.
        /// </summary>
        public List<Map> Maps { get; set; }

        public bool CompatibleWith(IThingReference other) {
            return other is MapThingReference;
        }

        public IThingReference Union(IThingReference other) {
            MapThingReference mapThingReference = other as MapThingReference;

            if (mapThingReference != null) {
                this.Maps.AddRange(mapThingReference.Maps.Where(map => this.Maps.Contains(map) == false));
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            MapThingReference mapThingReference = other as MapThingReference;

            if (mapThingReference != null) {
                this.Maps.RemoveAll(map => mapThingReference.Maps.Contains(map));
            }

            return this;
        }
    }
}
