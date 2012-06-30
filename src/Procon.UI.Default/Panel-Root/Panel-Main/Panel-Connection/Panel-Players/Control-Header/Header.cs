using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Players.Header
{
    [Extension(
        Alters    = new String[] { "MainConnectionPlayersLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Connection Players Layout" })]
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
        { get { return "Main Connection Players Header"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Players"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Players"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConnectionPlayersLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            HeaderView view = new HeaderView();
            layout.Children.Add(view);
            
            tProps["Score"].Value = 2000;
            tProps["Kdr"].Value   = 2.0;
            tProps["Ping"].Value  = 150;

            // Exit with good status.
            return true;
        }
    }
}
