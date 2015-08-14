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
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Potato.Core.Shared.Test.TestConfig.Mocks;

namespace Potato.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestRootOf : TestConfigBase {
        /// <summary>
        /// Tests that a root can be fetched from an existing type
        /// </summary>
        [Test]
        public void TestRootOfSingleProperty() {
            var config = new Config().Create<MockSimpleConcrete>();

            var root = config.RootOf<MockSimpleConcrete>();

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be fetched when multiple properties exist on the document
        /// </summary>
        [Test]
        public void TestRootOfMultipleProperties() {
            var config = new Config().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Potato.Core.Shared.Test.TestConfig.Mocks.DoesNotExist", new JArray()));

            var root = config.RootOf<MockSimpleConcrete>();

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestRootOfNotCreatedForProperty() {
            var config = new Config().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Potato.Core.Shared.Test.TestConfig.TestRootOf", new JArray()));

            var root = config.RootOf<TestRootOf>();

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.TestRootOf", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestRootOfNonExistantPropertyReturnsNotNull() {
            var config = new Config().Create<MockSimpleConcrete>();

            var root = config.RootOf<TestRootOf>();

            Assert.IsNotNull(root);
        }

        /// <summary>
        /// Tests that a root can be fetched from an existing type
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfSingleProperty() {
            var config = new Config().Create<MockSimpleConcrete>();

            var root = config.RootOf(typeof(MockSimpleConcrete));

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be fetched when multiple properties exist on the document
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfMultipleProperties() {
            var config = new Config().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Potato.Core.Shared.Test.TestConfig.Mocks.DoesNotExist", new JArray()));

            var root = config.RootOf(typeof(MockSimpleConcrete));

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfNotCreatedForProperty() {
            var config = new Config().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Potato.Core.Shared.Test.TestConfig.TestRootOf", new JArray()));

            var root = config.RootOf(typeof(TestRootOf));

            Assert.AreEqual("Potato.Core.Shared.Test.TestConfig.TestRootOf", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfNonExistantPropertyReturnsNotNull() {
            var config = new Config().Create<MockSimpleConcrete>();

            var root = config.RootOf(typeof(TestRootOf));

            Assert.IsNotNull(root);
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into RootOf.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestTypeNullException() {
            new Config().RootOf((Type)null);
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into RootOf.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestStringNullException() {
            new Config().RootOf((string)null);
        }
    }
}
