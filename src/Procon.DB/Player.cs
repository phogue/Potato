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
    public class Player
    {
        public IDatabaseProvider IDBP { get; set; }

        public int GetPlayerID()
        {
            return 0;
        }

        public int CreatePlayer()
        {
            return 0;
        }

        public int getProfilID()
        {
            return 0;
        }

        public int CreateProfile()
        {
            return 0;
        }

        public void Kills()
        {

        }

        public void UpdateScores()
        {

        }

        public void Joined()
        {
        }

        public void Left()
        {

        }

        //Banlist
        public void AddToBanlist(int PlayerID, int GameServerID, int AdminID, string Reason, string TypeOfBan, int BanDuration, bool Globalban)
        {
            if(PlayerID <= 0 || GameServerID <= 0 || AdminID <= 0 || TypeOfBan == null)
            {
                return;
            }
            string SQL_query = @"INSERT INTO tbl_banlist (`tbl_playerdata_PlayerID`, `tbl_gameservers_ServerID`, `tbl_admins_AdminID`,`Reason`, `TypeOfBan`, `BanDuration`, `Time`, `Global`) 
                                 VALUES (@pr1, @pr2, @pr3, @pr4, @pr5, @pr6, @pr7)";
            using(MySqlCommand Insertcommand = new MySqlCommand(SQL_query))
            {
                Insertcommand.Parameters.AddWithValue("@pr1", PlayerID);
                Insertcommand.Parameters.AddWithValue("@pr2", GameServerID);
                Insertcommand.Parameters.AddWithValue("@pr3", AdminID);
                Insertcommand.Parameters.AddWithValue("@pr4", Reason);
                Insertcommand.Parameters.AddWithValue("@pr5", TypeOfBan);
                Insertcommand.Parameters.AddWithValue("@pr6", BanDuration);
                Insertcommand.Parameters.AddWithValue("@pr7", Globalban);
                IDBP.ExecuteNonQuery(Insertcommand);
            }
        }

        public void RemoveFromBanlist()
        {

        }

        public void ModifyBan()
        {

        }

    }
}
