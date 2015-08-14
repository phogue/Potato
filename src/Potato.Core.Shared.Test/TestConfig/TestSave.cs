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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Potato.Core.Shared.Test.TestConfig {
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
            IConfig config = new Config();

            config.Save(ConfigFileA);

            ConfigFileA.Refresh();

            Assert.IsTrue(ConfigFileA.Exists);
        }

        /// <summary>
        /// Tests that a single item in the config will be saved to file.
        /// </summary>
        [Test]
        public void TestSimpleDocumentCreatesFile() {
            IConfig config = new Config() {
                Document = new JObject() {
                    new JProperty("Hello", "World!")
                }
            };

            config.Save(ConfigFileA);

            ConfigFileA.Refresh();

            Assert.IsTrue(ConfigFileA.Exists);
        }

        /// <summary>
        /// Tests that a single item in the config will be saved to file can be loaded once again.
        /// </summary>
        [Test]
        public void TestSimpleDocumentCanDeserialize() {
            IConfig config = new Config() {
                Document = new JObject() {
                    new JProperty("Hello", "World!")
                }
            };

            config.Save(ConfigFileA);

            var deseralized = JsonConvert.DeserializeObject(File.ReadAllText(ConfigFileA.FullName)) as JObject;

            Assert.IsNotNull(deseralized);
            Assert.AreEqual("World!", deseralized["Hello"].Value<string>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into save.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            IConfig config = new Config();

            config.Save(null);
        }
    }
}
