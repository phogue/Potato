using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Shared.Geolocation;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Procon.Fuzzy to use for geo location information
    /// </summary>
    public class LocationThingReference : IThingReference {

        /// <summary>
        /// The location attached to this thing.
        /// </summary>
        public List<Location> Locations { get; set; }

        public bool CompatibleWith(IThingReference other) {
            return other is LocationThingReference || other is PlayerThingReference;
        }

        public IThingReference Union(IThingReference other) {
            LocationThingReference locationThingReference = other as LocationThingReference;

            if (locationThingReference != null) {
                this.Locations.AddRange(locationThingReference.Locations.Where(location => this.Locations.Contains(location) == false));
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            LocationThingReference locationThingReference = other as LocationThingReference;

            if (locationThingReference != null) {
                this.Locations.RemoveAll(location => locationThingReference.Locations.Contains(location));
            }

            return this;
        }
    }
}
