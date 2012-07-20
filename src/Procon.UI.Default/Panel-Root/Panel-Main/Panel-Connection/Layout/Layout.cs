using System;
using System.Windows;
using System.Windows.Controls;

using Procon.UI.API;

namespace Procon.UI.Default.Root.Main.Connection.Layout
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
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
        { get { return "Main Connection Layout"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainLayout");

            // Do what I need to setup my control.
            LayoutView tView = new LayoutView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);

            // Setup the default settings.
            if (ExtensionApi.Settings["Connection"].Value == null)
                ExtensionApi.Settings["Connection"].Value = "Overview";

            // Exit with good status.
            return true;
        }
    }
}
