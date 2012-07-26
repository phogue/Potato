using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Maps
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Maps : IExtension
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


            // Commands.
            tCmmds["Restart"]["Map"].Value = new RelayCommand<Object>(
            #region -- Restarts the current map.
                x => {
                    ExtensionApi.Commands["Map"]["RestartMap"].Value.Execute(new Object[] { });
                },
                x => {
                    return ExtensionApi.Commands["Map"]["RestartMap"].Value != null &&
                           ExtensionApi.Commands["Map"]["RestartMap"].Value.CanExecute(new Object[] { });
                });
            #endregion
            tCmmds["Restart"]["Round"].Value = new RelayCommand<Object>(
            #region -- Restarts the current round.
                x => {
                    ExtensionApi.Commands["Map"]["RestartRound"].Value.Execute(new Object[] { });
                },
                x => {
                    return ExtensionApi.Commands["Map"]["RestartRound"].Value != null &&
                           ExtensionApi.Commands["Map"]["RestartRound"].Value.CanExecute(new Object[] { });
                });
            #endregion
            tCmmds["Next"]["Round"].Value = new RelayCommand<Object>(
            #region -- Advances to the next round.
                x => {
                    ExtensionApi.Commands["Map"]["NextRound"].Value.Execute(new Object[] { });
                },
                x => {
                    return ExtensionApi.Commands["Map"]["NextRound"].Value != null &&
                           ExtensionApi.Commands["Map"]["NextRound"].Value.CanExecute(new Object[] { });
                });
            #endregion
            tCmmds["Next"]["Map"].Value = new RelayCommand<Object>(
            #region -- Advances to the next map.
                x => {
                    ExtensionApi.Commands["Map"]["NextMap"].Value.Execute(new Object[] { });
                },
                x => {
                    return ExtensionApi.Commands["Map"]["NextMap"].Value != null &&
                           ExtensionApi.Commands["Map"]["NextMap"].Value.CanExecute(new Object[] { });
                });
            #endregion


            // Exit with good status.
            return true;
        }
    }
}
