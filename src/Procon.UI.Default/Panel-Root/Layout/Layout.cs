using System;
using System.Windows;

using Procon.UI.API;

namespace Procon.UI.Default.Root.Layout
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
        { get { return "Root Layout"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties


        [STAThread]
        public bool Entry(Window root)
        {
            // Do what I need to setup my control.
            LayoutView tView = new LayoutView();
            root.Content = tView;


            // Setup the default settings.
            if (ExtensionApi.Settings["Tutorial"].Value == null)
                ExtensionApi.Settings["Tutorial"].Value = "Step 1";


            // Exit with good status.
            return true;
        }
    }
}
