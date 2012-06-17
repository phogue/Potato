using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Views.Connections
{
    [Extension(
        Alters    = new String[] { "MainViewsLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Views Layout" })]
    public class Connections : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Views Connections"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Views"]["Connections"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Views"]["Connections"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainViewsLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            ConnectionsView view = new ConnectionsView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);

            // Exit with good status.
            return true;
        }
    }
}
