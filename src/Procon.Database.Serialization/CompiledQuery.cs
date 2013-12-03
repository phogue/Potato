using System;
using System.Collections.Generic;

namespace Procon.Database.Serialization {
    /// <summary>
    /// A compiled version of the query with as basic information 
    /// as we can boil the complex query down to
    /// </summary>
    [Serializable]
    public class CompiledQuery : ICompiledQuery {
        /// <summary>
        /// List of compiled child queries.
        /// </summary>
        public List<ICompiledQuery> Children { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        public IDatabaseObject Root { get; set; }

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
        /// The fields to set during the query
        /// </summary>
        public String Assignments { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        public String Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        public String Collections { get; set; }

        /// <summary>
        /// How to order the results
        /// </summary>
        public String Sortings { get; set; }
    }
}
