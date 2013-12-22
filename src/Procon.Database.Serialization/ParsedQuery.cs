using System;
using System.Collections.Generic;
using Procon.Database.Shared;

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

        /// <summary>
        /// Initializes all defaults
        /// </summary>
        public ParsedQuery() : base() {
            this.Children = new List<IParsedQuery>();
        }
    }
}
