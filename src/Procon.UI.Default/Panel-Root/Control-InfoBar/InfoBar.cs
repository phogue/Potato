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
using System.Windows;
using System.Windows.Controls;

using Procon.UI.API;

namespace Procon.UI.Default.Root.InfoBar
{
    [Extension(
        Alters    = new String[] { "RootLayoutControl" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Root Layout" })]
    public class InfoBar : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Info Bar"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        public bool Entry(Window root)
        {
            // Find controls.
            Grid mainLayout = ExtensionApi.FindControl<Grid>(root, "RootLayoutControl");

            // Check for conflicts.
            if (mainLayout == null) return false;

            // Setup my controls.
            InfoBarView qlbv = new InfoBarView();
            Grid.SetRow(qlbv, 0);
            Grid.SetColumn(qlbv, 0);
            Grid.SetRowSpan(qlbv, 1);
            Grid.SetColumnSpan(qlbv, 4);

            // Alter controls.
            mainLayout.Children.Add(qlbv);

            // We done here broski.
            return true;
        }
    }
}
