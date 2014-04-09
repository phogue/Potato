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
using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Procon.Fuzzy to use for map information
    /// </summary>
    public class MapThingReference : IThingReference {
        /// <summary>
        /// The map attached to this thing.
        /// </summary>
        public List<MapModel> Maps { get; set; }

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
