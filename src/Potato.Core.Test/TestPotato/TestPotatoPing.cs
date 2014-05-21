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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.TestPotato {
    [TestFixture]
    class TestPotatoPing {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            PotatoController instance = new PotatoController();

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoPing().SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        /// Tests that given permission the command will return as successful
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            PotatoController instance = new PotatoController();

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoPing().SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        /// Tests that an offset from the time started up until the command will be returned in milliseconds.
        /// </summary>
        [Test]
        public void TestResultSuccessUptimeReturned() {
            PotatoController instance = new PotatoController() {
                InstantiatedStamp = DateTime.Now.AddSeconds(-5)
            };

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoPing().SetOrigin(CommandOrigin.Local));

            Assert.GreaterOrEqual(int.Parse(result.Now.Content.First()), 5000);

            instance.Dispose();
        }
    }
}
