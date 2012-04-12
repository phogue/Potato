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
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Procon.UI.API;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Styles
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Dark : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Dark Style"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Find controls.
            Grid       rootLayout = ExtensionApi.FindControl<Grid>(root, "RootLayoutControl");
            TabControl tabsLayout = ExtensionApi.FindControl<TabControl>(root, "ConnectionTabsLayoutControl");
            GroupBox   chatLayout = ExtensionApi.FindControl<GroupBox>(root, "ConnectionChatAndEvents");
            

            // Check for conflicts.
            if (rootLayout == null || tabsLayout == null || chatLayout == null) return false;

            // Setup my controls.
            // -- Background Color.
            SolidColorBrush rootBgColor = new SolidColorBrush(Color.FromArgb(255, 80,  88,  98));
            SolidColorBrush tabsBgColor = new SolidColorBrush(Color.FromArgb(60,  255, 255, 255));
            // -- Background Image.
            ImageBrush rootBgImage = null;            
            if (File.Exists(Defines.PROCON_LOGO))
                rootBgImage = new ImageBrush(
                              new BitmapImage(
                              new Uri(Defines.PROCON_LOGO, UriKind.RelativeOrAbsolute))) {
                                  AlignmentX = AlignmentX.Center, Stretch  = Stretch.None,
                                  AlignmentY = AlignmentY.Center, TileMode = TileMode.None
                              };
            // -- Default Label Style
            Style sLabel = new Style(typeof(Label), (Style)root.FindResource(typeof(Label)));
            sLabel.Setters.Add(new Setter(Label.PaddingProperty, new Thickness(0.0)));
            sLabel.Setters.Add(new Setter(Label.ForegroundProperty, new SolidColorBrush(Color.FromArgb(255, 204, 204, 204))));
            sLabel.Setters.Add(new Setter(Label.VerticalAlignmentProperty, VerticalAlignment.Center));
            // -- Default Label Style
            Style sCheckBox = new Style(typeof(CheckBox), (Style)root.FindResource(typeof(CheckBox)));
            sCheckBox.Setters.Add(new Setter(Label.PaddingProperty, new Thickness(0.0)));
            sCheckBox.Setters.Add(new Setter(Label.ForegroundProperty, new SolidColorBrush(Color.FromArgb(255, 204, 204, 204))));
            // -- Default GroupBox Style
            Style sGroupBox = new Style(typeof(GroupBox), (Style)root.FindResource(typeof(GroupBox)));
            sGroupBox.Setters.Add(new Setter(GroupBox.ForegroundProperty, new SolidColorBrush(Color.FromArgb(255, 7, 125, 201))));
            // -- Default DataGrid Style
            Style sDataGrid = new Style(typeof(DataGrid), (Style)root.FindResource(typeof(DataGrid)));
            sDataGrid.Setters.Add(new Setter(DataGrid.BackgroundProperty, new SolidColorBrush(Color.FromArgb(204, 255, 255, 255))));

            // Alter controls.
            root.Background       = rootBgColor;
            rootLayout.Background = rootBgImage;
            rootLayout.Resources.Add(typeof(Label),    sLabel);
            rootLayout.Resources.Add(typeof(CheckBox), sCheckBox);
            rootLayout.Resources.Add(typeof(GroupBox), sGroupBox);
            rootLayout.Resources.Add(typeof(DataGrid), sDataGrid);
            tabsLayout.Background = tabsBgColor;
            chatLayout.Background = tabsBgColor;

            // We done here broski.
            return true;
        }
    }
}
