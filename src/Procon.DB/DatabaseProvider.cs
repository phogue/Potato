// Copyright 2011 Christian 'XpKiller' Suhr
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Procon.DB
{  
    public class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        //public Gameserver Gameserver { get; set; }
        //public Player Player { get; set;}
       // public Admin Admin { get; set; }
        MySqlConnectionStringBuilder MSCSB;

        /// <summary>
        /// Define your Databaseconnection
        /// </summary>
        /// <param name="Server">DNS Name or IP Address of the MySql Server</param>
        /// <param name="Port">Database Port</param>
        /// <param name="Database">Select the target DB</param>
        /// <param name="UserID">Database Username</param>
        /// <param name="MySqlPassword"> Password</param>
        /// <param name="useCompression">Enables Connection Compression (true/false)</param>
        public DatabaseProvider(string Server, uint Port, string Database, string UserID, string MySqlPassword, bool useCompression)
        {
            MSCSB = new MySqlConnectionStringBuilder();
            MSCSB.Server = Server;
            MSCSB.Port = Port;
            MSCSB.Database = Database;
            MSCSB.UserID = UserID;
            MSCSB.Password = MySqlPassword;
            MSCSB.UseCompression = useCompression;
            //MSCSB.Keepalive = 30;
            MSCSB.Pooling = true;
            MSCSB.MinimumPoolSize = 1;
            MSCSB.MaximumPoolSize = 10;
        }
        /// <summary>
        /// Use this to execute DDL commands on the DB server.
        /// This method automatically starts a transaction if there is none.
        /// </summary>
        /// <param name="SqlQuery">Your SQL query</param>
        /// <param name="Commit">Enable to autocommit a transaction.</param>
        public DatabaseProvider ExecuteNonQuery(string SqlQuery)
        {
            using (MySqlConnection MySqlConnection = new MySqlConnection(MSCSB.ConnectionString))
            {
                MySqlConnection.Open();
                MySqlTransaction MyTransaction = MySqlConnection.BeginTransaction();
                MySqlCommand MyCommand = new MySqlCommand(SqlQuery, MySqlConnection, MyTransaction);
                try
                {
                    MyCommand.ExecuteNonQuery();
                    MyTransaction.Commit();
                }
                catch(Exception e)
                {
                    try
                    {
                        MyTransaction.Rollback();
                    }
                    catch (MySqlException ex)
                    {
                        if (MyTransaction.Connection != null)
                        {
                            //Console.WriteLine("An exception of type " + ex.GetType() +" was encountered while attempting to roll back the transaction.");
                        }
                    }
                }
            }
            return this;
        }

        public DatabaseProvider ExecuteNonQuery(MySqlCommand MySqlCommand)
        {
            using (MySqlConnection MySqlConnection = new MySqlConnection(MSCSB.ConnectionString))
            {
                MySqlConnection.Open();
                MySqlTransaction MyTransaction = MySqlConnection.BeginTransaction();
                try
                {
                    MySqlCommand.Connection = MySqlConnection;
                    MySqlCommand.Transaction = MyTransaction;
                    MySqlCommand.ExecuteNonQuery();
                    MyTransaction.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        MyTransaction.Rollback();
                    }
                    catch (MySqlException ex)
                    {
                        if (MyTransaction.Connection != null)
                        {
                            //Console.WriteLine("An exception of type " + ex.GetType() + " was encountered while attempting to roll back the transaction.");
                        }
                    }
                }
            }
            return this;
        }

        public DatabaseProvider ExecuteNonQuery(List<MySqlCommand> MySqlCommand)
        {
            using (MySqlConnection MySqlConnection = new MySqlConnection(MSCSB.ConnectionString))
            {
                MySqlConnection.Open();
                MySqlTransaction MyTransaction = MySqlConnection.BeginTransaction();
                foreach (MySqlCommand MyCommand in MySqlCommand)
                {
                    try
                    {
                        MyCommand.Connection = MySqlConnection;
                        MyCommand.Transaction = MyTransaction;
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            MyTransaction.Rollback();
                        }
                        catch (MySqlException ex)
                        {
                            if (MyTransaction.Connection != null)
                            {
                                Console.WriteLine("An exception of type " + ex.GetType() + " was encountered while attempting to roll back the transaction.");
                            }
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Executes a SELECT Query on the DB and return the result in a DataTable.
        /// </summary>
        /// <param name="SelectQuery">Select query</param>
        /// <returns></returns>
        public DataTable FillDataTable(string SelectQuery)
        {
            using (DataTable Table = new DataTable())
            {
                try
                {
                    using (MySqlDataAdapter MyAdapter = new MySqlDataAdapter(SelectQuery,MSCSB.ConnectionString))
                    {
                        MyAdapter.Fill(Table);
                    }
                }
                catch (MySqlException)
                {
                    throw;
                }
                return Table;
            }
        }

        public DataTable FillDataTable(MySqlCommand selectCommand)
        {
            using (DataTable Table = new DataTable())
            {
                try
                {
                    selectCommand.Connection = new MySqlConnection(MSCSB.ConnectionString);
                    using (MySqlDataAdapter MyAdapter = new MySqlDataAdapter(selectCommand))
                    {
                        MyAdapter.Fill(Table);
                    }
                }
                catch (MySqlException)
                {
                    throw;
                }
                finally
                {
                    selectCommand.Dispose();
                }
                return Table;
            }
        }

        //=====================Dispose========================
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DatabaseProvider ()
        {
            Dispose (false);
        }
        protected virtual void Dispose(bool fDisposing)
        {
            if (fDisposing)
            {
                // Release managed ressources
               
            }
            // Release unmanaged ressources
        }
    }
}
