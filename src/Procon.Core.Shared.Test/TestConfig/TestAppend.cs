using System;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Shared.Test.TestConfig.Mocks;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestAppend : TestConfigBase {
        /// <summary>
        /// Tests data can be appended to the root 
        /// </summary>
        [Test]
        public void TestAppendedToRoot() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();
            config.Append(new MockSimpleConcrete() {
                Name = "Phogue",
                Age = 100
            });

            Assert.AreEqual("Phogue", config.Root.First["Name"].Value<String>());
            Assert.AreEqual(100, config.Root.First["Age"].Value<int>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into union.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            new JsonConfig().Append<MockSimpleConcrete>(null);
        }
    }
}
