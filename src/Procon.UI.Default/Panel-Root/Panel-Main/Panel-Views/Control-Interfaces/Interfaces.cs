using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Views.Interfaces
{
    [Extension(
        Alters    = new String[] { "MainViewsLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Views Layout" })]
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
        { get { return "Main Views Interfaces"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Views"]["Interfaces"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Views"]["Interfaces"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainViewsLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            InterfacesView view = new InterfacesView();
            layout.Children.Add(view);
            
            // Commands.
            List<PlayerViewModel> tSelectedPlayers = new List<PlayerViewModel>();
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    view.MainViewsInterfacesAddContainer.VerticalOffset = view.MainViewsInterfacesHeader.ActualHeight;
                    view.MainViewsInterfacesAddContainer.IsOpen = true;
                    view.MainViewsInterfacesAddHost.Focus();
                });
            #endregion
            tCmmds["Add"]["Confirm"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Connect" button is clicked.
                x => {
                    ViewModelBase.PublicCommands["Interface"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["User"].Value,
                            tProps["Pass"].Value
                        });
                    view.MainViewsInterfacesAddContainer.IsOpen = false;
                    view.MainViewsInterfacesAddHost.Text        = String.Empty;
                    view.MainViewsInterfacesAddPort.Text        = String.Empty;
                    view.MainViewsInterfacesAddUser.Text        = String.Empty;
                    view.MainViewsInterfacesAddPass.Password    = String.Empty;
                },
                x => {
                    return ViewModelBase.PublicCommands["Interface"]["Add"].Value != null &&
                           ViewModelBase.PublicCommands["Interface"]["Add"].Value.CanExecute(
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
                    ViewModelBase.PublicCommands["Interface"]["Remove"].Value.Execute(new Object[] {
                        x.Hostname,
                        x.Port.ToString()
                    });
                },
                x => {
                    return x != null &&
                           ViewModelBase.PublicCommands["Interface"]["Remove"].Value != null &&
                           ViewModelBase.PublicCommands["Interface"]["Remove"].Value.CanExecute(new Object[] {
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

            // Exit with good status.
            return true;
        }
    }
}
