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

namespace Potato.Net.Shared.Models {
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
        public string CountryName { get; set; }

        /// <summary>
        /// ISO 3166-1 Alpha-2 country code
        /// http://en.wikipedia.org/wiki/ISO_3166-1
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Initializes with the default values
        /// </summary>
        public Location() {
            CountryName = string.Empty;
            CountryCode = string.Empty;
        }
    }
}
