using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Overview
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Overview : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Main Connection Overview"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Overview"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Overview"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");

            // Do what I need to setup my control.
            OverviewView tView = new OverviewView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);

            // Exit with good status.
            return true;
        }
    }
}
