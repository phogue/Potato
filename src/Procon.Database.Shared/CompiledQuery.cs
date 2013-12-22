using System;
using System.Collections.Generic;

namespace Procon.Database.Shared {
    /// <summary>
    /// A compiled version of the query with as basic information 
    /// as we can boil the complex query down to
    /// </summary>
    [Serializable]
    public class CompiledQuery : QueryData, ICompiledQuery {
        /// <summary>
        /// List of compiled child queries.
        /// </summary>
        public List<ICompiledQuery> Children { get; set; }

        /// <summary>
        /// The combined and compield version of the query
        /// </summary>
        public List<String> Compiled { get; set; }

        /// <summary>
        /// Initializes all defaults
        /// </summary>
        public CompiledQuery() : base() {
            this.Children = new List<ICompiledQuery>();
            this.Compiled = new List<String>();
        }
    }
}
