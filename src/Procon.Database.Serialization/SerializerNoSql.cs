using System;
using System.Collections.Generic;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {

    public abstract class SerializerNoSql : ISerializer {

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        public List<string> Methods { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        public List<string> Fields { get; set; }

        /// <summary>
        /// The conditions used during an update, remove or modify
        /// </summary>
        public List<string> Conditions { get; set; }

        /// <summary>
        /// The collection this query should be used against.
        /// </summary>
        public List<string> Collections { get; set; }

        protected SerializerNoSql() {
            this.Methods = new List<string>();
            this.Fields = new List<string>();
            this.Conditions = new List<string>();
            this.Collections = new List<string>();
        }

        public virtual String Parse(Method method) {

            return "";
        }

        public String Parse(IQuery query) {
            Method method = query as Method;

            return method != null ? this.Parse(method) : String.Empty;
        }
    }
}
