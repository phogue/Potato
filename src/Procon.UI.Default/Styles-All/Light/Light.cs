using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Procon.UI.API;

namespace Procon.UI.Default.Styles.Light
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
            Grid rootLayout     = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            Grid tutorialLayout = ExtensionApi.FindControl<Grid>(root, "TutorialLayout");
            Grid mainLayout     = ExtensionApi.FindControl<Grid>(root, "MainLayout");
            if  (rootLayout == null || tutorialLayout == null || mainLayout == null) return false;

            Grid headerLayout     = ExtensionApi.FindControl<Grid>(mainLayout, "MainHeader");
            Grid navigationLayout = ExtensionApi.FindControl<Grid>(mainLayout, "MainNavigation");
            if (headerLayout == null || navigationLayout == null) return false;

            RadioButton navPlayers  = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationPlayers");
            RadioButton navMaps     = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationMaps");
            RadioButton navBans     = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationBans");
            RadioButton navPlugins  = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationPlugins");
            RadioButton navSettings = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationSettings");
            RadioButton navOptions  = ExtensionApi.FindControl<RadioButton>(navigationLayout, "MainNavigationOptions");
            if (navPlayers == null || navMaps == null || navBans == null || navPlugins == null || navSettings == null || navOptions == null) return false;

            Label headerUpTime = ExtensionApi.FindControl<Label>(headerLayout, "MainHeaderUpTime");
            Label playersName  = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersTitleName");
            Label playersMode  = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersTitleMode");
            if (headerUpTime == null || playersName == null || playersMode == null) return false;

            // Load our resources.
            ResourceDictionary tResources = new ResourceDictionary() {
                Source = new Uri("pack://application:,,,/Procon.UI.Default;component/Styles-All/Light/Light.xaml")
            };

            // Setup the default styles.
            rootLayout.Resources.Add(typeof(Button),      tResources["StyleButton"]      as Style);
            rootLayout.Resources.Add(typeof(RadioButton), tResources["StyleRadioButton"] as Style);
            rootLayout.Resources.Add(typeof(TextBox),     tResources["StyleTextBox"]     as Style);
            rootLayout.Resources.Add(typeof(PasswordBox), tResources["StylePasswordBox"] as Style);
            rootLayout.Resources.Add(typeof(Label),       tResources["StyleLabel"]       as Style);
            rootLayout.Resources.Add(typeof(TextBlock),   tResources["StyleTextBlock"]   as Style);
            rootLayout.Resources.Add(typeof(Image),       tResources["StyleImage"]       as Style);

            // Setup the page-specific styles.
            tutorialLayout.Resources.Add(typeof(Button), tResources["StyleButtonDefault"] as Style);
            headerLayout.Resources.Add(typeof(Button),   tResources["StyleButtonSpecial"] as Style);

            // Setup the control-specific styles.
            navPlayers.Style  = tResources["StyleRadioButtonPlayers"]  as Style;
            navMaps.Style     = tResources["StyleRadioButtonMaps"]     as Style;
            navBans.Style     = tResources["StyleRadioButtonBans"]     as Style;
            navPlugins.Style  = tResources["StyleRadioButtonPlugins"]  as Style;
            navSettings.Style = tResources["StyleRadioButtonSettings"] as Style;
            navOptions.Style  = tResources["StyleRadioButtonOptions"]  as Style;

            // Setup various properties for controls.
            headerUpTime.Foreground = tResources["BrushTextSoft"] as Brush;
            playersName.Foreground  = tResources["BrushTextSoft"] as Brush;
            playersMode.Foreground  = tResources["BrushTextSoft"] as Brush;

            // Setup the backgrounds for some controls.
            tutorialLayout.Background   = tResources["BrushSpotlight"]   as Brush;
            mainLayout.Background       = tResources["BrushLight"]       as Brush;
            headerLayout.Background     = tResources["BrushHeaderHover"] as Brush;
            navigationLayout.Background = tResources["BrushBlueDot"]     as Brush;

            return true;
        }
    }
}
