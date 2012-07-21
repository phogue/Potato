using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Maps
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Players : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Connection Maps"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Maps"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Maps"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");


            // Do what I need to setup my control.
            MapsView tView = new MapsView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);


            // Exit with good status.
            return true;
        }
    }
}
