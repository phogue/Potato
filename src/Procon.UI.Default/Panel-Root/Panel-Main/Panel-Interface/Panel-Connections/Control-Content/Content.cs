using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Configure.Connections.Content
{
    [Extension(
        Alters    = new String[] { "MainConfigureConnectionsLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Configure Connections Layout" })]
    public class Content : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Configure Connections Content"; } }

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
            ContentView view = new ContentView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);
            
            // Commands.
            tCmmds["View"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when a connection is double-clicked.
                x => {
                    MouseButtonEventArgs e = x.Args as MouseButtonEventArgs;
                    if (e.ClickCount == 2)
                        ExtensionApi.Settings["View"].Value = "Connection";
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

            // Exit with good status.
            return true;
        }
    }
}
