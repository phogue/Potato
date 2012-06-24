using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            if (layout == null) return false;

            tProps["Score"].Value = 2000.0;
            tProps["Kdr"].Value   = 2.0;
            tProps["Ping"].Value  = 150.0;

            // Do what I need to setup my control.
            ListView view = new ListView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);

            // Commands.
            List<Player> tSelectedPlayers = new List<Player>();
            tCmmds["Select"].Value = new RelayCommand<Player>(
            #region  -- Handles when player is selected/de-selected.
                x => {
                    if (tSelectedPlayers.Contains(x)) tSelectedPlayers.Remove(x);
                    else                              tSelectedPlayers.Add(x);
                });
            #endregion
            tCmmds["Kick"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Kick" button is clicked.
                x => {
                    foreach (Player player in tSelectedPlayers)
                        ExtensionApi.Connection.Action(new Kick() {
                            Target = player,
                            Reason = "Admin Decision."
                        });
                    tSelectedPlayers.Clear();
                });
            #endregion
            tCmmds["Ban"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Ban" button is clicked.
                x => {
                    foreach (Player player in tSelectedPlayers)
                        ExtensionApi.Connection.Action(new Ban() {
                            BanActionType = BanActionType.Ban,
                            Target = player,
                            Time = new TimeSubset() {
                                Context = TimeSubsetContext.Permanent
                            },
                            Reason = "Admin Decision."
                        });
                    tSelectedPlayers.Clear();
                });
            #endregion

            // Manage selection for when players list is refreshed.
            ICollectionView tPlayersCollection = null;
            NotifyCollectionChangedEventHandler tCollectionChanged =
                (s, e) => {
                    if (e.Action == NotifyCollectionChangedAction.Reset)
                        foreach (Player player in tSelectedPlayers)
                            view.MainPlayersListList.SelectedItems.Add(player);
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                        tSelectedPlayers.Remove(e.OldItems[0] as Player);
                };
            ViewModelBase.PublicProperties["Connections"].PropertyChanged +=
                (s, e) => {
                    if (tPlayersCollection != null) tPlayersCollection.CollectionChanged -= tCollectionChanged;
                    tPlayersCollection = CollectionViewSource.GetDefaultView(ExtensionApi.Connection.Players);
                    if (tPlayersCollection != null) tPlayersCollection.CollectionChanged += tCollectionChanged;
                };
            tPlayersCollection = CollectionViewSource.GetDefaultView(ExtensionApi.Connection.Players);
            if (tPlayersCollection != null) tPlayersCollection.CollectionChanged += tCollectionChanged;


            // Exit with good status.
            return true;
        }
    }
}
