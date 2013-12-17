using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Database.Drivers;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Test {
    [TestFixture]
    public class TestConnection {
        [Test]
        [Ignore]
        public void TestMethod1() {
            IDriver driver = new MySqlDriver() {
                Settings = new DriverSettings() {
                    Hostname = "localhost",
                    Username = "root",
                    Password = "",
                    Port = 3306,
                    Database = "test_connection"
                }
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

            List<IDatabaseObject> result = driver.Query(new Find().Collection("player").Condition("Name", "Phogue"));

            driver.Close();
        }

        [Test]
        [Ignore]
        public void TestMethod2() {
            IDriver driver = new MySqlDriver() {
                Settings = new DriverSettings() {
                    Hostname = "localhost",
                    Username = "root",
                    Password = "",
                    Port = 3306,
                    Database = "test_connection"
                }
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

            CollectionValue result = driver.Query(new Find().Collection("player").Condition("Name", "Phogue").Assignment("Score", 99)) as CollectionValue;

            JArray array = result.ToJArray();

            foreach (JObject obj in result.ToJArray()) {
                var x = 0;
            }

            driver.Close();
        }

        [Test]
        [Ignore]
        public void TestMethod3() {
            IDriver driver = new MongoDbDriver() {
                Settings = new DriverSettings() {
                    Hostname = "localhost",
                    Database = "chataway"
                }
            };

            driver.Connect();

            CollectionValue result = driver.Query(new Create().Collection("posts").Index("Player", "Name", new Primary())) as CollectionValue;

            JArray array = result.ToJArray();

            String kkll = array.ToString();

            foreach (JObject obj in result.ToJArray()) {
                var x = 0;
            }

            driver.Close();
        }

        [Test]
        [Ignore]
        public void TestMethod4() {
            IDriver driver = new SqLiteDriver() {
                Settings = new DriverSettings() {
                    Database = "test_connection"
                }
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

            //CollectionValue result = driver.Query(new Create().Collection("Player").Field("Name").Field("Score").Index("player", "Name")) as CollectionValue;


            List<IDatabaseObject> result = driver.Query(new Find().Collection("Player").Condition("Score", 100));

            JArray array = ((CollectionValue)result.First()).ToJArray();

            driver.Close();
        }

    }
}
