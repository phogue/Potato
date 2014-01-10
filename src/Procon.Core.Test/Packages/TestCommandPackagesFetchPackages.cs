using NUnit.Framework;
using Procon.Core.Packages;
using Procon.Core.Shared;

namespace Procon.Core.Test.Packages {
    [TestFixture]
    public class TestCommandPackagesFetchPackages {
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

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesFetchPackages().SetOrigin(CommandOrigin.Remote).SetUsername("Phogue"));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that the packages can be fetched (or initiated a fetch) using the command with permissions.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            PackagesController packages = new PackagesController();

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesFetchPackages().SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }
    }
}
