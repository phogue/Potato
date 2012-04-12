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
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Connection.Tabs.Plugins
{
    [Extension(
        Alters    = new String[] { "ConnectionTabsLayoutControl" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Connection Tabs Layout" })]
    public class Plugins : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Connection Tabs Plugins"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Find controls.
            TabControl connectionTabs = ExtensionApi.FindControl<TabControl>(root, "ConnectionTabsLayoutControl");

            // Check for conflicts.
            if (connectionTabs == null) return false;

            // Setup my controls.
            PluginsView pv = new PluginsView();
            pv.Header = Localizer.Loc("Procon.UI.Default.Root.Connection.Tabs.Plugins.Title");

            // Alter controls.
            connectionTabs.Items.Add(pv);

            // We done here broski.
            return true;
        }
    }
}
