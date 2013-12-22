#region

using NUnit.Framework;
using Procon.Core.Repositories;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestRepositoryPackageReference {
        /// <summary>
        ///     Tests that a uid is sanatized. We test that the process is done here
        ///     but the sanitize methods are tested elsewhere.
        /// </summary>
        [Test]
        public void TestPackageUid() {
            var reference = new RepositoryPackageReference() {
                PackageUid = "MyPackageUid!"
            };

            Assert.AreEqual("MyPackageUid", reference.PackageUid);
        }

        /// <summary>
        ///     Tests that a url is slugged. We test that the process is done here
        ///     but the sanitize methods are tested elsewhere.
        /// </summary>
        [Test]
        public void TestRepositoryUrlSlug() {
            var reference = new RepositoryPackageReference() {
                RepositoryUrlSlug = "http://repo.myrcon.com/procon2/"
            };

            Assert.AreEqual("repomyrconcom_procon2", reference.RepositoryUrlSlug);
        }
    }
}