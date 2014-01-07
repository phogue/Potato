using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Procon.Core.Packages;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;

namespace Procon.Core.Test.Packages {
    /// <summary>
    /// Tests the packages controller's basic functionality, setting up remote repositories, polling etc.
    /// </summary>
    /// <remarks>
    ///     <para>While we use Nuget packages for testing we do not validate any of Nuget's processes</para>
    /// </remarks>
    [TestFixture]
    public class TestPackagesController {
        /// <summary>
        /// Opens a repository (local one) as source.
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static PackagesController OpenMockSourceRepository(String @namespace = "") {
            var variables = new VariableController();

            var packages = new PackagesController() {
                Shared = {
                    Variables = variables
                }
            }.Execute() as PackagesController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(@namespace, CommonVariableNames.PackagesRepositoryUri), "path to repository fake remote stuff");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.PackagesConfigGroups, new List<String>() {
                @namespace
            });

            return packages;
        }

        [Test]
        public void TestUninstalledPackage() {
            PackagesController packages = TestPackagesController.OpenMockSourceRepository();


        }
    }
}
