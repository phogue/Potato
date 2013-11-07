using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Database.Serialization {
    public class CompiledQuery : ICompiledQuery {

        /// <summary>
        /// The compiled query, used for SQL queries.
        /// </summary>
        public String Completed { get; set; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        public String Method { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        public List<String> Fields { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        public String Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        public String Collections { get; set; }
    }
}
