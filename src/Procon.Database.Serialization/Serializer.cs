using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    public abstract class Serializer : ISerializer {

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        protected List<String> Methods { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        protected List<String> Fields { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        protected List<String> Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        protected List<String> Collections { get; set; }

        /// <summary>
        /// What fields and direction to sort by.
        /// </summary>
        protected List<String> Sortings { get; set; } 

        protected Serializer() {
            this.Methods = new List<String>();
            this.Fields = new List<String>();
            this.Conditions = new List<String>();
            this.Collections = new List<String>();
            this.Sortings = new List<String>();
        }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        protected IQuery Root { get; set; }

        public abstract ICompiledQuery Compile();

        public abstract ISerializer Parse(Method method);

        public ISerializer Parse(IQuery query) {
            Method method = query as Method;

            this.Parse(method);

            return this;
        }
    }
}
