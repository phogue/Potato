using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Configure.Connections.Header
{
    [Extension(
        Alters    = new String[] { "MainConfigureConnectionsLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Configure Connections Layout" })]
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
        { get { return "Main Configure Connections Header"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Configure"]["Connections"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Configure"]["Connections"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConfigureConnectionsLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            HeaderView view = new HeaderView();
            layout.Children.Add(view);

            tProps["Additional"].Value = String.Empty;
            
            // Commands.
            tCmmds["Add"]["Open"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Add" button is clicked.
                x => {
                    view.ConfigureConnectionsHeaderAddContainer.VerticalOffset = view.MainConfigureConnectionsHeader.ActualHeight;
                    view.ConfigureConnectionsHeaderAddContainer.IsOpen = true;
                    view.ConfigureConnectionsHeaderAddHost.Focus();
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
                    view.ConfigureConnectionsHeaderAddContainer.IsOpen   = false;
                    view.ConfigureConnectionsHeaderAddHost.Text          = String.Empty;
                    view.ConfigureConnectionsHeaderAddPort.Text          = String.Empty;
                    view.ConfigureConnectionsHeaderAddType.SelectedIndex = 0;
                    view.ConfigureConnectionsHeaderAddPass.Password      = String.Empty;
                    view.ConfigureConnectionsHeaderAddAdditional.Text    = String.Empty;
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
