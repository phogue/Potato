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
using Potato.Core.Remote;

namespace Potato.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestDispose {
        /// <summary>
        ///     Tests variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestCommandServerDisposed() {
            var commandServer = new CommandServerController();

            commandServer.Dispose();

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNull(commandServer.TunnelObjects);
        }
    }
}
