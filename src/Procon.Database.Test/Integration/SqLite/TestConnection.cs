using NUnit.Framework;
using Procon.Database.Drivers;

namespace Procon.Database.Test.Integration.SqLite {
    [TestFixture]
    public class TestConnection {
        /// <summary>
        /// Tests we can load up an in-memory SQLite database
        /// </summary>
        [Test]
        public void TestBasicConnection() {
            IDriver driver = new SqLiteDriver() {
                Settings = new DriverSettings() {
                    Memory = true
                }
            };

            Assert.IsTrue(driver.Connect());

            driver.Close();
        }
    }
}
