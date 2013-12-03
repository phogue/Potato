using System;
using Procon.Database.Serialization;

namespace Procon.Database {
    public abstract class Driver : IDriver, IDisposable, ICloneable {

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
        protected abstract IDatabaseObject Query(ICompiledQuery query);

        /// <summary>
        /// Query the open driver
        /// </summary>
        /// <param name="query"></param>
        public abstract IDatabaseObject Query(IDatabaseObject query);

        /// <summary>
        /// Closes the open connection the database.
        /// </summary>
        public abstract void Close();

        public void Dispose() {
            this.Close();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
