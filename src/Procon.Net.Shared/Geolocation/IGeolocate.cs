using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Geolocation {
    /// <summary>
    /// Geolocation by a string value
    /// </summary>
    public interface IGeolocate {
        /// <summary>
        /// Fetch a location by a string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Location Locate(String value);
    }
}
