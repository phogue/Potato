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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Truths;

namespace Potato.Net.Shared.Test.Models {
    [TestFixture]
    public class TestModels {

        /// <summary>
        /// Test that all non-static classes within the Potato.Net.Shared.Models namespace
        /// are marked as serializable.
        /// </summary>
        [Test]
        public void TestClassesAreSerializable() {
            var nonSerializableTypes = typeof(ITruth).Assembly
                .GetTypes()
                // Where it's a class
                .Where(type => type.IsClass == true)
                // Where it's not static (both are true when static)
                .Where(type => type.IsAbstract == false || type.IsSealed == false)
                // Where the type exists in our namespace
                .Where(type => type.Namespace != null && type.Namespace.Equals("Potato.Net.Shared.Models"))
                // The class is not generated for an anonymous delegate (I assume this is what it is?)
                .Where(type => type.IsNested == false)
                // Where they are not serializable
                .Where(type => type.IsSerializable == false);

            Assert.IsEmpty(nonSerializableTypes);
        }
    }
}
