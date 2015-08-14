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

using System.Linq;
using NUnit.Framework;
using Potato.Core.Events;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.TestPotato {
    [TestFixture]
    public class TestPotatoServiceRestart {
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
            var instance = new PotatoController();

            var result = instance.Tunnel(CommandBuilder.PotatoServiceRestart().SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        /// Tests that with permissions the result will be a success.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            var instance = new PotatoController();

            var result = instance.Tunnel(CommandBuilder.PotatoServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        /// Tests that a service message is set when successfully executing a restart command.
        /// </summary>
        [Test]
        public void TestMessageLogged() {
            var instance = new PotatoController();

            instance.Tunnel(CommandBuilder.PotatoServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsNotNull(instance.ServiceMessage);
            Assert.AreEqual("restart", instance.ServiceMessage.Name);

            instance.Dispose();
        }

        /// <summary>
        /// Tests that an event is logged for a restart when successfully executing a restart command.
        /// </summary>
        [Test]
        public void TestEventLogged() {
            var events = new EventsController();
            var instance = new PotatoController {
                Shared = {
                    Events = events
                }
            };

            instance.Tunnel(CommandBuilder.PotatoServiceRestart().SetOrigin(CommandOrigin.Local));

            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("PotatoServiceRestarting", events.LoggedEvents.First().Name);

            instance.Dispose();
        }
    }
}
