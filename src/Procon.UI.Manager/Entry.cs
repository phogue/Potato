// Copyright 2011 Cameron 'Imisnew2' Gunnin
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

using Procon.UI.Manager.ViewModels;
using Procon.UI.Manager.Views;

namespace Procon.UI.Manager
{
    /// <summary>
    /// Allows the user to manage which extensions are used by Procon 2 when the program
    /// is ran by creating a UI that searches the default Extensions directory for Procon 2.
    /// </summary>
    class Entry
    {
        [STAThread]
        static void Main(String[] args)
        {
            // Create the main view and view model.
            ManagerView      v  = new ManagerView();
            ManagerViewModel vm = new ManagerViewModel();

            // Set the Data Context and display the window.
            v.DataContext = vm;
            v.ShowDialog();
        }
    }
}