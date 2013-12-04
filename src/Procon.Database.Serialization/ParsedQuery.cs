using System;
using System.Collections.Generic;

namespace Procon.Database.Serialization {
    /// <summary>
    /// The initial parse to collection peices of information but
    /// remain in a hierarchy
    /// </summary>
    [Serializable]
    public class ParsedQuery : QueryData, IParsedQuery {
        /// <summary>
        /// List of parsed child queries.
        /// </summary>
        public List<IParsedQuery> Children { get; set; }

        public ParsedQuery() : base() {
            this.Children = new List<IParsedQuery>();
        }
    }
}
