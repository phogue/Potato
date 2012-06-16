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
            // Load our resources.
            ResourceDictionary tResources = new ResourceDictionary() {
                Source = new Uri("pack://application:,,,/Procon.UI.Default;component/Styles-All/Light/Light.xaml")
            };


            // Setup base styles.
            Grid rootLayout = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            if (rootLayout == null) return false;
            rootLayout.Resources.Add(typeof(Button),      tResources["StyleButton"]      as Style);
            rootLayout.Resources.Add(typeof(RadioButton), tResources["StyleRadioButton"] as Style);
            rootLayout.Resources.Add(typeof(TextBox),     tResources["StyleTextBox"]     as Style);
            rootLayout.Resources.Add(typeof(PasswordBox), tResources["StylePasswordBox"] as Style);
            rootLayout.Resources.Add(typeof(Label),       tResources["StyleLabel"]       as Style);
            rootLayout.Resources.Add(typeof(TextBlock),   tResources["StyleTextBlock"]   as Style);
            rootLayout.Resources.Add(typeof(Image),       tResources["StyleImage"]       as Style);


            // Setup default styles.
            Grid mainLayout = ExtensionApi.FindControl<Grid>(rootLayout, "MainLayout");
            Grid tutLayout  = ExtensionApi.FindControl<Grid>(rootLayout, "TutorialLayout");
            if (tutLayout == null || mainLayout == null) return false;
            mainLayout.Resources.Add(typeof(Button), tResources["StyleButtonDefault"] as Style);
            tutLayout.Resources.Add( typeof(Button), tResources["StyleButtonDefault"] as Style);


            // Setup special styles.
            Grid hdrLayout = ExtensionApi.FindControl<Grid>(mainLayout, "MainHeader");
            Grid navLayout = ExtensionApi.FindControl<Grid>(mainLayout, "MainNavigation");
            if (hdrLayout == null || navLayout == null) return false;
            hdrLayout.Resources.Add(typeof(Button), tResources["StyleButtonSpecial"] as Style);


            // Modify specific controls.
            Label     hdrUpTime  = ExtensionApi.FindControl<Label>(hdrLayout, "MainHeaderUpTime");
            Label     plrName    = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersTitleName");
            Label     plrMode    = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersTitleMode");
            DockPanel plrContent = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainPlayersListContent");
            DockPanel plrActions = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainPlayersListActions");
            DockPanel chtContent = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainChatContent");
            DockPanel chtTitle   = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainChatTitle");
            TextBox   chtBox     = ExtensionApi.FindControl<TextBox>(mainLayout, "MainChatBox");
            if (hdrUpTime == null || plrName == null || plrMode == null) return false;
            hdrUpTime.Foreground = tResources["BrushTextSoft"] as Brush;
            plrName.Foreground   = tResources["BrushTextSoft"] as Brush;
            plrMode.Foreground   = tResources["BrushTextSoft"] as Brush;
            chtBox.Background    = tResources["BrushChatBox"]  as Brush;


            // Update specific backgrounds.
            mainLayout.Background = tResources["BrushLight"]       as Brush;
            tutLayout.Background  = tResources["BrushSpotlight"]   as Brush;
            hdrLayout.Background  = tResources["BrushHeaderHover"] as Brush;
            navLayout.Background  = tResources["BrushBlueDot"]     as Brush;
            plrContent.Background = tResources["BrushOffset"]      as Brush;
            plrActions.Background = tResources["BrushEmphasis"]    as Brush;
            chtContent.Background = tResources["BrushChatMain"]    as Brush;
            chtTitle.Background   = tResources["BrushChatTitle"]   as Brush;

            return true;
        }
    }
}
