using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;
using Procon.UI.Default.Controls;

namespace Procon.UI.Default.Root.Main.Navigation
{
    [Extension(
        Alters    = new String[] { "MainLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Layout" })]
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
        { get { return "Main Navigation"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Navigation"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Navigation"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            NavigationView view = new NavigationView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);
            
            // Commands
            tCmmds["Swap"].Value = new RelayCommand<RadioImageButton>(
                // -- Handles when the "Swap Connections" button is clicked.
                x => {
                    if (view.MainNavigationPlayers == x)
                        ExtensionApi.Settings["View"].Value = "Players";
                    else if (view.MainNavigationMaps == x)
                        ExtensionApi.Settings["View"].Value = "Maps";
                    else if (view.MainNavigationBans == x)
                        ExtensionApi.Settings["View"].Value = "Bans";
                    else if (view.MainNavigationPlugins == x)
                        ExtensionApi.Settings["View"].Value = "Plugins";
                    else if (view.MainNavigationSettings == x)
                        ExtensionApi.Settings["View"].Value = "Settings";
                    else if (view.MainNavigationOptions == x)
                        ExtensionApi.Settings["View"].Value = "Options";
                },
                x => {
                    return ExtensionApi.Connection != null;
                });

            // Allow other views to be used other than these.
            ExtensionApi.Settings["View"].PropertyChanged += (s, e) => {
                view.MainNavigationPlayers.IsChecked  = (String)ExtensionApi.Settings["View"].Value == "Players";
                view.MainNavigationMaps.IsChecked     = (String)ExtensionApi.Settings["View"].Value == "Maps";
                view.MainNavigationBans.IsChecked     = (String)ExtensionApi.Settings["View"].Value == "Bans";
                view.MainNavigationPlugins.IsChecked  = (String)ExtensionApi.Settings["View"].Value == "Plugins";
                view.MainNavigationSettings.IsChecked = (String)ExtensionApi.Settings["View"].Value == "Settings";
                view.MainNavigationOptions.IsChecked  = (String)ExtensionApi.Settings["View"].Value == "Options";
            };
            ExtensionApi.Settings["View"].Value = ExtensionApi.Settings["View"].Value;

            // Exit with good status.
            return true;
        }
    }
}
