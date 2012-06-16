using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Navigation
{
    [Extension(
        Alters    = new String[] { "MainLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Layout" })]
    public class Navigation : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Navigation"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Navigation"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Navigation"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            NavigationView view = new NavigationView();
            Grid.SetRow(view, 1);
            tProps["Players"].Value = true;
            layout.Children.Add(view);

            // Exit with good status.
            return true;
        }
    }
}
