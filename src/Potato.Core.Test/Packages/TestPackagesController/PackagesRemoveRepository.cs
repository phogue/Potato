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
    public class PackagesRemoveRepository {
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
            var packages = new PackagesController();

            var result = packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests removing a non-existant uri will still result in a success
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            var packages = new PackagesController();

            var result = packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests removing a uri will fire a repository removed event
        /// </summary>
        [Test]
        public void TestResultSuccessEventListed() {
            var wait = new AutoResetEvent(false);

            var packages = (PackagesController)new PackagesController().Execute();

            packages.Shared.Events.EventLogged += (sender, @event) => {
                if (@event.GenericEventType == GenericEventType.PackagesRepositoryRemoved && @event.Then.Repositories.First().Uri == "https://teamcity.myrcon.com/nuget") {
                    wait.Set();
                }
            };

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(wait.WaitOne(5000));
        }

        /// <summary>
        /// Tests adding a uri then removing it will result in only the orphanage remaining
        /// </summary>
        [Test]
        public void TestSingleRepositoryAddedThenRemoved() {
            var packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(1, packages.Cache.Repositories.Count);
            Assert.IsTrue(packages.Cache.Repositories.First().IsOrphanage);
        }

        /// <summary>
        /// Tests removing the same uri twice still results in a success
        /// </summary>
        [Test]
        public void TestDoubleRepositorySetup() {
            var packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(1, packages.Cache.Repositories.Count);
            Assert.IsTrue(packages.Cache.Repositories.First().IsOrphanage);
        }

        /// <summary>
        /// Tests two unique repositories added and removed will result in only the orphanage
        /// </summary>
        [Test]
        public void TestSuccessTwoUniqueRepositoriesAdded() {
            var packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget2").SetOrigin(CommandOrigin.Local));

            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget2").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(1, packages.Cache.Repositories.Count);
        }

        /// <summary>
        /// Tests that two repositories added but only one removed will result in only one repository being saved
        /// </summary>
        [Test]
        public void TestSuccessOneRepositorySaved() {
            var packages = (PackagesController)new PackagesController().Execute();

            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));
            packages.Tunnel(CommandBuilder.PackagesAppendRepository("https://teamcity.myrcon.com/nuget2").SetOrigin(CommandOrigin.Local));

            packages.Tunnel(CommandBuilder.PackagesRemoveRepository("https://teamcity.myrcon.com/nuget1").SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(2, packages.Cache.Repositories.Count);

            Assert.AreEqual(1, packages.Shared.Variables.ArchiveVariables.First(archive => archive.Key.ToLower() == CommonVariableNames.PackagesConfigGroups.ToString().ToLower()).Value.ToList<string>().Count);
        }
    }
}
