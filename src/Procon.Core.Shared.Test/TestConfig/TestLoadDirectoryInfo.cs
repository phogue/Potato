using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestLoadDirectoryInfo : TestConfigBase {
        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestSingleDocumentNotNull() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestSingleRootNotNull() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestSingleRootSetToFilePath() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.AreEqual("Phogue", config.Root.First["Name"].Value<String>());
            Assert.AreEqual(100, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestUnionedDocumentNotNull() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(this.ConfigFileB.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestUnionedRootNotNull() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(this.ConfigFileB.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// Files are loaded in alphabetical order.
        /// </summary>
        [Test]
        public void TestUnionedRootSetToFirstFilePath() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(this.ConfigFileB.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.AreEqual("Ike", config.Root.First["Name"].Value<String>());
            Assert.AreEqual(10, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the document contains the alternative data
        /// </summary>
        [Test]
        public void TestUnionedDocumentContainsAlternativeToRoot() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(this.ConfigFileB.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.AlternativeName"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.AreEqual("Phogue", config.Document["Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].First["Name"].Value<String>());
            Assert.AreEqual(100, config.Document["Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestUnionedWithIdenticalNamespaces() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");
            File.WriteAllText(this.ConfigFileB.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Ike"", ""Age"": 10  } ] }");

            IConfig config = new JsonConfig().Load(this.ConfigFileA.Directory);

            Assert.AreEqual("Ike", config.Root.First["Name"].Value<String>());
            Assert.AreEqual(10, config.Root.First["Age"].Value<int>());

            Assert.AreEqual("Phogue", config.Root.Last["Name"].Value<String>());
            Assert.AreEqual(100, config.Root.Last["Age"].Value<int>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into union.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            new JsonConfig().Load((DirectoryInfo)null);
        }
    }
}
