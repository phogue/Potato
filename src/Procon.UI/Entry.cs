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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Procon.Core;
using Procon.UI.API.Converters;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;
using Procon.UI.Extensions;

namespace Procon.UI
{
    /// <summary>
    /// Builds a UI by loading extensions via the extension controller.
    /// The default UI is already built in Procon.UI.Default and is, by default
    /// loaded and executed.  The behaviour of the UI can be altered by writing
    /// and extension and using the Extension Manager to load that extension.
    /// </summary>
    class Entry
    {
        [STAThread]
        static void Main(String[] args)
        {
            // Create the root element of the UI, set its icon, and set its background.
            Window root = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Name       = "Root", Title  = "Procon 2",
                MinWidth   = 900,    Width  = 900,
                MinHeight  = 650,    Height = 650
            };
            if (File.Exists(Defines.PROCON_ICON))
                root.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));

            // Start Procon.
            InstanceViewModel Procon = new InstanceViewModel(new Instance());
            InstanceViewModel.PublicProperties["Procon"].Value = Procon;
            Procon.Execute();

            // Load the extensions into the UI.
            ExtensionController.ReadConfig(Path.Combine(Defines.EXTENSIONS_DIRECTORY, Defines.EXTENSIONS_CONFIG), root);

            // Set the Data Context and display the window.
            root.DataContext = Procon;
            root.ShowDialog();

            // Shutdown Procon.
            Procon.Shutdown();
        }
    }
}
