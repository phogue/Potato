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
    public class Gameserver
    {
        public IDatabaseProvider IDBP { get; set; }

        public int AddGameserver(string ServerName, string Description, string IP, uint Port, string GameTag)
        {
            int GameID = GetGameID(GameTag);
            if (GameID == 0)
            {
                throw new Exception("GameID not found!");
            }
            string SQL_query = @"INSERT INTO tbl_gameservers (`ServerName`,`Description`,`IP`,`Port`,`tbl_game_GameID`) VALUES (@pr1,@pr2,@pr3,@pr4,@pr5)";
            using (MySqlCommand Insertcommand = new MySqlCommand(SQL_query))
            {
                Insertcommand.Parameters.AddWithValue("@pr1", ServerName);
                Insertcommand.Parameters.AddWithValue("@pr2", Description);
                Insertcommand.Parameters.AddWithValue("@pr3", IP);
                Insertcommand.Parameters.AddWithValue("@pr4", Port);
                Insertcommand.Parameters.AddWithValue("@pr5", GameID);
                IDBP.ExecuteNonQuery(Insertcommand);
            }
            return GetGameserverID(IP, Port);
        }

        public void DeleteGameserver(int GameServerID)
        {
            string SQL_query = @"DELETE FROM tbl_gameservers WHERE `ServerID`= @pr1";
            using (MySqlCommand Deletecommand = new MySqlCommand(SQL_query))
            {
                Deletecommand.Parameters.AddWithValue("@pr1", GameServerID);
                IDBP.ExecuteNonQuery(Deletecommand);
            }
        }

        public int GetGameserverID(String GameserverName)
        {
            string SQL_query = @"SELECT ServerID FROM tbl_gameservers WHERE ServerName = @pr1";
            int int_result = 0;
            using (MySqlCommand Selectcommand = new MySqlCommand(SQL_query))
            {
                Selectcommand.Parameters.AddWithValue("@pr1", GameserverName);
                DataTable ResultTable = IDBP.FillDataTable(Selectcommand);
                foreach (DataRow row in ResultTable.Rows)
                {
                    int_result = Convert.ToInt32(row["ServerID"]);
                }
            }
            return int_result;
        }

        public int GetGameserverID(String GameserverIP, uint Port)
        {
            string SQL_query = @"SELECT ServerID FROM tbl_gameservers WHERE IP = @pr1 AND Port = @pr2";
            int int_result = 0;
            using (MySqlCommand Selectcommand = new MySqlCommand(SQL_query))
            {
                Selectcommand.Parameters.AddWithValue("@pr1", GameserverIP);
                Selectcommand.Parameters.AddWithValue("@pr2", Port);
                DataTable ResultTable = IDBP.FillDataTable(Selectcommand);
                foreach (DataRow row in ResultTable.Rows)
                {
                    int_result = Convert.ToInt32(row["ServerID"]);
                }
            }
            return int_result;
        }

        public int GetGameID(string GameTag)
        {
            string SQL_query = @"SELECT GameID FROM tbl_games WHERE Tag = @pr1";
            int int_result = 0;
            using (MySqlCommand Selectcommand = new MySqlCommand(SQL_query))
            {
                Selectcommand.Parameters.AddWithValue("@pr1", GameTag);
                DataTable ResultTable = IDBP.FillDataTable(Selectcommand);
                foreach (DataRow row in ResultTable.Rows)
                {
                    int_result = Convert.ToInt32(row["GameID"]);
                }
            }
            return int_result;
        }

        public int AddGame(string Tag, string GameName, string Description, string WeapontableName)
        {
            int GameID = GetGameID(Tag);
            if (GameID != 0)
            {
                return GameID;
            }
            string SQL_query = @"INSERT INTO tbl_games (`Tag`,`GameName`,`Description`,`Weapontablename`) VALUES (@pr1,@pr2,@pr3,@pr4)";
            using (MySqlCommand Insertcommand = new MySqlCommand(SQL_query))
            {
                Insertcommand.Parameters.AddWithValue("@pr1", Tag);
                Insertcommand.Parameters.AddWithValue("@pr2", GameName);
                Insertcommand.Parameters.AddWithValue("@pr3", Description);
                Insertcommand.Parameters.AddWithValue("@pr4", WeapontableName);
                IDBP.ExecuteNonQuery(Insertcommand);
            }
            return GetGameID(Tag);
        }

        public void DeleteGame(int GameID)
        {
            string SQL_query = @"DELETE FROM tbl_games WHERE `GameID`= @pr1";
            using(MySqlCommand Deletecommand = new MySqlCommand(SQL_query))
            {
                Deletecommand.Parameters.AddWithValue("@pr1", GameID);
                IDBP.ExecuteNonQuery(Deletecommand);
            }
        }
    }
}
