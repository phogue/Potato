using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Procon.Database.Drivers;
using Procon.Database.Shared;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Methods.Schema;
using Procon.Database.Shared.Builders.Modifiers;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Database.Test {
    /// <summary>
    /// I keep this class around to perform local tests on real databases. It was helpful
    /// just to test on a real database when the Procon.Database project was first created.
    /// </summary>
    public class TestConnection {
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

            CollectionValue result = driver.Query(new Find().Collection("player").Condition("Name", "Phogue").Set("Score", 99)) as CollectionValue;

            JArray array = result.ToJArray();

            foreach (JObject obj in result.ToJArray()) {
                var x = 0;
            }

            driver.Close();
        }

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
