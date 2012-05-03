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
            Grid rootLayout  = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            Grid startLayout = ExtensionApi.FindControl<Grid>(root, "StartLayout");
            if  (rootLayout == null || startLayout == null) return false;

            // Some colors for various controls.
            Color cTextColor          = Color.FromArgb(255, 35,   31,  32);
            Color cControlLightColor  = Color.FromArgb(244, 244, 244, 244);
            Color cControlMediumColor = Color.FromArgb(255, 230, 230, 230);
            Color cControlDarkColor   = Color.FromArgb(255, 205, 205, 205);
            Color cBorderLightColor   = Color.FromArgb(255, 252, 252, 252);
            Color cBorderDarkColor    = Color.FromArgb(255, 163, 163, 163);

            // Some brushes using the colors we setup.
            SolidColorBrush bTextBrush = new SolidColorBrush(cTextColor);
            RadialGradientBrush bStartBackgroundBrush = new RadialGradientBrush();
            bStartBackgroundBrush.GradientOrigin = new Point(0.5, 0.5);
            bStartBackgroundBrush.Center         = new Point(0.5, 0.65);
            bStartBackgroundBrush.RadiusX        = 0.4;
            bStartBackgroundBrush.RadiusY        = 0.6;
            bStartBackgroundBrush.GradientStops.Add(new GradientStop(cControlLightColor,  0.0));
            bStartBackgroundBrush.GradientStops.Add(new GradientStop(cControlLightColor,  0.2));
            bStartBackgroundBrush.GradientStops.Add(new GradientStop(cControlDarkColor,   1.0));
            LinearGradientBrush bControlBrush = new LinearGradientBrush();
            bControlBrush.StartPoint = new Point(0.5, 0);
            bControlBrush.EndPoint   = new Point(0.5, 1);
            bControlBrush.GradientStops.Add(new GradientStop(cControlLightColor,  0.0));
            bControlBrush.GradientStops.Add(new GradientStop(cControlMediumColor, 1.0));
            LinearGradientBrush bBorderBrush = new LinearGradientBrush();
            bBorderBrush.StartPoint = new Point(0.5, 0);
            bBorderBrush.EndPoint   = new Point(0.5, 1);
            bBorderBrush.GradientStops.Add(new GradientStop(cBorderLightColor, 0.0));
            bBorderBrush.GradientStops.Add(new GradientStop(cBorderDarkColor, 1.0));
            SolidColorBrush bSolidBorderBrush = new SolidColorBrush(cControlDarkColor);

            // Setup my own styles for various controls.
            Style sButton      = new Style(typeof(Button),      (Style)root.FindResource(typeof(Button)));
            Style sLabel       = new Style(typeof(Label),       (Style)root.FindResource(typeof(Label)));
            Style sTextBlock   = new Style(typeof(TextBlock),   (Style)root.FindResource(typeof(TextBlock)));
            Style sTextBox     = new Style(typeof(TextBox),     (Style)root.FindResource(typeof(TextBox)));
            Style sPasswordBox = new Style(typeof(PasswordBox), (Style)root.FindResource(typeof(PasswordBox)));

            /* Button Style */ {
                sButton.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0, 10, 0, 10)));
                sButton.Setters.Add(new Setter(Button.BackgroundProperty,  bControlBrush));
                sButton.Setters.Add(new Setter(Button.BorderBrushProperty, bBorderBrush));
                sButton.Setters.Add(new Setter(Button.ForegroundProperty, bTextBrush));
                sButton.Setters.Add(new Setter(Button.FontSizeProperty, 12.0));
                sButton.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.Bold));
                sButton.Setters.Add(new Setter(Button.FontFamilyProperty, new FontFamily("Arial")));
            }
            /* Label Style */ {
                sLabel.Setters.Add(new Setter(Label.PaddingProperty, new Thickness(0, 10, 0, 10)));
                sLabel.Setters.Add(new Setter(Label.VerticalAlignmentProperty, VerticalAlignment.Center));
                sLabel.Setters.Add(new Setter(Label.ForegroundProperty, bTextBrush));
                sLabel.Setters.Add(new Setter(Label.FontSizeProperty, 12.0));
                sLabel.Setters.Add(new Setter(Label.FontWeightProperty, FontWeights.Bold));
                sLabel.Setters.Add(new Setter(Label.FontFamilyProperty, new FontFamily("Arial")));
            }
            /* TextBlock Style */ {
                sTextBlock.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(0, 10, 0, 10)));
                sTextBlock.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Top));
                sTextBlock.Setters.Add(new Setter(TextBlock.ForegroundProperty, bTextBrush));
                sTextBlock.Setters.Add(new Setter(TextBlock.FontSizeProperty, 12.0));
                sTextBlock.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
                sTextBlock.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily("Arial")));
            }
            /* TextBox Style */ {
                sTextBox.Setters.Add(new Setter(TextBox.BorderBrushProperty, bSolidBorderBrush));
                sTextBox.Setters.Add(new Setter(TextBox.FontSizeProperty, 12.0));
                sTextBox.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            }
            /* PasswordBox Style */ {
                sPasswordBox.Setters.Add(new Setter(PasswordBox.BorderBrushProperty, bSolidBorderBrush));
                sPasswordBox.Setters.Add(new Setter(PasswordBox.FontSizeProperty, 12.0));
                sPasswordBox.Setters.Add(new Setter(PasswordBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            }

            // Apply the styles where they're needed.
            rootLayout.Resources.Add(typeof(Button),      sButton);
            rootLayout.Resources.Add(typeof(Label),       sLabel);
            rootLayout.Resources.Add(typeof(TextBlock),   sTextBlock);
            rootLayout.Resources.Add(typeof(TextBox),     sTextBox);
            rootLayout.Resources.Add(typeof(PasswordBox), sPasswordBox);
            startLayout.Background = bStartBackgroundBrush;

            return true;
        }
    }
}
