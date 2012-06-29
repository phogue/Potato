using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;

namespace Procon.UI.Default.Root.Main.Header
{
    [Extension(
        Alters    = new String[] { "MainLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Layout" })]
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
        { get { return "Main Header"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Header"];
        private InfinityDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Header"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            HeaderView view = new HeaderView();
            layout.Children.Add(view);

            // Commands
            tCmmds["Swap"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Swap Connections" button is clicked.
                x => {
                    ExtensionApi.Settings["View"].Value = "Configure";
                });
            #endregion

            // Setup the settings.
            if (ExtensionApi.Settings["View"].Value == null)
                ExtensionApi.Settings["View"].Value = "Configure";

            // Exit with good status.
            return true;
        }
    }
}
