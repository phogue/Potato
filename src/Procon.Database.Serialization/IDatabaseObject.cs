using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    /// <summary>
    /// The base object with a bunch of helper methods to make
    /// building a query relatively 
    /// </summary>
    public interface IDatabaseObject : ICollection<IDatabaseObject> {

        IDatabaseObject Method(IDatabaseObject data);

        IDatabaseObject Database(IDatabaseObject data);

        IDatabaseObject Database(String name);

        IDatabaseObject Index(IDatabaseObject data);

        IDatabaseObject Index(String name);

        IDatabaseObject Index(String name, SortByModifier sortByModifier);

        IDatabaseObject Index(String name, IndexModifer indexModifier);

        IDatabaseObject Index(String name, IndexModifer indexModifier, SortByModifier sortByModifier);

        IDatabaseObject Modifier(IDatabaseObject data);

        IDatabaseObject Field(IDatabaseObject data);

        IDatabaseObject Field(String name);

        IDatabaseObject Field(String name, FieldType type, bool nullable = true);

        IDatabaseObject Field(String name, int length, bool nullable = true);

        IDatabaseObject Condition(IDatabaseObject data);

        /// <summary>
        /// Implied equals condition `name` = data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IDatabaseObject Condition(String name, Object data);

        /// <summary>
        /// Shorthand for quick conditionals
        /// </summary>
        /// <param name="name"></param>
        /// <param name="equality"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IDatabaseObject Condition(String name, Equality equality, Object data);

        IDatabaseObject Assignment(IDatabaseObject data);

        IDatabaseObject Assignment(String name, Object data);

        IDatabaseObject Collection(IDatabaseObject data);

        /// <summary>
        /// Shorthand for quick collection statements
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDatabaseObject Collection(String name);

        IDatabaseObject Sort(IDatabaseObject data);

        IDatabaseObject Sort(String name, Modifier modifier = null);
    }
}
