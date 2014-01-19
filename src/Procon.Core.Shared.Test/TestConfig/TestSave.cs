using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared.Test.TestConfig {
    /// <summary>
    /// Tests saving a config, but assumes the underlying Json framework has been tested.
    /// </summary>
    [TestFixture]
    public class TestSave : TestConfigBase {
        /// <summary>
        /// Tests that an empty config will save an empty file, but the file will be created
        /// </summary>
        [Test]
        public void TestEmptyDocumentCreatesFile() {
            IConfig config = new JsonConfig();

            config.Save(this.ConfigFileA);

            this.ConfigFileA.Refresh();

            Assert.IsTrue(this.ConfigFileA.Exists);
        }

        /// <summary>
        /// Tests that a single item in the config will be saved to file.
        /// </summary>
        [Test]
        public void TestSimpleDocumentCreatesFile() {
            IConfig config = new JsonConfig() {
                Document = new JObject() {
                    new JProperty("Hello", "World!")
                }
            };

            config.Save(this.ConfigFileA);

            this.ConfigFileA.Refresh();

            Assert.IsTrue(this.ConfigFileA.Exists);
        }

        /// <summary>
        /// Tests that a single item in the config will be saved to file can be loaded once again.
        /// </summary>
        [Test]
        public void TestSimpleDocumentCanDeserialize() {
            IConfig config = new JsonConfig() {
                Document = new JObject() {
                    new JProperty("Hello", "World!")
                }
            };

            config.Save(this.ConfigFileA);

            JObject deseralized = JsonConvert.DeserializeObject(File.ReadAllText(this.ConfigFileA.FullName)) as JObject;

            Assert.IsNotNull(deseralized);
            Assert.AreEqual("World!", deseralized["Hello"].Value<String>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into save.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            IConfig config = new JsonConfig();

            config.Save(null);
        }
    }
}
