using System.Collections.Generic;

namespace Procon.Database.Shared {
    /// <summary>
    /// A compiled version of the query with as basic information 
    /// as we can boil the complex query down to
    /// </summary>
    public interface ICompiledQuery : IQueryData {

        /// <summary>
        /// List of compiled child queries.
        /// </summary>
        List<ICompiledQuery> Children { get; set; }

        /// <summary>
        /// The combined and compield version of the query
        /// </summary>
        List<string> Compiled { get; set; }
    }
}
