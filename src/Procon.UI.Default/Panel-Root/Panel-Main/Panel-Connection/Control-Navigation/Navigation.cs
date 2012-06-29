using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.Default.Controls;

namespace Procon.UI.Default.Root.Main.Connection.Navigation
{
    [Extension(
        Alters    = new String[] { "MainConnectionLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Connection Layout" })]
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
        private InfinityDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Navigation"];
        private InfinityDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Navigation"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            NavigationView view = new NavigationView();
            layout.Children.Add(view);
            
            // Commands
            tCmmds["Swap"].Value = new RelayCommand<RadioImageButton>(
            #region -- Handles when a radio button is clicked.
                x => {
                    if (view.ConnectionNavigationPlayers == x)
                        ExtensionApi.Settings["Pane"].Value = "Players";
                    else if (view.ConnectionNavigationMaps == x)
                        ExtensionApi.Settings["Pane"].Value = "Maps";
                    else if (view.ConnectionNavigationBans == x)
                        ExtensionApi.Settings["Pane"].Value = "Bans";
                    else if (view.ConnectionNavigationPlugins == x)
                        ExtensionApi.Settings["Pane"].Value = "Plugins";
                    else if (view.ConnectionNavigationSettings == x)
                        ExtensionApi.Settings["Pane"].Value = "Settings";
                    else if (view.ConnectionNavigationOptions == x)
                        ExtensionApi.Settings["Pane"].Value = "Options";
                },
                x => {
                    return ExtensionApi.Connection != null;
                });
            #endregion

            // Allow other panes to be used other than these.
            ExtensionApi.Settings["Pane"].PropertyChanged += (s, e) => {
                view.ConnectionNavigationPlayers.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Players";
                view.ConnectionNavigationMaps.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Maps";
                view.ConnectionNavigationBans.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Bans";
                view.ConnectionNavigationPlugins.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Plugins";
                view.ConnectionNavigationSettings.IsChecked = (String)ExtensionApi.Settings["Pane"].Value == "Settings";
                view.ConnectionNavigationOptions.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Options";
            };
            ExtensionApi.Settings["Pane"].Value = ExtensionApi.Settings["Pane"].Value;

            // Exit with good status.
            return true;
        }
    }
}
