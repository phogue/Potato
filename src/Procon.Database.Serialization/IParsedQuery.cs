using System;
using System.Collections.Generic;

namespace Procon.Database.Serialization {
    public interface IParsedQuery {
        /// <summary>
        /// List of parsed child queries.
        /// </summary>
        List<IParsedQuery> Children { get; set; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        List<String> Methods { get; set; }

        /// <summary>
        /// The databases to query against
        /// </summary>
        List<String> Databases { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        List<String> Fields { get; set; }

        /// <summary>
        /// The fields used to when assigning a value to a field (update, insert)
        /// </summary>
        List<String> Assignments { get; set; }

        /// <summary>
        /// The indices to apply when the alter/create
        /// </summary>
        List<String> Indices { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        List<String> Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        List<String> Collections { get; set; }

        /// <summary>
        /// What fields and direction to sort by.
        /// </summary>
        List<String> Sortings { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        IDatabaseObject Root { get; set; }
    }
}
