using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Players.Content
{
    [Extension(
        Alters    = new String[] { "MainPlayersLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Players Layout" })]
    public class Content : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Connection Players Content"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Players"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Players"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConnectionPlayersLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            ContentView view = new ContentView();
            Grid.SetRow(view, 1);
            layout.Children.Add(view);

            tProps["Action"]["Reason"].Value = "Admin Decision.";
            tProps["Action"]["Length"].Value = new TimeSubset() { Context = TimeSubsetContext.Permanent };

            // Commands.
            List<Player> tSelectedPlayers = new List<Player>();
            tCmmds["Select"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when player is selected/de-selected.
                x => {
                    // For some reason, the selection is cleared if the right mouse button is used.
                    if ((x.Args as MouseButtonEventArgs).ChangedButton == MouseButton.Right)
                        tSelectedPlayers.Clear();

                    // Add or remove the player, depending on whether the player was in the list or not.
                    Player tPlayer = x.Parameter as Player;
                    if (tSelectedPlayers.Contains(tPlayer))
                        tSelectedPlayers.Remove(tPlayer);
                    else
                        tSelectedPlayers.Add(tPlayer);
                });
            #endregion
            
            tCmmds["Move"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when the "Move" context menu is clicked.
                x => {
                    ExtensionApi.Commands["Player"]["Move"].Value.Execute(new Object[] {
                        (x.Parameter),
                        (x.Sender as FrameworkElement).DataContext,
                        tProps["Action"]["Reason"].Value
                    });
                },
                x => {
                    return x.Parameter != null &&
                           ExtensionApi.Commands["Player"]["Move"].Value != null &&
                           ExtensionApi.Commands["Player"]["Move"].Value.CanExecute(new Object[] {
                               (x.Parameter),
                               (x.Sender as FrameworkElement).DataContext,
                               tProps["Action"]["Reason"].Value
                           });
                });
            #endregion
            tCmmds["Kick"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Kick" context menu is clicked.
                x => {
                    ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                        x,
                        tProps["Action"]["Reason"].Value
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Player"]["Kick"].Value != null &&
                           ExtensionApi.Commands["Player"]["Kick"].Value.CanExecute(new Object[] {
                               x,
                               tProps["Action"]["Reason"].Value
                           });
                });
            #endregion
            tCmmds["Ban"].Value = new RelayCommand<Player>(
            #region  -- Handles when the "Ban" context menu is clicked.
                x => {
                    ExtensionApi.Commands["Player"]["Ban"].Value.Execute(new Object[] {
                        x,
                        tProps["Action"]["Length"].Value,
                        tProps["Action"]["Reason"].Value
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Player"]["Ban"].Value != null &&
                           ExtensionApi.Commands["Player"]["Ban"].Value.CanExecute(new Object[] {
                               x,
                               tProps["Action"]["Length"].Value,
                               tProps["Action"]["Reason"].Value
                           });
                });
            #endregion

            tCmmds["Kick"]["Selection"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Kick" button is clicked.
                x => {
                    foreach (Player player in tSelectedPlayers)
                        ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Reason"].Value
                        });
                    tSelectedPlayers.Clear();
                });
            #endregion
            tCmmds["Ban"]["Selection"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Ban" button is clicked.
                x => {
                    foreach (Player player in tSelectedPlayers)
                        ExtensionApi.Commands["Player"]["Ban"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Length"].Value,
                            tProps["Action"]["Reason"].Value
                        });
                    tSelectedPlayers.Clear();
                });
            #endregion

            // Used to manage the player's collection.
            ObservableCollection<Player>        tBoundPlayers   = null;
            Action<Player>                      tSortPlayer     = null;
            NotifyCollectionChangedEventHandler tPlayersUpdated = null;
            PropertyChangedEventHandler         tPlayerUpdated  = null;
            PropertyChangedEventHandler         tResetPlayers   = null;

            // Manage the player's collection.
            tSortPlayer =
            #region -- Sorts the player to the correct spot in the player list.
                x => {
                    Int32                        tMod    = 0;
                    Int32                        tIndex  = 0;
                    ObservableCollection<Player> tSorted = tProps.Value as ObservableCollection<Player>;
                    while (tIndex < tSorted.Count) {
                        // Still moving the index.
                        if (tSorted[tIndex] == x)
                            tIndex += (tMod = 1);
                        // Move to the correct team.
                        else if (tSorted[tIndex].Team < x.Team)
                            tIndex++;
                        // Move to the correct name placement.
                        else if (tSorted[tIndex].Team == x.Team && tSorted[tIndex].Name.CompareTo(x.Name) < 0)
                            tIndex++;
                        // We're at the correct spot.
                        else
                            break;
                    }
                    tSorted.Move(tSorted.IndexOf(x), tIndex - tMod);

                        
                };
            #endregion
            tPlayersUpdated =
            #region -- Handles updates due to a player being added or removed.
                (s, e) => {
                    Player                       tPlayer = null;
                    ObservableCollection<Player> tSorted = tProps.Value as ObservableCollection<Player>;
                    // Add and sort the player.
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        tPlayer = e.NewItems[0] as Player;
                        tSorted.Add(tPlayer);
                        tPlayer.PropertyChanged += tPlayerUpdated;
                        tSortPlayer(tPlayer);
                    }
                    // Remove the player.
                    else if (e.Action == NotifyCollectionChangedAction.Remove) {
                        tPlayer = e.OldItems[0] as Player;
                        tPlayer.PropertyChanged -= tPlayerUpdated;
                        tSorted.Remove(tPlayer);
                    }
                    tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Score"));
                    tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Kdr"));
                    tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Ping"));
                };
            #endregion
            tPlayerUpdated =
            #region -- Handles updates due to a player's property changing.
                (s, e) => {
                    Player tPlayer = s as Player;
                    Double tDouble = Double.MaxValue;
                    switch (e.PropertyName) {
                        case "Team":
                            root.Dispatcher.Invoke(tSortPlayer, s);
                            break;

                        case "Score":
                            if (Double.TryParse(tProps["Score"].Value.ToString(), out tDouble))
                                tPlayer.DataSet("ui.Score", tPlayer.Score >= tDouble ? Visibility.Visible : Visibility.Hidden);
                            break;
                        case "Kdr":
                            if (Double.TryParse(tProps["Kdr"].Value.ToString(), out tDouble))
                                tPlayer.DataSet("ui.Kdr", tPlayer.Kdr >= tDouble ? Visibility.Visible : Visibility.Hidden);
                            break;
                        case "Ping":
                            if (Double.TryParse(tProps["Ping"].Value.ToString(), out tDouble))
                                tPlayer.DataSet("ui.Ping", tPlayer.Kdr >= tDouble ? Visibility.Visible : Visibility.Hidden);
                            break;
                    }
                };
            #endregion
            tResetPlayers = 
            #region -- Detaches the old list, re-creates the list correctly sorted, retaches the new list.
                (s, e) => {
                    // Cleanup old stuff.
                    ObservableCollection<Player> tSorted = tProps.Value as ObservableCollection<Player>;
                    if (tBoundPlayers != null) {
                        tBoundPlayers.CollectionChanged -= tPlayersUpdated;
                        foreach (Player player in tSorted)
                            player.PropertyChanged -= tPlayerUpdated;
                        tBoundPlayers = null;
                    }
                    // Group & Sort the new list of players.
                    if (ExtensionApi.Connection != null) {
                        tBoundPlayers = ExtensionApi.Connection.Players;
                        tProps.Value  = tSorted = new ObservableCollection<Player>();
                        foreach (Player player in tBoundPlayers) {
                            tPlayerUpdated(player, new PropertyChangedEventArgs("Score"));
                            tPlayerUpdated(player, new PropertyChangedEventArgs("Kdr"));
                            tPlayerUpdated(player, new PropertyChangedEventArgs("Ping"));
                            tSorted.Add(player);
                            player.PropertyChanged += tPlayerUpdated;
                            tSortPlayer(player);
                        }
                        tBoundPlayers.CollectionChanged += tPlayersUpdated;
                    }
                };
            #endregion

            // Let the managing begin.
            ExtensionApi.Properties["Connection"].PropertyChanged += tResetPlayers;

            // Exit with good status.
            return true;
        }
    }
}
