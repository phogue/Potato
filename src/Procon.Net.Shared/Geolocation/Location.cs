using System;

namespace Procon.Net.Shared.Geolocation {

    [Serializable]
    public sealed class Location {
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

        public Location() {
            this.CountryName = String.Empty;
            this.CountryCode = String.Empty;
        }
    }
}
