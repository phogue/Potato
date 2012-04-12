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
    public class Admin
    {
        public IDatabaseProvider IDBP { get; set; }

        public int AddAdmin(string AdminName)
        {
            string SQL_query = @"INSERT INTO tbl_admins (`Name`) VALUES (@pr1)";
            using(MySqlCommand Insertcommand = new MySqlCommand(SQL_query))
            {
                Insertcommand.Parameters.AddWithValue("@pr1", AdminName);
                IDBP.ExecuteNonQuery(Insertcommand);
            }
            return GetAdminID(AdminName);
        }

        public int GetAdminID(string AdminName)
        {
            string SQL_query = @"SELECT AdminID FROM tbl_admins WHERE Name = @pr1";
            int int_result = 0;
            using (MySqlCommand Selectcommand = new MySqlCommand(SQL_query))
            {
                Selectcommand.Parameters.AddWithValue("@pr1", AdminName);
                DataTable ResultTable = IDBP.FillDataTable(Selectcommand);
                foreach (DataRow row in ResultTable.Rows)
                {
                    int_result = Convert.ToInt32(row["AdminID"]);
                }
            }
            return int_result;
        }
    }
}
