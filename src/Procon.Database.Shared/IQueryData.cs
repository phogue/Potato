using System.Collections.Generic;

namespace Procon.Database.Shared {
    /// <summary>
    /// The base shared data between Compiled and Parsed data.
    /// </summary>
    public interface IQueryData {
        /// <summary>
        /// The number of documents to read or edit before ignoring.
        /// </summary>
        long? Limit { get; set; }

        /// <summary>
        /// The number of documents to ignore before reading or editing.
        /// </summary>
        long? Skip { get; set; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        List<string> Methods { get; set; }

        /// <summary>
        /// The databases to query against
        /// </summary>
        List<string> Databases { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        List<string> Fields { get; set; }

        /// <summary>
        /// The values to assign to fields
        /// </summary>
        List<string> Values { get; set; }

        /// <summary>
        /// The fields used to when assigning a value to a field (update, insert)
        /// </summary>
        List<string> Assignments { get; set; }

        /// <summary>
        /// The indices to apply when the alter/create
        /// </summary>
        List<string> Indices { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        List<string> Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        List<string> Collections { get; set; }

        /// <summary>
        /// What fields and direction to sort by.
        /// </summary>
        List<string> Sortings { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        IDatabaseObject Root { get; set; }
    }
}
