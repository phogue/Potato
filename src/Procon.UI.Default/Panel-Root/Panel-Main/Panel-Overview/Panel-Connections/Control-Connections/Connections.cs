using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Overview.Connections
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Connections : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Main Overview Connections"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Overview"]["Connections"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Overview"]["Connections"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainOverviewLayout");


            // Do what I need to setup my control.
            ConnectionsView tView = new ConnectionsView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);
            

            // Setup default property values.
            tProps["Additional"].Value = String.Empty;
            

            // Commands.
            tCmmds["View"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when a connection is selected.
                x => {
                    if (ExtensionApi.Connection == x.Parameter)
                        ExtensionApi.Settings["View"].Value = "Connection";
                });
            #endregion
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    tView.MainOverviewConnectionsAddContainer.VerticalOffset = tView.MainOverviewConnectionsHeader.ActualHeight;
                    tView.MainOverviewConnectionsAddContainer.IsOpen = true;
                    tView.MainOverviewConnectionsAddHost.Focus();
                },
                x => {
                    return ExtensionApi.Interface != null && (ExtensionApi.Interface.IsLocal || ExtensionApi.Interface.ConnectionState == Net.ConnectionState.Ready);
                });
            #endregion
            tCmmds["Add"]["Confirm"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Connect" button is clicked.
                x => {
                    ExtensionApi.Commands["Connection"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Type"].Value.ToString(),
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["Pass"].Value,
                            tProps["Additional"].Value
                        });
                    tView.MainOverviewConnectionsAddContainer.IsOpen   = false;
                    tView.MainOverviewConnectionsAddHost.Text          = String.Empty;
                    tView.MainOverviewConnectionsAddPort.Text          = String.Empty;
                    tView.MainOverviewConnectionsAddType.SelectedIndex = 0;
                    tView.MainOverviewConnectionsAddPass.Password      = String.Empty;
                    tView.MainOverviewConnectionsAddAdditional.Text    = String.Empty;
                },
                x => {
                    return ExtensionApi.Commands["Connection"]["Add"].Value != null &&
                           ExtensionApi.Commands["Connection"]["Add"].Value.CanExecute(
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
                    ExtensionApi.Commands["Connection"]["Remove"].Value.Execute(new Object[] {
                        x.GameType.ToString(),
                        x.Hostname,
                        x.Port.ToString()
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Connection"]["Remove"].Value != null &&
                           ExtensionApi.Commands["Connection"]["Remove"].Value.CanExecute(new Object[] {
                               x.GameType.ToString(),
                               x.Hostname,
                               x.Port.ToString()
                           });
                });
            #endregion
            tCmmds["Pass"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the password changes.
                x => {
                    PasswordBox tElement = x.Sender as PasswordBox;
                    if (tElement != null)
                        tProps["Pass"].Value = tElement.Password;
                });
            #endregion


            // Exit with good status.
            return true;
        }
    }
}
