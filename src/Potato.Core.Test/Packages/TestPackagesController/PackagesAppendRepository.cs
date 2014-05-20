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
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Potato.Core.Packages;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Packages.TestPackagesController {
    public class PackagesAppendRepository {
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
            PackagesController packages = new PackagesController();

            ICommandResult result = packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests adding a uri will setup the repository to have packages fetched.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            PackagesController packages = new PackagesController();

            ICommandResult result = packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests adding a uri will fire a repository added event
        /// </summary>
        [Test]
        public void TestResultSuccessEventListed() {
            var wait = new AutoResetEvent(false);

            PackagesController packages = (PackagesController)new PackagesController().Execute();

            packages.Shared.Events.EventLogged += (sender, @event) => {
                if (@event.GenericEventType == GenericEventType.PackagesRepositoryAppended && @event.Now.Repositories.First().Uri == "https://teamcity.myrcon.com/nuget") {
                    wait.Set();
                }
            };

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(wait.WaitOne(5000));
        }

        /// <summary>
        /// Tests adding a uri will setup the repository to have packages fetched.
        /// </summary>
        [Test]
        public void TestSingleRepositorySetup() {
            PackagesController packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(2, packages.Cache.Repositories.Count);
            Assert.AreEqual("https://teamcity.myrcon.com/nuget", packages.Cache.Repositories.First().Uri);
        }

        /// <summary>
        /// Tests adding the same uri twice will still result in only a single uri being added.
        /// </summary>
        [Test]
        public void TestDoubleRepositorySetup() {
            PackagesController packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(2, packages.Cache.Repositories.Count);
        }

        /// <summary>
        /// Tests that two repository unique url's will both be setup for their packages to be fetched.
        /// </summary>
        [Test]
        public void TestSuccessTwoUniqueRepositoriesAdded() {
            PackagesController packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget2").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(3, packages.Cache.Repositories.Count);
        }

        /// <summary>
        /// Tests that two end points are saved to the config
        /// </summary>
        [Test]
        public void TestSuccessTwoRepositoriesSaved() {
            PackagesController packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget2").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(3, packages.Cache.Repositories.Count);
            
            Assert.AreEqual(2, packages.Shared.Variables.ArchiveVariables.First(archive => archive.Key.ToLower() == CommonVariableNames.PackagesConfigGroups.ToString().ToLower()).Value.ToList<String>().Count);
        }
    }
}
