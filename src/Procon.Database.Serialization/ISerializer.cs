using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    public interface ISerializer {

        /// <summary>
        /// Compile a query, creating a new compiled object and assigning all reduced data
        /// from the serializer to the compiled query.
        /// </summary>
        /// <returns></returns>
        ICompiledQuery Compile();

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        ISerializer Parse(Method method);

        /// <summary>
        /// Alias for Parse(Method method) without requirement of caller to convert type.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ISerializer Parse(IQuery query);
    }
}
