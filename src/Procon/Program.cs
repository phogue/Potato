// Copyright 2011 Geoffrey 'Phogue' Green
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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Procon {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {

            bool startConsole = false;
            bool updateOnly = false;
            //string strArgs = String.Empty;

            // search for parameter to start UI/console and add all others to a string to pass them on
            if (args.Contains("-console") == true || args.Contains("--console") == true)
            {
                startConsole = true;
            }

            if (args.Contains("-updateonly") == true || args.Contains("--updateonly") == true)
            {
                updateOnly = true;
            }

            // string[] to string with separator
            //strArgs = String.Join("|", args);
            
            new Updater().Execute().Shutdown();

            if (updateOnly == false)
            {
                try
                {
                    if (startConsole)
                    {
                        //Starts the Procon.Console.exe
                        Process.Start(Updater.PROCON_DIRECTORY_PROCON_CONSOLE_EXE, String.Join(" ", args));
                    }
                    else
                    {
                        // Starts the Procon.UI.exe if no -console parameter is give
                        Process.Start(Updater.PROCON_DIRECTORY_PROCON_UI_EXE);
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
