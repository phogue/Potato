using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders;

namespace Procon.Database {
    public interface IDriver {
        /// <summary>
        /// The name of this driver.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        String Hostname { get; set; }

        /// <summary>
        /// The port to connect over.
        /// </summary>
        uint Port { get; set; }

        /// <summary>
        /// The username for authentication.
        /// </summary>
        String Username { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        String Password { get; set; }

        /// <summary>
        /// The name of the database to select.
        /// </summary>
        String Database { get; set; }

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        bool Connect();

        /// <summary>
        /// Query the open driver
        /// </summary>
        /// <param name="query"></param>
        IDatabaseObject Query(IDatabaseObject query);

        /// <summary>
        /// Closes the open connection the database.
        /// </summary>
        void Close();
    }
}
