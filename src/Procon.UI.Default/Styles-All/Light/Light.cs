using System;
using System.Windows;
using System.Windows.Controls;
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
            rootLayout.Resources.Add(typeof(Button),      tResources["StyleButton"]);
            rootLayout.Resources.Add(typeof(RadioButton), tResources["StyleRadioButton"]);
            rootLayout.Resources.Add(typeof(TextBox),     tResources["StyleTextBox"]);
            rootLayout.Resources.Add(typeof(PasswordBox), tResources["StylePasswordBox"]);
            rootLayout.Resources.Add(typeof(Label),       tResources["StyleLabel"]);
            rootLayout.Resources.Add(typeof(TextBlock),   tResources["StyleTextBlock"]);
            rootLayout.Resources.Add(typeof(ListBox),     tResources["StyleListBox"]);
            rootLayout.Resources.Add(typeof(Image),       tResources["StyleImage"]);


            // Setup default styles.
            Grid mainLayout = ExtensionApi.FindControl<Grid>(rootLayout, "MainLayout");
            Grid tutLayout  = ExtensionApi.FindControl<Grid>(rootLayout, "TutorialLayout");
            if (tutLayout == null || mainLayout == null) return false;
            mainLayout.Resources.Add(typeof(Button), tResources["StyleButtonDefault"]);
            tutLayout.Resources.Add( typeof(Button), tResources["StyleButtonDefault"]);


            // Setup special styles.
            Grid    hdrLayout   = ExtensionApi.FindControl<Grid>(mainLayout, "MainHeader");
            Grid    navLayout   = ExtensionApi.FindControl<Grid>(mainLayout, "MainNavigation");
            ListBox vieIntrList = ExtensionApi.FindControl<ListBox>(mainLayout, "MainViewsInterfacesList");
            ListBox vieConnList = ExtensionApi.FindControl<ListBox>(mainLayout, "MainViewsConnectionsList");
            if (hdrLayout == null || navLayout == null || vieConnList == null || vieIntrList == null) return false;
            hdrLayout.Resources.Add(typeof(Button), tResources["StyleButtonSpecial"]);
            vieIntrList.Resources.Add("Background", tResources["BrushMedium"]);
            vieConnList.Resources.Add("Background", tResources["BrushMedium"]);


            // Modify specific controls.
            Label     plrName    = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersListName");
            Label     plrMode    = ExtensionApi.FindControl<Label>(mainLayout, "MainPlayersListMode");
            DockPanel plrContent = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainPlayersListContent");
            DockPanel plrActions = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainPlayersListActions");
            DockPanel vieIntrTtl = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainViewsInterfacesHeader");
            DockPanel vieConnTtl = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainViewsConnectionsHeader");
            DockPanel chtContent = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainChatContent");
            DockPanel chtTitle   = ExtensionApi.FindControl<DockPanel>(mainLayout, "MainChatTitle");
            TextBox   chtBox     = ExtensionApi.FindControl<TextBox>(mainLayout, "MainChatBox");
            if (plrName    == null || plrMode    == null || plrContent == null || plrActions == null ||
                vieIntrTtl == null || vieConnTtl == null || chtContent == null || chtTitle   == null || chtBox == null) return false;
            plrName.Foreground    = tResources["BrushTextSoft"]     as Brush;
            plrMode.Foreground    = tResources["BrushTextSoft"]     as Brush;
            plrContent.Background = tResources["BrushOffset"]       as Brush;
            plrActions.Background = tResources["BrushEmphasis"]     as Brush;
            vieIntrTtl.Background = tResources["BrushEmphasis"]     as Brush;
            vieConnTtl.Background = tResources["BrushEmphasis"]     as Brush;
            chtContent.Background = tResources["BrushDarkEmphasis"] as Brush;
            chtTitle.Background   = tResources["BrushDarkHeader"]   as Brush;
            chtBox.Background     = tResources["BrushChatBox"]      as Brush;


            // Update specific backgrounds.
            mainLayout.Background = tResources["BrushLight"]       as Brush;
            tutLayout.Background  = tResources["BrushSpotlight"]   as Brush;
            hdrLayout.Background  = tResources["BrushHeaderHover"] as Brush;
            navLayout.Background  = tResources["BrushBlueDot"]     as Brush;


            return true;
        }
    }
}
