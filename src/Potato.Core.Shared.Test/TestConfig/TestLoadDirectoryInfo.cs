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
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Potato.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestLoadDirectoryInfo : TestConfigBase {
        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestSingleDocumentNotNull() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestSingleRootNotNull() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestSingleRootSetToFilePath() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.AreEqual("Phogue", config.Root.First["Name"].Value<string>());
            Assert.AreEqual(100, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestUnionedDocumentNotNull() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(ConfigFileB.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestUnionedRootNotNull() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(ConfigFileB.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// Files are loaded in alphabetical order.
        /// </summary>
        [Test]
        public void TestUnionedRootSetToFirstFilePath() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(ConfigFileB.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.AreEqual("Ike", config.Root.First["Name"].Value<string>());
            Assert.AreEqual(10, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the document contains the alternative data
        /// </summary>
        [Test]
        public void TestUnionedDocumentContainsAlternativeToRoot() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(ConfigFileB.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.AreEqual("Phogue", config.Document["Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].First["Name"].Value<string>());
            Assert.AreEqual(100, config.Document["Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestUnionedWithIdenticalNamespaces() {
            File.WriteAllText(ConfigFileA.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(ConfigFileB.FullName, @"{ ""Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            var config = new Config().Load(ConfigFileA.Directory);

            Assert.AreEqual("Ike", config.Root.First["Name"].Value<string>());
            Assert.AreEqual(10, config.Root.First["Age"].Value<int>());

            Assert.AreEqual("Phogue", config.Root.Last["Name"].Value<string>());
            Assert.AreEqual(100, config.Root.Last["Age"].Value<int>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into union.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            new Config().Load((DirectoryInfo)null);
        }
    }
}
