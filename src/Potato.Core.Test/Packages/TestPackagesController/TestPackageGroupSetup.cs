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
using NUnit.Framework;
using Potato.Core.Packages;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;

namespace Potato.Core.Test.Packages.TestPackagesController {
    /// <summary>
    /// Tests the packages controller's basic functionality, setting up remote repositories, polling etc.
    /// </summary>
    /// <remarks>
    ///     <para>While we use Nuget packages for testing we do not validate any of Nuget's processes</para>
    /// </remarks>
    [TestFixture]
    public class TestPackageGroupSetup {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests the grouped repository setting can be setup via variables.
        /// </summary>
        [Test]
        public void TestPackageGroupSetupLinear() {
            var variables = new VariableController();

            var @namespace = "";

            var packages = new PackagesController() {
                Shared = {
                    Variables = variables
                }
            }.Execute() as PackagesController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(@namespace, CommonVariableNames.PackagesRepositoryUri), "path-to-repository");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.PackagesConfigGroups, new List<String>() {
                @namespace
            });

            Assert.IsNotEmpty(packages.Cache.Repositories);
            Assert.AreEqual("path-to-repository", packages.Cache.Repositories.FirstOrDefault().Uri);
        }

        /// <summary>
        /// Tests the grouped repository setting can be setup via variables, with the group being
        /// set first then the group name added to the known group spaces
        /// </summary>
        [Test]
        public void TestPackageGroupSetupOrderReversed() {
            var variables = new VariableController();

            var @namespace = "";

            var packages = new PackagesController() {
                Shared = {
                    Variables = variables
                }
            }.Execute() as PackagesController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.PackagesConfigGroups, new List<String>() {
                @namespace
            });

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(@namespace, CommonVariableNames.PackagesRepositoryUri), "path-to-repository");

            Assert.IsNotEmpty(packages.Cache.Repositories);
            Assert.AreEqual("path-to-repository", packages.Cache.Repositories.FirstOrDefault().Uri);
        }

        /// <summary>
        /// Tests variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestDispose() {
            PackagesController packages = new PackagesController();

            packages.Dispose();

            Assert.IsNull(packages.GroupedVariableListener);
            Assert.IsNull(packages.LocalRepository);
            Assert.IsNull(packages.Cache);
        }
    }
}
