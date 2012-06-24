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
            rootLayout.Resources[typeof(Button)]      = tResources["StyleButton"];
            rootLayout.Resources[typeof(RadioButton)] = tResources["StyleRadioButton"];
            rootLayout.Resources[typeof(TextBox)]     = tResources["StyleTextBox"];
            rootLayout.Resources[typeof(PasswordBox)] = tResources["StylePasswordBox"];
            rootLayout.Resources[typeof(Label)]       = tResources["StyleLabel"];
            rootLayout.Resources[typeof(TextBlock)]   = tResources["StyleTextBlock"];
            rootLayout.Resources[typeof(ListBoxItem)] = tResources["StyleListBoxItem"];
            rootLayout.Resources[typeof(ListBox)]     = tResources["StyleListBox"];
            rootLayout.Resources[typeof(Image)]       = tResources["StyleImage"];

            // Setup refined styles.
            Grid manLayout = ExtensionApi.FindControl<Grid>(rootLayout, "MainLayout");
            Grid tutLayout = ExtensionApi.FindControl<Grid>(rootLayout, "TutorialLayout");
            manLayout.Resources[typeof(Button)] = tResources["StyleButtonDefault"];
            tutLayout.Resources[typeof(Button)] = tResources["StyleButtonDefault"];

            // Setup optional styles.
            rootLayout.Resources["StyleButtonSpecial"] = tResources["StyleButtonSpecial"];

            // Setup control brushes.
            rootLayout.Resources["BrushControlNormal"] = tResources["BrushControlNormal"];
            rootLayout.Resources["BrushControlHover"]  = tResources["BrushControlHover"];
            rootLayout.Resources["BrushControlPress"]  = tResources["BrushControlPress"];
            rootLayout.Resources["BrushSpecialNormal"] = tResources["BrushSpecialNormal"];
            rootLayout.Resources["BrushSpecialHover"]  = tResources["BrushSpecialHover"];
            rootLayout.Resources["BrushSpecialPress"]  = tResources["BrushSpecialPress"];
            rootLayout.Resources["BrushBorder"]        = tResources["BrushBorder"];

            // Setup text brushes.
            rootLayout.Resources["BrushTextLight"]      = tResources["BrushTextLight"];
            rootLayout.Resources["BrushTextLightSoft"]  = tResources["BrushTextLightSoft"];
            rootLayout.Resources["BrushTextDark"]       = tResources["BrushTextDark"];
            rootLayout.Resources["BrushTextDarkSoft"]   = tResources["BrushTextDarkSoft"];
            rootLayout.Resources["BrushInputTextLight"] = tResources["BrushInputTextLight"];
            rootLayout.Resources["BrushInputTextDark"]  = tResources["BrushInputTextDark"];

            // Setup specific brushes.
            rootLayout.Resources["BrushLight"]         = tResources["BrushLight"];
            rootLayout.Resources["BrushMild"]          = tResources["BrushMild"];
            rootLayout.Resources["BrushDark"]          = tResources["BrushDark"];
            rootLayout.Resources["BrushHeaderLight"]   = tResources["BrushHeaderLight"];
            rootLayout.Resources["BrushEmphasisLight"] = tResources["BrushEmphasisLight"];
            rootLayout.Resources["BrushContentLight"]  = tResources["BrushContentLight"];
            rootLayout.Resources["BrushHeaderDark"]    = tResources["BrushHeaderDark"];
            rootLayout.Resources["BrushEmphasisDark"]  = tResources["BrushEmphasisDark"];
            rootLayout.Resources["BrushContentDark"]   = tResources["BrushContentDark"];

            // Setup team brushes.
            rootLayout.Resources["BrushTeam0Header"]  = tResources["BrushTeam0Header"];
            rootLayout.Resources["BrushTeam0Content"] = tResources["BrushTeam0Content"];
            rootLayout.Resources["BrushTeam1Header"]  = tResources["BrushTeam1Header"];
            rootLayout.Resources["BrushTeam1Content"] = tResources["BrushTeam1Content"];
            rootLayout.Resources["BrushTeam2Header"]  = tResources["BrushTeam2Header"];
            rootLayout.Resources["BrushTeam2Content"] = tResources["BrushTeam2Content"];
            rootLayout.Resources["BrushTeam3Header"]  = tResources["BrushTeam3Header"];
            rootLayout.Resources["BrushTeam3Content"] = tResources["BrushTeam3Content"];
            rootLayout.Resources["BrushTeam4Header"]  = tResources["BrushTeam4Header"];
            rootLayout.Resources["BrushTeam4Content"] = tResources["BrushTeam4Content"];

            // Background brushes.
            rootLayout.Resources["BrushSpotlight"]  = tResources["BrushSpotlight"];
            rootLayout.Resources["BrushHeader"]     = tResources["BrushHeader"];
            rootLayout.Resources["BrushNavigation"] = tResources["BrushNavigation"];


            //// Setup special styles.
            //ListBox vieIntrList = ExtensionApi.FindControl<ListBox>(manLayout, "MainViewsInterfacesList");
            //ListBox vieConnList = ExtensionApi.FindControl<ListBox>(manLayout, "MainViewsConnectionsList");
            ////vieIntrList.Resources["Background"] = tResources["BrushContentLight"];
            ////vieConnList.Resources["Background"] = tResources["BrushContentLight"];


            //// Players List
            //Label     plrName    = ExtensionApi.FindControl<Label>(manLayout, "MainPlayersListName");
            //Label     plrMode    = ExtensionApi.FindControl<Label>(manLayout, "MainPlayersListMode");
            //DockPanel plrContent = ExtensionApi.FindControl<DockPanel>(manLayout, "MainPlayersListContent");
            //DockPanel plrActions = ExtensionApi.FindControl<DockPanel>(manLayout, "MainPlayersListActions");
            //DockPanel vieIntrTtl = ExtensionApi.FindControl<DockPanel>(manLayout, "MainViewsInterfacesHeader");
            //DockPanel vieConnTtl = ExtensionApi.FindControl<DockPanel>(manLayout, "MainViewsConnectionsHeader");
            //DockPanel chtContent = ExtensionApi.FindControl<DockPanel>(manLayout, "MainChatContent");
            //DockPanel chtTitle   = ExtensionApi.FindControl<DockPanel>(manLayout, "MainChatTitle");
            //TextBox   chtBox     = ExtensionApi.FindControl<TextBox>(manLayout, "MainChatBox");
            //plrName.Foreground    = tResources["BrushTextSoft"]     as Brush;
            //plrMode.Foreground    = tResources["BrushTextSoft"]     as Brush;
            //plrContent.Background = tResources["BrushOffset"]       as Brush;
            //plrActions.Background = tResources["BrushEmphasis"]     as Brush;
            //vieIntrTtl.Background = tResources["BrushEmphasis"]     as Brush;
            //vieConnTtl.Background = tResources["BrushEmphasis"]     as Brush;
            //chtContent.Background = tResources["BrushDarkEmphasis"] as Brush;
            //chtTitle.Background   = tResources["BrushDarkHeader"]   as Brush;
            //chtBox.Background     = tResources["BrushChatBox"]      as Brush;


            return true;
        }
    }
}
