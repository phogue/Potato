#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using System.Linq;
using Potato.Fuzzy.Tokens.Object;
using Potato.Net.Shared.Geolocation;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Potato.Fuzzy to use for geo location information
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
            var locationThingReference = other as LocationThingReference;

            if (locationThingReference != null) {
                Locations.AddRange(locationThingReference.Locations.Where(location => Locations.Contains(location) == false));
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            var locationThingReference = other as LocationThingReference;

            if (locationThingReference != null) {
                Locations.RemoveAll(location => locationThingReference.Locations.Contains(location));
            }

            return this;
        }
    }
}
