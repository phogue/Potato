using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Events;
using Procon.Core.Repositories;
using Procon.Core.Utils;
using Procon.Net.Utils;

namespace Procon.Core.Test.Repositories {
    [TestClass]
    public class TestRepositoryControllerExecute {

        protected static String ExecuteInstalledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed");
        protected static String ExecutePackagesInstalledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed\Packages");
        protected static String ExecuteUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed\Updates");
        protected static String ExecutePackagesUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed\Updates\Packages");
        protected static String ExecuteTemporaryUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed\Updates\Temporary");

        [TestInitialize]
        public void Initialize() {
            Directory.CreateDirectory(ExecuteInstalledPath);
            Directory.CreateDirectory(ExecutePackagesInstalledPath);
            Directory.CreateDirectory(ExecuteUpdatesPath);
            Directory.CreateDirectory(ExecutePackagesUpdatesPath);
            Directory.CreateDirectory(ExecuteTemporaryUpdatesPath);
             
            // Remove all files and directories within the execute installed path for each test.
            new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Execute\Installed")).Clean();
        }

        protected FlatPackedPackage SetupAndSaveExecutePackage(String path, String repositoryUrlSlug) {
            XElement element = XElement.Parse(@"<package>
    <uid>DownloadCacheTest</uid>
    <name>DownloadCacheTest</name>
    <package_versions>
        <package_version>
            <version>
                <major>1</major>
                <minor>2</minor>
                <build>3</build>
                <revision>4</revision>
            </version>
            <files>
                <file>
                    <name>ThisIsInASubDirectory.txt</name>
                    <size>89</size>
                    <date>1341275726</date>
                    <relative_path>plugins\ThisIsInASubDirectory.txt</relative_path>
                    <md5>cbee0eff10c65e6bc4369fbea92df09e</md5>
                    <last_modified>2012-07-03T10:05:26+09:30</last_modified>
                </file>
                <file>
                    <name>ThisFileIsIdentical.txt</name>
                    <size>43</size>
                    <date>1341275647</date>
                    <relative_path>ThisFileIsIdentical.txt</relative_path>
                    <md5>01952484abdda0158c827a8848528e90</md5>
                    <last_modified>2012-07-03T10:04:07+09:30</last_modified>
                </file>
            </files>
        </package_version>
    </package_versions>
</package>");

            FlatPackedPackage package = element.FromXElement<FlatPackedPackage>();

            package.PackagesUpdatesPath = path;

            package.Repository = new Repository() {
                UrlSlug = repositoryUrlSlug
            };

            package.Save();

            return package;
        }

        protected void ValidateExecutePackage(Package package) {
            Assert.AreEqual("DownloadCacheTest", package.Uid);
            Assert.AreEqual("DownloadCacheTest", package.Name);

            Assert.AreEqual(new Version(1, 2, 3, 4), package.Versions[0].Version.SystemVersion);

            Assert.AreEqual("ThisIsInASubDirectory.txt", package.Versions[0].Files[0].Name);
            Assert.AreEqual(89, package.Versions[0].Files[0].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 5, 26), package.Versions[0].Files[0].LastModified);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", package.Versions[0].Files[0].RelativePath);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", package.Versions[0].Files[0].Md5);

            Assert.AreEqual("ThisFileIsIdentical.txt", package.Versions[0].Files[1].Name);
            Assert.AreEqual(43, package.Versions[0].Files[1].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 4, 7), package.Versions[0].Files[1].LastModified);
            Assert.AreEqual(@"ThisFileIsIdentical.txt", package.Versions[0].Files[1].RelativePath);
            Assert.AreEqual("01952484abdda0158c827a8848528e90", package.Versions[0].Files[1].Md5);
        }

        protected Core.Repositories.RepositoryController SetupRepositoryController() {
            return new Core.Repositories.RepositoryController() {
                PackagesPath = ExecutePackagesInstalledPath,
                PackagesUpdatesPath = ExecutePackagesUpdatesPath
            };
        }

        /// <summary>
        /// Tests that existing installed packages are loaded up in the controller.
        /// </summary>
        [TestMethod]
        public void TestRepositoryControllerInstalledRepository() {
            this.SetupAndSaveExecutePackage(ExecutePackagesInstalledPath, "repomyrconcom_procon2");

            Core.Repositories.RepositoryController repositoryController = this.SetupRepositoryController();

            repositoryController.Execute();

            Repository locallyInstalledRepository = repositoryController.LocalInstalledRepositories.First();
            
            this.ValidateExecutePackage(locallyInstalledRepository.Packages.First());
        }
        
        /// <summary>
        /// Tests that existing updates packages are loaded up in the controller.
        /// </summary>
        [TestMethod]
        public void TestRepositoryControllerUpdatedRepository() {
            this.SetupAndSaveExecutePackage(ExecutePackagesUpdatesPath, "repomyrconcom_procon2");

            Core.Repositories.RepositoryController repositoryController = this.SetupRepositoryController();

            repositoryController.Execute();

            Repository locallyInstalledRepository = repositoryController.LocalUpdatedRepositories.First();

            this.ValidateExecutePackage(locallyInstalledRepository.Packages.First());
        }
        
        /// <summary>
        /// Tests that existing updates packages are loaded up in the controller.
        /// </summary>
        [TestMethod]
        public void TestRepositoryControllerRemoteRepository() {
            AutoResetEvent requestWait = new AutoResetEvent(false);

            RepositoryController repositoryController = this.SetupRepositoryController();

            EventsController eventsController = new EventsController();

            repositoryController.Events = eventsController;

            repositoryController.Execute(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            eventsController.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            Assert.IsTrue(requestWait.WaitOne(60000));

            Assert.IsTrue(repositoryController.RemoteRepositories.First().Packages.Count > 0);
        }
    }
}
