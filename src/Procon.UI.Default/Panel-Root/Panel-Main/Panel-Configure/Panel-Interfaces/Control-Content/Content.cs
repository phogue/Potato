using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Configure.Interfaces.Content
{
    [Extension(
        Alters    = new String[] { "MainConfigureInterfacesLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Configure Interfaces Layout" })]
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
        { get { return "Main Configure Interfaces Content"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Configure"]["Interfaces"];
        private InfinityDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Configure"]["Interfaces"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConfigureInterfacesLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            ContentView view = new ContentView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);
            
            // Commands.
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

            // Exit with good status.
            return true;
        }
    }
}
