using NUnit.Framework;
using Procon.Core.Test.Database.Helpers;

namespace Procon.Core.Test.Database {
    [TestFixture]
    public class TestDatabaseMigrationController {
        /// <summary>
        /// Tests that the initial migration (with no entries in the migrations table) will
        /// still call all the migrations.
        /// </summary>
        [Test]
        public void TestMigrationUpFromNothing() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.Up();

            Assert.AreEqual(4, helper.Counter);
            Assert.AreEqual(4, helper.Tracker);
        }

        /// <summary>
        /// Tests we can specify what migration to go to
        /// </summary>
        [Test]
        public void TestMigrationUpFromNothingToSecond() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.Up(2);

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(2, helper.Tracker);
        }

        /// <summary>
        /// Tests that migrations from a second migration can go to the latest migration.
        /// </summary>
        [Test]
        public void TestMigrationUpFromSecond() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(2);

            helper.Migrations.Up();

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(4, helper.Tracker);
        }

        /// <summary>
        /// Tests that migrations from the latest migration will yield no migration calls
        /// </summary>
        [Test]
        public void TestMigrationUpFromLatest() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Up();

            Assert.AreEqual(0, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        /// Tests that all down migrations are called when on the latest migration,
        /// essentially this would be used to "uninstall"
        /// </summary>
        [Test]
        public void TestMigrationDownFromLatest() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Down();

            Assert.AreEqual(4, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        /// Tests we can specify what migration to go to
        /// </summary>
        [Test]
        public void TestMigrationDownFromLatestToSecond() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(4);

            helper.Migrations.Down(2);

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(2, helper.Tracker);
        }

        /// <summary>
        /// Tests that migrations from a second migration can go to the first migration.
        /// </summary>
        [Test]
        public void TestMigrationDownFromSecond() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.SaveVersion(2);

            helper.Migrations.Down();

            Assert.AreEqual(2, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }

        /// <summary>
        /// Tests that migrations from no migrations will yield no migration calls
        /// </summary>
        [Test]
        public void TestMigrationDownFromNothing() {
            TestMigrationTrackerHelper helper = new TestMigrationTrackerHelper();

            helper.Migrations.Down();

            Assert.AreEqual(0, helper.Counter);
            Assert.AreEqual(0, helper.Tracker);
        }
    }
}
