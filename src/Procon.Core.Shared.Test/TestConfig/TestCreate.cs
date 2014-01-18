using System;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Shared.Test.TestConfig.Mocks;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestCreate : TestConfigBase {
        /// <summary>
        /// Tests that the namespace is setup correctly.
        /// </summary>
        [Test]
        public void TestStructureCreated() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            Assert.IsNotNull(config.Document);
            Assert.IsNotNull(config.Document["Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"]);
            Assert.IsNotNull(config.Document["Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].Value<JArray>());
        }

        /// <summary>
        /// Tests the root is set to the maximum document value.
        /// </summary>
        [Test]
        public void TestRootMatchesNamespace() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            Assert.AreEqual(config.Document["Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"], config.Root);
        }
    }
}
