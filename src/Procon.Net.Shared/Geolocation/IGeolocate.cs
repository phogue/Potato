using System;

namespace Procon.Net.Shared.Geolocation {
    public interface IGeolocate {

        /// <summary>
        /// Fetch a location by a string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Location Locate(String value);
    }
}
