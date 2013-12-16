using System;

namespace Procon.Net.Models {

    /// <summary>
    /// A three dimensional location of a player/object.
    /// </summary>
    [Serializable]
    public sealed class Point3D : NetworkModel {

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

        public Point3D() : base() {

        }

        public Point3D(String x, String y, String z) : base() {
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
