using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Procon.Database.Drivers;
using Procon.Database.Shared;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Database.Test.Integration.SqLite {
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
