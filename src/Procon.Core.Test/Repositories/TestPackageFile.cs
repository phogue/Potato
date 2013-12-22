#region

using System;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Repositories;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestPackageFile {
        /// <summary>
        ///     Tests that a file can be deserialized from static xml collected from the repository
        /// </summary>
        [Test, Ignore]
        
        public void TestPackageFileDeserialization() {
            XElement element = XElement.Parse(@"<file>
    <name>ThisIsInASubDirectory.txt</name>
    <size>89</size>
    <date>1341275726</date>
    <relative_path>plugins\ThisIsInASubDirectory.txt</relative_path>
    <md5>cbee0eff10c65e6bc4369fbea92df09e</md5>
    <last_modified>2012-07-03T10:05:26+09:30</last_modified>
</file>");

            var packageFile = element.FromXElement<PackageFile>();

            Assert.AreEqual("ThisIsInASubDirectory.txt", packageFile.Name);
            Assert.AreEqual(89, packageFile.Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 5, 26), packageFile.LastModified);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", packageFile.RelativePath);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", packageFile.Md5);
        }
    }
}