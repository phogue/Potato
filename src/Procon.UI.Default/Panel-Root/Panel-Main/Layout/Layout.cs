using System;
using System.Windows;
using System.Windows.Controls;

using Procon.UI.API;

namespace Procon.UI.Default.Main.Layout
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
        { get { return "Main Layout"; } }

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

            // Exit with good status.
            return true;
        }
    }
}
