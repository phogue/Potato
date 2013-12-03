using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Drivers {
    public class MySqlDriver : Driver {

        /// <summary>
        /// The open connection to the database.
        /// </summary>
        protected DbConnection Connection { get; set; }

        public override String Name {
            get {
                return "MySQL";
            }
        }

        public override bool Connect() {
            bool opened = true;

            if (this.Connection != null) {
                this.Connection.Close();
                this.Connection = null;
            }

            MySqlConnectionStringBuilder connectionBuilder = new MySqlConnectionStringBuilder {
                Server = this.Settings.Hostname,
                Port = this.Settings.Port != null ? this.Settings.Port.Value : 0,
                Database = this.Settings.Database,
                UserID = this.Settings.Username,
                Password = this.Settings.Password,
                UseCompression = true
            };

            try {
                this.Connection = new MySqlConnection(connectionBuilder.ToString());
                this.Connection.Open();

                if (this.Connection.State != ConnectionState.Open) {
                    this.Connection.Close();
                    this.Connection = null;
                }
            }
            catch {
                opened = false;
            }

            return opened;
        }

        public override IDatabaseObject Query(IDatabaseObject query) {
            return this.Query(new SerializerMySql().Parse(query).Compile());
        }

        /// <summary>
        /// Query a SELECT statement which returns results
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected void Read(ICompiledQuery query, CollectionValue result) {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open) {
                using (IDbCommand command = this.Connection.CreateCommand()) {
                    command.CommandText = query.Completed;

                    using (IDataReader reader = command.ExecuteReader()) {
                        if (reader != null) {
                            while (reader.Read() == true) {
                                DocumentValue row = new DocumentValue();

                                for (int field = 0; field < reader.FieldCount; field++) {
                                    row.Assignment(reader.GetName(field), reader.GetValue(field));
                                }

                                result.Add(row);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Query anything that does not return any results
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected void Execute(ICompiledQuery query, CollectionValue result) {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open) {
                using (IDbCommand command = this.Connection.CreateCommand()) {
                    command.CommandText = query.Completed;

                    result.Add(
                        new Affected() {
                            new NumericValue() {
                                Integer = command.ExecuteNonQuery()
                            }
                        }
                    );
                }
            }
        }

        protected override IDatabaseObject Query(ICompiledQuery query) {
            CollectionValue result = new CollectionValue();

            if (query.Root is Find) {
                this.Read(query, result);
            }
            else {
                this.Execute(query, result);
            }

            return result;
        }

        public override void Close() {
            if (this.Connection != null) {
                this.Connection.Close();
                // Is this already disposed when closed?
                this.Connection.Dispose();
                this.Connection = null;
            }
        }
    }
}
