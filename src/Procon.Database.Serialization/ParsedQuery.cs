using System;
using System.Collections.Generic;

namespace Procon.Database.Serialization {
    public class ParsedQuery : IParsedQuery {
        /// <summary>
        /// List of parsed child queries.
        /// </summary>
        public List<IParsedQuery> Children { get; set; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        public List<String> Methods { get; set; }

        /// <summary>
        /// The databases to query against
        /// </summary>
        public List<String> Databases { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        public List<String> Fields { get; set; }

        /// <summary>
        /// The fields used to when assigning a value to a field (update, insert)
        /// </summary>
        public List<String> Assignments { get; set; }

        /// <summary>
        /// The indices to apply when the alter/create
        /// </summary>
        public List<String> Indices { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        public List<String> Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        public List<String> Collections { get; set; }

        /// <summary>
        /// What fields and direction to sort by.
        /// </summary>
        public List<String> Sortings { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        public IDatabaseObject Root { get; set; }

        public ParsedQuery() {
            this.Children = new List<IParsedQuery>();
            this.Methods = new List<String>();
            this.Databases = new List<String>();
            this.Fields = new List<String>();
            this.Assignments = new List<String>();
            this.Conditions = new List<String>();
            this.Collections = new List<String>();
            this.Sortings = new List<String>();
            this.Root = null;
        }
    }
}
