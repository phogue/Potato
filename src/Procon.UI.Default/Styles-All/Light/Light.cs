using System;
using System.Windows;
using System.Windows.Controls;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.ViewModels;

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
            rootLayout.Resources["StyleButtonSpecial"]    = tResources["StyleButtonSpecial"];
            rootLayout.Resources["StyleListBoxItemColor"] = tResources["StyleListBoxItemColor"];

            // Setup templates.
            rootLayout.Resources[new DataTemplateKey(typeof(ConnectionViewModel))] = tResources["DataTemplateConnectionViewModel"];
            rootLayout.Resources[new DataTemplateKey(typeof(InterfaceViewModel))]  = tResources["DataTemplateInterfaceViewModel"];
            rootLayout.Resources[new DataTemplateKey(typeof(Chat))]                = tResources["DataTemplateChat"];
            rootLayout.Resources[new DataTemplateKey(typeof(Player))]              = tResources["DataTemplatePlayer"];

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

            // Chat box brushes.
            rootLayout.Resources["BrushChatTimestamp"] = tResources["BrushChatTimestamp"];

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

            return true;
        }
    }
}
