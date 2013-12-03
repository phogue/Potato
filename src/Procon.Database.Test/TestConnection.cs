using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Database.Drivers;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Test {
    [TestFixture]
    public class TestConnection {
        [Test]
        public void TestMethod1() {
            IDriver driver = new MySqlDriver() {
                Hostname = "localhost",
                Username = "root",
                Password = "",
                Port = 3306,
                Database = "test_connection"
            };

            driver.Connect();

            /*
            driver.Query(
                new Save()
                .Collection("player")
                .Assignment("Name", "Phogue")
                .Assignment("Rank", 10)
                .Assignment("Score", 50)
            );
            */

            IDatabaseObject result = driver.Query(new Find().Collection("player").Condition("Name", "Phogue"));

            driver.Close();
        }

        [Test]
        public void TestMethod2() {
            IDriver driver = new MySqlDriver() {
                Hostname = "localhost",
                Username = "root",
                Password = "",
                Port = 3306,
                Database = "test_connection"
            };

            driver.Connect();

            /*
            driver.Query(
                new Save()
                .Collection("player")
                .Assignment("Name", "Phogue")
                .Assignment("Rank", 10)
                .Assignment("Score", 50)
            );
            */

            IDatabaseObject result = driver.Query(new Find().Collection("player").Condition("Name", "Phogue").Assignment("Score", 99));

            driver.Close();
        }
    }
}
