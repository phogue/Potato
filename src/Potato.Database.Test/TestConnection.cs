#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Potato.Database.Drivers;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Test {
    /// <summary>
    /// I keep this class around to perform local tests on real databases. It was helpful
    /// just to test on a real database when the Potato.Database project was first created.
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
