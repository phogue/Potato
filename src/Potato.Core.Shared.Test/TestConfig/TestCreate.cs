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
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Potato.Core.Shared.Test.TestConfig.Mocks;

namespace Potato.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestCreate : TestConfigBase {
        /// <summary>
        /// Tests that the namespace is setup correctly.
        /// </summary>
        [Test]
        public void TestStructureCreated() {
            IConfig config = new Config().Create<MockSimpleConcrete>();

            Assert.IsNotNull(config.Document);
            Assert.IsNotNull(config.Document["Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"]);
            Assert.IsNotNull(config.Document["Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"].Value<JArray>());
        }

        /// <summary>
        /// Tests the root is set to the maximum document value.
        /// </summary>
        [Test]
        public void TestRootMatchesNamespace() {
            IConfig config = new Config().Create<MockSimpleConcrete>();

            Assert.AreEqual(config.Document["Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete"], config.Root);
        }
    }
}
