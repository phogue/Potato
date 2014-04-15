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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Potato.Database.Drivers;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Test.Integration.SqLite {
    public class SqLiteDatabaseIntegration : IDatabaseIntegration {
        /// <summary>
        /// Tests a query on an in memory sql lite
        /// </summary>
        /// <param name="setup">A list of queries to run prior to running a find query</param>
        /// <param name="query"></param>
        /// <param name="expected"></param>
        public void TestFindQuery(IEnumerable<IDatabaseObject> setup, IDatabaseObject query, JArray expected) {
            IDriver driver = new SqLiteDriver() {
                Settings = new DriverSettings() {
                    Memory = true
                }
            };

            driver.Connect();

            foreach (var item in setup) {
                driver.Query(item);
            }

            List<IDatabaseObject> results = driver.Query(query);

            Assert.AreEqual(expected.ToString(Formatting.None), ((CollectionValue)results.First()).ToJArray().ToString(Formatting.None));

            driver.Close();
        }
    }
}
