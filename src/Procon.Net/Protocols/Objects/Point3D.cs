using System;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Point3D : NetworkObject {

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Point3D() : base() {

        }

        public Point3D(string x, string y, string z) : base() {
            int iX = 0, iY = 0, iZ = 0;

            int.TryParse(x, out iX);
            int.TryParse(y, out iY);
            int.TryParse(z, out iZ);

            this.X = iX;
            this.Y = iY;
            this.Z = iZ;
        }
    }
}
