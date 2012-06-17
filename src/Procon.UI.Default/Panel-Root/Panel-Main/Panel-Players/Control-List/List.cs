using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Players.List
{
    [Extension(
        Alters    = new String[] { "MainPlayersLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Players Layout" })]
    public class List : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Players List"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = ViewModelBase.PublicProperties["Main"]["Players"]["List"];
        private InfinityDictionary<String, ICommand> tCmmds = ViewModelBase.PublicCommands["Main"]["Players"]["List"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainPlayersLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            ListView view = new ListView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);
            
            // Commands.
            List<PlayerViewModel> tSelectedPlayers = new List<PlayerViewModel>();
            tCmmds["Select"].Value = new RelayCommand<PlayerViewModel>(
                // -- Handles when player is selected/de-selected.
                x => {
                    if (tSelectedPlayers.Contains(x)) tSelectedPlayers.Remove(x);
                    else                              tSelectedPlayers.Add(x);
                });
            tCmmds["Kick"].Value = new RelayCommand<Object>(
                // -- Handles when the "Kick" button is clicked.
                x => {
                    foreach (PlayerViewModel tPlayer in tSelectedPlayers)
                        ExtensionApi.Connection.Action(new Kick() {
                            Target = new Player() {
                                UID    = tPlayer.Uid,
                                SlotID = tPlayer.SlotID,
                                Name   = tPlayer.Name,
                                IP     = tPlayer.IP
                            },
                            Reason = "Admin Decision."
                        });
                });
            tCmmds["Ban"].Value = new RelayCommand<Object>(
                // -- Handles when the "Ban" button is clicked.
                x => {
                    foreach (PlayerViewModel tPlayer in tSelectedPlayers)
                        ExtensionApi.Connection.Action(new Ban() {
                            BanActionType = BanActionType.Ban,
                            Target = new Player() {
                                UID    = tPlayer.Uid,
                                SlotID = tPlayer.SlotID,
                                Name   = tPlayer.Name,
                                IP     = tPlayer.IP
                            },
                            Time = new TimeSubset() {
                                Context = TimeSubsetContext.Permanent
                            },
                            Reason = "Admin Decision."
                        });
                });

            tProps["Score"].Value = 2000.0;
            tProps["Kdr"].Value   = 2.0;
            tProps["Ping"].Value  = 150.0;

            // Exit with good status.
            return true;
        }
    }
}
