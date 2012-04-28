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
    public class Light : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Light Style"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Find controls.
            Grid rootLayout = ExtensionApi.FindControl<Grid>(root, "RootLayoutControl");

            // Check for conflicts.
            if (rootLayout == null) return false;

            // Setup my controls.
            // -- Some colors for various controls.
            Color cControlLightColor  = Colors.White;
            Color cControlMediumColor = Color.FromArgb(255, 208, 208, 208);
            Color cBorderLightColor   = Color.FromArgb(255, 204, 204, 204);
            Color cBorderDarkColor    = Color.FromArgb(255, 68,  68,  68);
            // -- Some brushes using the colors we setup.
            LinearGradientBrush bControlBrush = new LinearGradientBrush();
            bControlBrush.StartPoint = new Point(0.5, 0);
            bControlBrush.EndPoint   = new Point(0.5, 1);
            bControlBrush.GradientStops.Add(new GradientStop(cControlLightColor, 0.0));
            bControlBrush.GradientStops.Add(new GradientStop(cControlMediumColor, 1.0));
            LinearGradientBrush bBorderBrush = new LinearGradientBrush();
            bBorderBrush.StartPoint = new Point(0, 0);
            bBorderBrush.EndPoint   = new Point(0, 1);
            bBorderBrush.GradientStops.Add(new GradientStop(cBorderLightColor, 0.0));
            bBorderBrush.GradientStops.Add(new GradientStop(cBorderDarkColor, 1.0));

            // -- Default Label Style
            Style sLabel = new Style(typeof(Label), (Style)root.FindResource(typeof(Label)));
            sLabel.Setters.Add(new Setter(Label.PaddingProperty, new Thickness(0.0)));
            sLabel.Setters.Add(new Setter(Label.VerticalAlignmentProperty, VerticalAlignment.Center));
            // -- Default Button Style
            Style sButton = new Style(typeof(Button), (Style)root.FindResource(typeof(Button)));
            sButton.Setters.Add(new Setter(Button.BackgroundProperty, bControlBrush));
            sButton.Setters.Add(new Setter(Button.BorderBrushProperty, bBorderBrush));

            // Alter controls.
            rootLayout.Resources.Add(typeof(Label), sLabel);
            rootLayout.Resources.Add(typeof(Button), sButton);

            // We done here broski.
            return true;
        }
    }
}
