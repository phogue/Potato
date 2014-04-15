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
    /// A three dimensional location of a player/object.
    /// </summary>
    [Serializable]
    public sealed class Point3DModel : NetworkModel {
        /// <summary>
        /// X coordinate of the player or object
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate of the player or object
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Z coordinate of the player or object
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Initializes an empty point
        /// </summary>
        public Point3DModel() : base() {

        }

        /// <summary>
        /// Initializes a point from three strings
        /// </summary>
        public Point3DModel(String x, String y, String z) : base() {
            float iX = 0, iY = 0, iZ = 0;

            float.TryParse(x, out iX);
            float.TryParse(y, out iY);
            float.TryParse(z, out iZ);

            this.X = iX;
            this.Y = iY;
            this.Z = iZ;
        }
    }
}
