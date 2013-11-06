using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    public interface ISerializer {

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        List<string> Methods { get; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        List<string> Fields { get; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        List<string> Conditions { get; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        List<string> Collections { get; }

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        String Parse(Method method);

        /// <summary>
        /// Alias for Parse(Method method) without requirement of caller to convert type.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        String Parse(IQuery query);
    }
}
