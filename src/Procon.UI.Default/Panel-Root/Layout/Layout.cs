using System;
using System.Windows;

namespace Procon.UI.Default.Root.Layout
{
    using Procon.UI.API;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Layout : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "TeamPlayer Gaming"; } }

        public String Name
        { get { return GetType().Namespace; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties


        [STAThread]
        public bool Entry(Window root)
        {
            // Do what I need to setup my control.
            LayoutView tView = new LayoutView();
            root.Content = tView;


            // Exit with good status.
            return true;
        }
    }
}
