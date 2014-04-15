#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.IO;
using System.Linq;
using Potato.Net.Shared.Geolocation.Maxmind;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Geolocation {
    /// <summary>
    /// Geolocation with an IP using Maxmind's library and database.
    /// </summary>
    public class GeolocateIp : IGeolocate {
        /// <summary>
        /// Used when determining a player's Country Name and Code.
        /// </summary>
        protected static CountryLookup CountryLookup = null;

        /// <summary>
        /// Loads the Maxmind GeoIP database, if available.
        /// </summary>
        static GeolocateIp() {
            try {
                String path = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "GeoIP.dat", SearchOption.AllDirectories).FirstOrDefault();

                if (String.IsNullOrEmpty(path) == false) {
                    GeolocateIp.CountryLookup = new CountryLookup(path);
                }
            }
            catch {
                GeolocateIp.CountryLookup = null;
            }
        }

        /// <summary>
        /// Validates the IP is roughly in ipv4 and gets the ip from a ip:port pair.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected String Ip(String value) {
            String ip = value;

            // Validate Ip has colon before trying to split.
            if (value != null && value.Contains(":")) {
                ip = value.Split(':').FirstOrDefault();
            }

            // Validate Ip String was valid before continuing.
            if (string.IsNullOrEmpty(ip) == true || ip.Contains('.') == false) {
                ip = null;
            }

            return ip;
        }

        /// <summary>
        /// Fetch a location by ip
        /// </summary>
        /// <param name="value">The ip</param>
        /// <returns></returns>
        public Location Locate(String value) {
            Location location = null;

            if (GeolocateIp.CountryLookup != null) {
                String ip = this.Ip(value);

                if (ip != null) {
                    // Try: In-case GeoIP.dat is not loaded properly
                    try {
                        location = new Location() {
                            CountryName = GeolocateIp.CountryLookup.lookupCountryName(ip),
                            CountryCode = GeolocateIp.CountryLookup.lookupCountryCode(ip)
                        };
                    }
                    catch {
                        // Blank the location
                        location = null;
                    }
                }
            }

            return location;
        }
    }
}
