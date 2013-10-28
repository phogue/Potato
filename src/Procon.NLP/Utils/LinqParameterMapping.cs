using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Procon.Nlp.Utils {
    public class LinqParameterMapping {

        /// <summary>
        /// The type of object to map to
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Parameter expression to be used when building linq expressions
        /// </summary>
        public ParameterExpression Parameter { get; set; }

        /// <summary>
        /// The list of objects to apply compiled linq expressions to
        /// </summary>
        public Object Collection { protected get; set; }

        public List<T> FetchCollection<T>() {
            return this.Collection as List<T>;
        }
    }
}
