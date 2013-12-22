using System.Collections.Generic;

namespace Procon.Database.Shared {
    /// <summary>
    /// The initial parse to collection peices of information but
    /// remain in a hierarchy
    /// </summary>
    public interface IParsedQuery : IQueryData {
        /// <summary>
        /// List of parsed child queries.
        /// </summary>
        List<IParsedQuery> Children { get; set; }
    }
}
