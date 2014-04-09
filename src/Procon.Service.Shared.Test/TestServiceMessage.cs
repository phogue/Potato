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

namespace Procon.Service.Shared.Test {
    [TestFixture]
    public class TestServiceMessage {
        /// <summary>
        /// Tests the initial properties are setup
        /// </summary>
        [Test]
        public void TestInitialValues() {
            var message = new ServiceMessage();

            Assert.IsNotNull(message.Arguments);
            Assert.IsNotNull(message.Stamp);
        }

        /// <summary>
        /// Tests values are nulled out on dispose
        /// </summary>
        [Test]
        public void TestDispose() {
            var message = new ServiceMessage();

            message.Dispose();

            Assert.IsNull(message.Arguments);
            Assert.IsNull(message.Name);
        }
    }
}
