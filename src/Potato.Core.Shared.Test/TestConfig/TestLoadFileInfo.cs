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
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Potato.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestLoadFileInfo : TestConfigBase {
        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestDocumentNotNull() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestRootNotNullWithMatchingProperty() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if no matching property is found. It
        /// will then get the first property in the document.
        /// </summary>
        [Test]
        public void TestRootNotNullWithoutMatchingProperty() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""This.Type.Does.Not.Exist"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if the resulting document is empty.
        /// </summary>
        [Test]
        public void TestRootNotNullEmptyFile() {
            File.WriteAllText(ConfigFileA.FullName, @"{ }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if the first property is not an array
        /// </summary>
        [Test]
        public void TestRootNotNullNotArray() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""This.Type.Does.Not.Exist"": { ""Name"":""Phogue"", ""Age"": 100  } }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestRootSetToFilePath() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.AreEqual("Phogue", config.Root.First["Name"].Value<string>());
            Assert.AreEqual(100, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that invalid json will result in an empty document/root
        /// </summary>
        [Test]
        public void TestInvalidJsonParsingNotNullRoot() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""N 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that invalid json will result in an empty document/root
        /// </summary>
        [Test]
        public void TestInvalidJsonParsingNotNullDocument() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""N 100  } ] }");

            var config = new Config().Load(ConfigFileA);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into union.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            new Config().Load((FileInfo) null);
        }
    }
}
