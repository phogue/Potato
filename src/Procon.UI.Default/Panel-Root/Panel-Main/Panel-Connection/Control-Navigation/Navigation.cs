using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Navigation
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Navigation : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Connection Navigation"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Navigation"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Navigation"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");


            // Do what I need to setup my control.
            NavigationView tView = new NavigationView();
            tLayout.Children.Add(tView);


            // Commands.
            tCmmds["Swap"].Value = new RelayCommand<RadioButton>(
            #region -- Handles when a radio button is clicked.
                x => {
                    if (x.Name == "MainConnectionNavigationPlayers")
                        ExtensionApi.Settings["Pane"].Value = "Players";
                    else if (x.Name == "MainConnectionNavigationMaps")
                        ExtensionApi.Settings["Pane"].Value = "Maps";
                    else if (x.Name == "MainConnectionNavigationBans")
                        ExtensionApi.Settings["Pane"].Value = "Bans";
                    else if (x.Name == "MainConnectionNavigationPlugins")
                        ExtensionApi.Settings["Pane"].Value = "Plugins";
                    else if (x.Name == "MainConnectionNavigationSettings")
                        ExtensionApi.Settings["Pane"].Value = "Settings";
                });
            #endregion


            // Setup the default settings.
            if (ExtensionApi.Settings["Pane"].Value == null)
                ExtensionApi.Settings["Pane"].Value = "Players";
            tView.MainConnectionNavigationPlayers.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Players";
            tView.MainConnectionNavigationMaps.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Maps";
            tView.MainConnectionNavigationBans.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Bans";
            tView.MainConnectionNavigationPlugins.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Plugins";
            tView.MainConnectionNavigationSettings.IsChecked = (String)ExtensionApi.Settings["Pane"].Value == "Settings";


            // Exit with good status.
            return true;
        }
    }
}
