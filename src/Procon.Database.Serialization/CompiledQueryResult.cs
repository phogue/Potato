using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Procon.Database.Serialization {

    /// <summary>
    /// Stores the results of a query before being serialized to json.
    /// </summary>
    public class CompiledQueryResult {

        /// <summary>
        /// The number of rows returned or modified
        /// </summary>
        public int AffectedRows { get; set; }

        /// <summary>
        /// The rows returned from a find query
        /// </summary>
        public JArray Rows { get; set; }

        public CompiledQueryResult() {
            this.AffectedRows = 0;
            this.Rows = new JArray();
        }

        /// <summary>
        /// Serializes this object into a string
        /// </summary>
        /// <returns></returns>
        public String Serialize() {
            return new JObject() {
                new JProperty("AffectedRows", this.AffectedRows),
                new JProperty("Rows", this.Rows)
            }.ToString(Formatting.None);
        }
    }
}
