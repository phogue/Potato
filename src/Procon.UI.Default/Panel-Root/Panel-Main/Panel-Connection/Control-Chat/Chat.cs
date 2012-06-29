using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;

namespace Procon.UI.Default.Root.Main.Connection.Chat
{
    [Extension(
        Alters    = new String[] { "MainConnectionLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Connection Layout" })]
    public class Chat : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Connection Chat"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Chat"];
        private InfinityDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Chat"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            ChatView     view = new ChatView();
            GridSplitter splt = new GridSplitter();
            Grid.SetRow(view, 2);
            Grid.SetRow(splt, 2);
            splt.Height = 10;
            splt.Background = Brushes.Transparent;
            splt.ResizeBehavior = GridResizeBehavior.PreviousAndCurrent;
            splt.VerticalAlignment = VerticalAlignment.Top;
            splt.HorizontalAlignment = HorizontalAlignment.Stretch;
            layout.Children.Add(view);
            layout.Children.Add(splt);
            
            // Commands.
            GridLength tHeight    = new GridLength(1.0, GridUnitType.Star);
            Boolean    tMinimized = true;
            tCmmds["MinMax"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when the chat box title is clicked.
                x => {
                    RowDefinition tRowDef = layout.RowDefinitions.Count > 2 ? layout.RowDefinitions[2] : null;
                    if (tRowDef != null) {
                        if (!tMinimized) {
                            tHeight = tRowDef.Height;
                            tRowDef.Height = new GridLength(tRowDef.MinHeight);
                        }
                        else tRowDef.Height = tHeight;
                        tMinimized ^= true;
                    }
                });
            #endregion

            // Exit with good status.
            return true;
        }
    }
}
