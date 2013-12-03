using System;
using System.Data.Common;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders;

namespace Procon.Database {
    public abstract class Driver : IDriver, IDisposable, ICloneable, IEquatable<Driver> {

        /// <summary>
        /// The name of this driver.
        /// </summary>
        public abstract String Name { get; }

        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        public String Hostname { get; set; }

        /// <summary>
        /// The port to connect over.
        /// </summary>
        public uint Port { get; set; }

        /// <summary>
        /// The username for authentication.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// The name of the database to select.
        /// </summary>
        public String Database { get; set; }

        /// <summary>
        /// The open connection to the database.
        /// </summary>
        protected DbConnection Connection { get; set; }

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

        public bool Equals(Driver other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Hostname, other.Hostname) && Port == other.Port && string.Equals(Username, other.Username) && string.Equals(Password, other.Password) && string.Equals(Database, other.Database);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Driver)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Hostname != null ? Hostname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Port;
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Database != null ? Database.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
