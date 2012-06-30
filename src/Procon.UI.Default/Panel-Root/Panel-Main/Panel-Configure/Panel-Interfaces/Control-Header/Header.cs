using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Utils;
using Procon.UI.API.Commands;

namespace Procon.UI.Default.Root.Main.Configure.Interfaces.Header
{
    [Extension(
        Alters    = new String[] { "MainConfigureInterfacesLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Configure Interfaces Layout" })]
    public class Header : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Configure Interfaces Header"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Configure"]["Interfaces"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Configure"]["Interfaces"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConfigureInterfacesLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            HeaderView view = new HeaderView();
            layout.Children.Add(view);
            
            // Commands.
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    view.ConfigureInterfacesHeaderAddContainer.VerticalOffset = view.MainConfigureInterfacesHeader.ActualHeight;
                    view.ConfigureInterfacesHeaderAddContainer.IsOpen = true;
                    view.ConfigureInterfacesHeaderAddHost.Focus();
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
                    view.ConfigureInterfacesHeaderAddContainer.IsOpen = false;
                    view.ConfigureInterfacesHeaderAddHost.Text        = String.Empty;
                    view.ConfigureInterfacesHeaderAddPort.Text        = String.Empty;
                    view.ConfigureInterfacesHeaderAddUser.Text        = String.Empty;
                    view.ConfigureInterfacesHeaderAddPass.Password    = String.Empty;
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
