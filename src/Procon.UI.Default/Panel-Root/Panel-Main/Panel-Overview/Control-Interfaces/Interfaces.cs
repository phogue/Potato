using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Overview.Interfaces
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Interfaces : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Overview Interfaces"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Overview"]["Interfaces"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Overview"]["Interfaces"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainOverviewLayout");


            // Do what I need to setup my control.
            InterfacesView tView = new InterfacesView();
            tLayout.Children.Add(tView);
            

            // Commands.
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    tView.MainOverviewInterfacesAddContainer.VerticalOffset = tView.MainOverviewInterfacesHeader.ActualHeight;
                    tView.MainOverviewInterfacesAddContainer.IsOpen = true;
                    tView.MainOverviewInterfacesAddHost.Focus();
                });
            #endregion
            tCmmds["Add"]["Confirm"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Connect" button is clicked.
                x => {
                    ExtensionApi.Commands["Interface"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["User"].Value,
                            tProps["Pass"].Value
                        });
                    tView.MainOverviewInterfacesAddContainer.IsOpen = false;
                    tView.MainOverviewInterfacesAddHost.Text        = String.Empty;
                    tView.MainOverviewInterfacesAddPort.Text        = String.Empty;
                    tView.MainOverviewInterfacesAddUser.Text        = String.Empty;
                    tView.MainOverviewInterfacesAddPass.Password    = String.Empty;
                },
                x => {
                    return ExtensionApi.Commands["Interface"]["Add"].Value != null &&
                           ExtensionApi.Commands["Interface"]["Add"].Value.CanExecute(
                            new Object[] {
                                tProps["Host"].Value,
                                tProps["Port"].Value,
                                tProps["User"].Value,
                                tProps["Pass"].Value
                            });
                });
            #endregion
            tCmmds["Remove"].Value = new RelayCommand<InterfaceViewModel>(
            #region -- Handles when the "Remove" button is clicked.
                x => {
                    ExtensionApi.Commands["Interface"]["Remove"].Value.Execute(new Object[] {
                        x.Hostname,
                        x.Port.ToString()
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Interface"]["Remove"].Value != null &&
                           ExtensionApi.Commands["Interface"]["Remove"].Value.CanExecute(new Object[] {
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
