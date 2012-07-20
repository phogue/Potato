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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Procon.DB;
using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Security.Objects;
using Procon.Net;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils;

namespace Procon.DB.Test
{
    class Program
    {
        private IDatabaseProvider MyDB;
        private Admin Admin;

        static void Main(string[] a)
        {
            Program Test = new Program();
            System.Threading.Thread.Sleep(2000);
        }


        public Program()
        {
            MyDB = new DatabaseProvider("127.0.0.1", 3306, "Procon2", "procontest", "procontest", false);
            Admin = new Admin();
            Admin.IDBP = MyDB;

            //Console.WriteLine(Admin.AddAdmin(String.Concat("XpKiller").ToString()));
            //Console.WriteLine(Admin.AddAdmin(String.Concat("Tester").ToString()));

            //Setting up Procon2 Core
            System.Console.WriteLine("Starting Procon2 Instance..");
            Instance i = new Instance();
            i.Execute();
            Interface myInterface = i.Interfaces[0];
            myInterface.AddConnection(CommandInitiator.Local, "BF_BC2", "173.199.64.11", 19587, "854059");
            Connection myConnection = myInterface.Connections[0];

            //Setting up the Eventhandling
            myConnection.GameEvent += new Game.GameEventHandler(myConnection_GameEvent);

            myConnection.ClientEvent += new Game.ClientEventHandler(myConnection_ClientEvent);


            System.Console.WriteLine("State: " + myConnection.GameState.Variables.ConnectionState.ToString());

            System.Console.WriteLine("Type 'exit' to close this application");

            //while (String.Compare(System.Console.ReadLine(), "exit", true) != 0) ;
            System.Console.ReadKey();
            System.Console.WriteLine("State: " + myConnection.GameState.Variables.ConnectionState.ToString());
            System.Console.WriteLine("Shutting down Procon2 instance..");
            i.Dispose();

        }

        void myConnection_ClientEvent(Game sender, ClientEventArgs e)
        {
            System.Console.WriteLine("Got ClientEvent: " + e.EventType.ToString());
        }


        void myConnection_GameEvent(Game sender, GameEventArgs e)
        {
            System.Console.WriteLine("Got GameEvent: " + e.EventType.ToString());
            switch (e.EventType)
            {
                case GameEventType.PlayerJoin:
                    System.Console.WriteLine("Player joined " + e.Player.Name);
                    break;
                case GameEventType.PlayerLeave:
                    break;
                case GameEventType.PlayerMoved:
                    break;
                case GameEventType.PlayerlistUpdated:
                    break;

                case GameEventType.PlayerKicked:
                    break;
                case GameEventType.PlayerBanned:
                    break;
                case GameEventType.PlayerUnbanned:
                    break;

                case GameEventType.PlayerKill:
                    break;
                case GameEventType.PlayerSpawn:
                    break;

                case GameEventType.Chat:
                    break;
                case GameEventType.RoundChanged:
                    break;

                case GameEventType.ServerInfoUpdated:
                    break;
                case GameEventType.BanlistUpdated:
                    break;
                case GameEventType.GameConfigExecuted:
                    break;
            }
        }
    }
}