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
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.Models {
    [TestFixture]
    public class Point3DTest {

        /// <summary>
        /// Tests the point is zeroed when using the empty constructor
        /// </summary>
        [Test]
        public void TestPointZeroed() {
            var point = new Point3DModel();

            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
            Assert.AreEqual(0, point.Z);
        }

        /// <summary>
        /// Tests the point is parsed successfully when strings are passed into the constructor
        /// </summary>
        [Test]
        public void TestPointParsed() {
            var point = new Point3DModel("1", "2", "3");

            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
            Assert.AreEqual(3, point.Z);
        }
    }
}
