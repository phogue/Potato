using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.Core.Interfaces.Connections.TextCommands;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

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


            // Setup default property values.
            if (ExtensionApi.Settings["Pane"].Value == null)
                ExtensionApi.Settings["Pane"].Value = "Players";
            tView.MainConnectionNavigationPlayers.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Players";
            tView.MainConnectionNavigationMaps.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Maps";
            tView.MainConnectionNavigationBans.IsChecked     = (String)ExtensionApi.Settings["Pane"].Value == "Bans";
            tView.MainConnectionNavigationPlugins.IsChecked  = (String)ExtensionApi.Settings["Pane"].Value == "Plugins";
            tView.MainConnectionNavigationSettings.IsChecked = (String)ExtensionApi.Settings["Pane"].Value == "Settings";


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
            tCmmds["Omni"]["Preview"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles whenever the omni box is typed in.
                x => {
                    String tString = tProps["Omni"]["Text"].Value as String;
                    if (tString != null)
                        if (tString.Trim() != String.Empty && ExtensionApi.Connection != null) {
                            tProps["Omni"]["Open"].Value = false;
                            ExtensionApi.Connection.TextPreview(tString);
                        }
                });
            #endregion
            tCmmds["Omni"]["Command"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles whenever the omni box has enter pressed.
                x => {
                    KeyEventArgs tArgs = x.Args as KeyEventArgs;
                    if (tArgs != null && tArgs.Key == Key.Enter) {
                        String tString = tProps["Omni"]["Text"].Value as String;
                        if (tString != null)
                            if (tString.Trim() != String.Empty && ExtensionApi.Connection != null) {
                                ExtensionApi.Connection.TextCommand(tString);
                                tProps["Omni"]["Text"].Value = String.Empty;
                                tProps["Omni"]["Open"].Value = false;
                            }
                    }
                });
            #endregion


            // Information Management.
            ConnectionViewModel tConnection = null;

            TextCommandController.TextCommandEventHandler tOmni = (s, e) =>
            #region -- Calls tProps["Omni"]
            {
                if (e.EventType == TextCommandEventType.Previewed && e.Speaker == null)
                    ((Action<TextCommand, Match, List<TextCommand>>)tProps["Omni"].Value)(e.Command, e.Match, e.AlternativeCommands);
            };
            #endregion

            tProps["Omni"].Value = new Action<TextCommand, Match, List<TextCommand>>(
            #region -- Manages the information displayed in the command preview popup.
                (c, m, l) => {
                    if ((String)tProps["Omni"]["Text"].Value == m.Text) {
                        tProps["Omni"]["Open"].Value        = true;
                        tProps["Omni"]["Commands"].Value    = String.Join(", ", c.Commands);
                        tProps["Omni"]["Description"].Value = c.DescriptionKey;
                        
                        tProps["Omni"]["Match"].Value   = m;
                        tProps["Omni"]["Others"].Value  = l;
                    }
                });
            #endregion

            tProps["Swap"].Value = new Action<ConnectionViewModel>(
            #region -- Changes the lists being viewed.
                x => {
                    // Cleanup old stuff.
                    if (tConnection != null) {
                        tConnection.TextEvent -= tOmni;
                        tConnection = null;
                    }
                    // Setup new stuff.
                    if (x != null) {
                        tConnection = x;
                        tConnection.TextEvent += tOmni;
                    }
                });
            #endregion

            ExtensionApi.Properties["Connection"].PropertyChanged += (s, e) => {
                new Thread(() => {
                    ((Action<ConnectionViewModel>)tProps["Swap"].Value)(ExtensionApi.Connection);
                }).Start();
            };


            // Exit with good status.
            return true;
        }
    }
}
