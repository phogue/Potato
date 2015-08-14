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

using System;

namespace Potato.Net.Shared {
    public sealed class PacketDispatch : IEquatable<PacketDispatch>, IPacketDispatch {

        /// <summary>
        /// The name of the packet we should check against
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Where the packet originated from (initiated from client, recieved response or initiated by server, expecting response)
        /// </summary>
        public PacketOrigin Origin { get; set; }

        // TODO: Add version information and allow multiples
        // Also include: "Fallback" so if no function matching the specified version
        // exist Potato will fallback on the method with this flag (set to true by default
        // need to specify false on newer methods)

        public bool Equals(PacketDispatch other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name) && Origin == other.Origin;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is PacketDispatch && Equals((PacketDispatch)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (int)Origin;
            }
        }
    }
}
