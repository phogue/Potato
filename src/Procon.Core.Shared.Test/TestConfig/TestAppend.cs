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
            IConfig config = new Config().Create<MockSimpleConcrete>();
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
            new Config().Append<MockSimpleConcrete>(null);
        }
    }
}
