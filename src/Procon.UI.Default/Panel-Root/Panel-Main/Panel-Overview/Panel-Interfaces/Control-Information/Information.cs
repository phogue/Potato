using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Procon.UI.Default.Root.Main.Overview.Interfaces.Information
{
    using Procon.UI.API;
    using Procon.UI.API.Utils;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Information : IExtension
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

        
        // An easy accessor for Properties and Commands of this control.
        private readonly ArrayDictionary<String, Object>   mProps;
        private readonly ArrayDictionary<String, ICommand> mComms;
        public Information()
        {
            mProps = ExtensionApi.GetProperties(GetType());
            mComms = ExtensionApi.GetCommands(GetType());
        }


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainOverviewInterfacesLayout");
            if (tLayout == null) {
                return false;
            }


            // Do what I need to setup my control.
            InformationView tView = new InformationView();
            Grid.SetRowSpan(tView, 2);
            Grid.SetColumn(tView, 1);
            tLayout.Children.Add(tView);


            // Exit with good status.
            return true;
        }
    }
}
