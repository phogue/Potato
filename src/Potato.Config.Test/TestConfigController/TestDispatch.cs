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
using NUnit.Framework;
using Potato.Config.Core;

namespace Potato.Config.Test.TestConfigController {
    [TestFixture]
    public class TestDispatch {
        /// <summary>
        /// Tests a null result when passing in a nulled command
        /// </summary>
        [Test]
        public void TestNulledResultForNulledCommand() {
            Assert.IsNull(ConfigController.Dispatch(null, null));
        }

        /// <summary>
        /// Tests a null result from passing in an unknown command
        /// </summary>
        [Test]
        public void TestNulledResultForUnknownCommand() {
            Assert.IsNull(ConfigController.Dispatch("This command does not exist", null));
        }
    }
}
