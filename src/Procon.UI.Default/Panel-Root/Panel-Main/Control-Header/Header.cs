using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Header
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
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
        { get { return "Main Header"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Header"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Header"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainLayout");


            // Do what I need to setup my control.
            HeaderView tView = new HeaderView();
            tLayout.Children.Add(tView);


            // Commands.
            tCmmds["State"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the connection status changes.
                x => {
                    Image              tSender = (Image)x.Sender;
                    DependencyProperty tArgs   = (DependencyProperty)x.Args;
                    if (tSender != null && tSender != null) {
                        String tState = tSender.GetValue(tArgs).ToString();
                        if (ExtensionApi.Properties["Images"]["Status"]["Dark"].ContainsKey(tState))
                            tSender.Source = ExtensionApi.Properties["Images"]["Status"]["Dark"][tState].Value as BitmapImage;
                        else
                            tSender.Source = ExtensionApi.Properties["Images"]["Status"]["Dark"]["Unknown"].Value as BitmapImage;
                    }
                });
            #endregion
            tCmmds["Overview"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when an interface or connection is selected.
                x => {
                    ExtensionApi.Settings["View"].Value = "Overview";
                });
            #endregion


            // Exit with good status.
            return true;
        }
    }
}
