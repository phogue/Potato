﻿#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Values;
using Potato.Database.Shared.Serializers.Sql;
using Potato.Database.Shared.Utils;

namespace Potato.Database.Drivers {
    /// <summary>
    /// Driver support for MySQL
    /// </summary>
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

            if (this.Connection == null || this.Connection.State == ConnectionState.Closed) {
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
            }

            return opened;
        }

        public override List<IDatabaseObject> Query(IDatabaseObject query) {
            this.Connect();

            return this.Query(new SerializerMySql().Parse(this.EscapeStringValues(query)).Compile());
        }

        public override IDatabaseObject EscapeStringValues(IDatabaseObject query) {
            foreach (StringValue item in query.DescendantsAndSelf<StringValue>()) {
                item.Data = MySqlHelper.EscapeString(item.Data);
            }

            return query;
        }

        /// <summary>
        /// Query a SELECT statement which returns results
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected CollectionValue Read(ICompiledQuery query, CollectionValue result) {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open) {
                using (IDbCommand command = this.Connection.CreateCommand()) {
                    command.CommandText = query.Compiled.FirstOrDefault();

                    using (IDataReader reader = command.ExecuteReader()) {
                        if (reader != null) {
                            while (reader.Read() == true) {
                                DocumentValue row = new DocumentValue();

                                for (int field = 0; field < reader.FieldCount; field++) {
                                    row.Set(reader.GetName(field), reader.GetValue(field));
                                }

                                result.Add(row);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Query anything that does not return any results
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected CollectionValue Execute(ICompiledQuery query, CollectionValue result) {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open) {
                using (IDbCommand command = this.Connection.CreateCommand()) {
                    command.CommandText = query.Compiled.FirstOrDefault();

                    result.Add(new Affected() {
                        new NumericValue() {
                            Long = command.ExecuteNonQuery()
                        }
                    });
                }
            }

            return result;
        }

        protected override List<IDatabaseObject> Query(ICompiledQuery query) {
            CollectionValue result = new CollectionValue();

            List<IDatabaseObject> results = new List<IDatabaseObject>();

            try {
                results.Add(query.Root is Find ? this.Read(query, result) : this.Execute(query, result));
            }
            catch (Exception exception) {
                results.Add(new Error() {
                    new StringValue() {
                        Data = exception.Message
                    }
                });
            }

            // Execute any index calls.
            foreach (ICompiledQuery child in query.Children.Where(child => !(child.Root is Merge))) {
                results.AddRange(this.Query(child));
            }

            return results;
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
