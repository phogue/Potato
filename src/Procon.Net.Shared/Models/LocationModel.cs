using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A physical location with as much or as little information that is available.
    /// </summary>
    [Serializable]
    public sealed class Location : NetworkModel {
        /// <summary>
        /// The latitude of the location
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude of the location
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The english country name
        /// </summary>
        public String CountryName { get; set; }

        /// <summary>
        /// ISO 3166-1 Alpha-2 country code
        /// http://en.wikipedia.org/wiki/ISO_3166-1
        /// </summary>
        public String CountryCode { get; set; }

        /// <summary>
        /// Initializes with the default values
        /// </summary>
        public Location() {
            this.CountryName = String.Empty;
            this.CountryCode = String.Empty;
        }
    }
}
