using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestLoadFileInfo : TestConfigBase {
        /// <summary>
        /// Tests that the document is populated after loading 
        /// </summary>
        [Test]
        public void TestDocumentNotNull() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Document);
        }

        /// <summary>
        /// Tests that the root is populated after loading 
        /// </summary>
        [Test]
        public void TestRootNotNullWithMatchingProperty() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if no matching property is found. It
        /// will then get the first property in the document.
        /// </summary>
        [Test]
        public void TestRootNotNullWithoutMatchingProperty() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""This.Type.Does.Not.Exist"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if the resulting document is empty.
        /// </summary>
        [Test]
        public void TestRootNotNullEmptyFile() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root is still populated even if the first property is not an array
        /// </summary>
        [Test]
        public void TestRootNotNullNotArray() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""This.Type.Does.Not.Exist"": { ""Name"":""Phogue"", ""Age"": 100  } }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that the root element is set if a property exists with the same filename.
        /// </summary>
        [Test]
        public void TestRootSetToFilePath() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""Name"":""Phogue"", ""Age"": 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.AreEqual("Phogue", config.Root.First["Name"].Value<String>());
            Assert.AreEqual(100, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests that invalid json will result in an empty document/root
        /// </summary>
        [Test]
        public void TestInvalidJsonParsingNotNullRoot() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""N 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

            Assert.IsNotNull(config.Root);
        }

        /// <summary>
        /// Tests that invalid json will result in an empty document/root
        /// </summary>
        [Test]
        public void TestInvalidJsonParsingNotNullDocument() {
            File.WriteAllText(this.ConfigFileA.FullName, @"{ ""Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"": [ { ""N 100  } ] }");

            IConfig config = new Config().Load(this.ConfigFileA);

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
