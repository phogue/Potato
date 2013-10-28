using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Repositories;

namespace Procon.Core.Test.Repositories {
    [TestClass]
    public class TestRepositoryPackageReference {

        /// <summary>
        /// Tests that a url is slugged. We test that the process is done here
        /// but the sanitize methods are tested elsewhere.
        /// </summary>
        [TestMethod]
        public void TestRepositoryUrlSlug() {
            RepositoryPackageReference reference = new RepositoryPackageReference() {
                RepositoryUrlSlug = "http://repo.myrcon.com/procon2/"
            };

            Assert.AreEqual("repomyrconcom_procon2", reference.RepositoryUrlSlug);
        }

        /// <summary>
        /// Tests that a uid is sanatized. We test that the process is done here
        /// but the sanitize methods are tested elsewhere.
        /// </summary>
        [TestMethod]
        public void TestPackageUid() {
            RepositoryPackageReference reference = new RepositoryPackageReference() {
                PackageUid = "MyPackageUid!"
            };

            Assert.AreEqual("MyPackageUid", reference.PackageUid);
        }
    }
}
