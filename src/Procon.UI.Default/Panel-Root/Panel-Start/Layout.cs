using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Start.Layout
{
    [Extension(
        Alters    = new String[] { "RootLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Root Layout" })]
    public class Layout : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Start Layout"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid rootLayout = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            if  (rootLayout == null) return false;

            // Do what I need to setup my control.
            LayoutView lv = new LayoutView();
            rootLayout.Children.Add(lv);

            // Handles "Remote Interface" and "Local Interface" button clicks.  Also handles when the password changes.
            InstanceViewModel.PublicCommands["Start"]["Local"].Value    = new RelayCommand<Object>(SelectedLocalInterface);
            InstanceViewModel.PublicCommands["Start"]["Remote"].Value   = new RelayCommand<Object>(SelectedRemoteInterface, CanSelectRemoteInterface);
            InstanceViewModel.PublicCommands["Start"]["Password"].Value = new RelayCommand<String>(pass => InstanceViewModel.PublicProperties["Start"]["Remote"]["Pass"].Value = pass);

            // Exit with good status.
            return true;
        }

        // Handles how to proceed depending on whether the user clicks the local or remote interface buttons.
        private void SelectedLocalInterface(Object nothing)
        {
            InstanceViewModel.PublicProperties["Interface"].Value = ((InstanceViewModel)InstanceViewModel.PublicProperties["Procon"].Value).Interfaces.Single(x => x.IsLocal);
            InstanceViewModel.PublicProperties["Settings"]["IsFirstTime"].Value = false;
        }
        private void SelectedRemoteInterface(Object nothing)
        {
            InstanceViewModel                  tProcon = (InstanceViewModel)InstanceViewModel.PublicProperties["Procon"].Value;
            InfinityDictionary<String, Object> tRemote = InstanceViewModel.PublicProperties["Start"]["Remote"];
            InstanceViewModel.InterfaceHandler tEvent = null;

            // Setup the event to watch for the remote interface to be added.
            tEvent = (v, i) => {
                tProcon.InterfaceAdded -= tEvent;
                InstanceViewModel.PublicProperties["Interface"].Value = i;
                InstanceViewModel.PublicProperties["Settings"]["IsFirstTime"].Value = false;
            };
            tProcon.InterfaceAdded += tEvent;

            // Attempt to add the remote interface.
            tProcon.AddInterface(
                (String)tRemote["Host"].Value,
                UInt16.Parse((String)tRemote["Port"].Value),
                (String)tRemote["User"].Value,
                (String)tRemote["Pass"].Value);
        }
        private bool CanSelectRemoteInterface(Object nothing)
        {
            String tString;
            UInt16 tUInt16;
            InfinityDictionary<String, Object> tRemote = InstanceViewModel.PublicProperties["Start"]["Remote"];
            return
                InstanceViewModel.PublicProperties["Procon"] != null
                && (tString = tRemote["Host"].Value as String) != null
                && tString.Trim() != String.Empty
                && UInt16.TryParse((String)tRemote["Port"].Value, out tUInt16)
                && tUInt16 != 0
                && tUInt16 != UInt16.MaxValue
                && (tString = tRemote["User"].Value as String) != null
                && tString.Trim() != String.Empty
                && (tString = tRemote["Pass"].Value as String) != null
                && tString.Trim() != String.Empty;
        }
    }
}
