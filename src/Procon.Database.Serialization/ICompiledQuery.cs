﻿using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    public interface ICompiledQuery {

        /// <summary>
        /// List of compiled child queries.
        /// </summary>
        List<ICompiledQuery> Children { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        IQuery Root { get; set; }

        /// <summary>
        /// The compiled query, used for SQL queries.
        /// </summary>
        String Completed { get; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        String Method { get; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        List<String> Fields { get; }

        /// <summary>
        /// The fields to set during the query
        /// </summary>
        String Assignments { get; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        String Conditions { get; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        String Collections { get; }

        /// <summary>
        /// How to order the results
        /// </summary>
        String Sortings { get; }
    }
}
