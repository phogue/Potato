using System;
using System.Collections.Generic;
using Procon.Database.Shared;

namespace Procon.Database {
    /// <summary>
    /// Base driver for a database connection
    /// </summary>
    public abstract class Driver : IDriver {

        /// <summary>
        /// The name of this driver.
        /// </summary>
        public abstract String Name { get; }

        /// <summary>
        /// The settings used to connect & authenticate with the database
        /// </summary>
        public IDriverSettings Settings { get; set; }

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        public abstract bool Connect();

        /// <summary>
        /// Runs a compiled query
        /// </summary>
        /// <param name="query"></param>
        protected abstract List<IDatabaseObject> Query(ICompiledQuery query);

        /// <summary>
        /// Query the open driver
        /// </summary>
        /// <param name="query"></param>
        public abstract List<IDatabaseObject> Query(IDatabaseObject query);

        /// <summary>
        /// Loop through escaping all StringValue database objects anywhere in the tree.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract IDatabaseObject EscapeStringValues(IDatabaseObject query);

        /// <summary>
        /// Closes the open connection the database.
        /// </summary>
        public abstract void Close();

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
