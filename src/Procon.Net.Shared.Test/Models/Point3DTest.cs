using NUnit.Framework;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Test.Models {
    [TestFixture]
    public class Point3DTest {

        /// <summary>
        /// Tests the point is zeroed when using the empty constructor
        /// </summary>
        [Test]
        public void TestPointZeroed() {
            Point3DModel point = new Point3DModel();

            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
            Assert.AreEqual(0, point.Z);
        }

        /// <summary>
        /// Tests the point is parsed successfully when strings are passed into the constructor
        /// </summary>
        [Test]
        public void TestPointParsed() {
            Point3DModel point = new Point3DModel("1", "2", "3");

            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
            Assert.AreEqual(3, point.Z);
        }
    }
}
