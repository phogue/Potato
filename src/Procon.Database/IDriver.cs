using System;
using System.Collections.Generic;
using Procon.Database.Shared;

namespace Procon.Database {
    /// <summary>
    /// Base driver for a database connection
    /// </summary>
    public interface IDriver : ICloneable {

        /// <summary>
        /// The name of this driver.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The settings used to connect & authenticate with the database
        /// </summary>
        IDriverSettings Settings { get; set; }

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        bool Connect();

        /// <summary>
        /// Query the open driver
        /// </summary>
        /// <param name="query"></param>
        List<IDatabaseObject> Query(IDatabaseObject query);

        /// <summary>
        /// Closes the open connection the database.
        /// </summary>
        void Close();
    }
}
