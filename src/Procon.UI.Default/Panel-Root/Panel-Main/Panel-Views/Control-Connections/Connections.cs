using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Views.Connections
{
    [Extension(
        Alters    = new String[] { "MainViewsLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Views Layout" })]
    public class Connections : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Views Connections"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Views"]["Connections"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Views"]["Connections"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainViewsLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            ConnectionsView view = new ConnectionsView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);
            
            // Commands.
            List<PlayerViewModel> tSelectedPlayers = new List<PlayerViewModel>();
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    view.MainViewsConnectionsAddContainer.VerticalOffset = view.MainViewsConnectionsHeader.ActualHeight;
                    view.MainViewsConnectionsAddContainer.IsOpen = true;
                    view.MainViewsConnectionsAddHost.Focus();
                },
                x => {
                    return ExtensionApi.Interface != null && (ExtensionApi.Interface.IsLocal || ExtensionApi.Interface.ConnectionState == Net.ConnectionState.Ready);
                });
            #endregion
            tCmmds["Add"]["Confirm"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Connect" button is clicked.
                x => {
                    ViewModelBase.PublicCommands["Connection"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Type"].Value.ToString(),
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["Pass"].Value,
                            tProps["Additional"].Value
                        });
                    view.MainViewsConnectionsAddContainer.IsOpen   = false;
                    view.MainViewsConnectionsAddHost.Text          = String.Empty;
                    view.MainViewsConnectionsAddPort.Text          = String.Empty;
                    view.MainViewsConnectionsAddType.SelectedIndex = 0;
                    view.MainViewsConnectionsAddPass.Password      = String.Empty;
                },
                x => {
                    return ViewModelBase.PublicCommands["Connection"]["Add"].Value != null &&
                           ViewModelBase.PublicCommands["Connection"]["Add"].Value.CanExecute(
                            new Object[] {
                                tProps["Type"].Value.ToString(),
                                tProps["Host"].Value,
                                tProps["Port"].Value,
                                tProps["Pass"].Value,
                                tProps["Additional"].Value
                            });
                });
            #endregion
            tCmmds["Remove"].Value = new RelayCommand<ConnectionViewModel>(
            #region -- Handles when the "Remove" button is clicked.
                x => {
                    ViewModelBase.PublicCommands["Connection"]["Remove"].Value.Execute(new Object[] {
                        x.GameType.ToString(),
                        x.Hostname,
                        x.Port.ToString()
                    });
                },
                x => {
                    return x != null &&
                           ViewModelBase.PublicCommands["Connection"]["Remove"].Value != null &&
                           ViewModelBase.PublicCommands["Connection"]["Remove"].Value.CanExecute(new Object[] {
                               x.GameType.ToString(),
                               x.Hostname,
                               x.Port.ToString()
                           });
                });
            #endregion
            tCmmds["Pass"].Value = new RelayCommand<String>(
            #region -- Handles when the password changes.
                pass => {
                    tProps["Pass"].Value = pass;
                });
            #endregion

            tProps["Additional"].Value = String.Empty;

            // Exit with good status.
            return true;
        }
    }
}
