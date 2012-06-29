using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

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
        private InfinityDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Players"];
        private InfinityDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Players"];


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

            tProps["Reason"].Value = "Admin Decision.";
            tProps["Length"].Value = new TimeSubset() { Context = TimeSubsetContext.Permanent };

            // Commands.
            List<Player> tSelectedPlayers = new List<Player>();
            tCmmds["Select"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when player is selected/de-selected.
                x => {
                    if ((x.Args as MouseButtonEventArgs).ChangedButton == MouseButton.Right)
                        tSelectedPlayers.Clear();
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
                        tProps["Reason"].Value
                    });
                },
                x => {
                    return x.Parameter != null &&
                           ExtensionApi.Commands["Player"]["Move"].Value != null &&
                           ExtensionApi.Commands["Player"]["Move"].Value.CanExecute(new Object[] {
                               (x.Parameter),
                               (x.Sender as FrameworkElement).DataContext,
                               tProps["Reason"].Value
                           });
                });
            #endregion
            tCmmds["Kick"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Kick" context menu is clicked.
                x => {
                    ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                        x,
                        tProps["Reason"].Value
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Player"]["Kick"].Value != null &&
                           ExtensionApi.Commands["Player"]["Kick"].Value.CanExecute(new Object[] {
                               x,
                               tProps["Reason"].Value
                           });
                });
            #endregion
            tCmmds["Ban"].Value = new RelayCommand<Player>(
            #region  -- Handles when the "Ban" context menu is clicked.
                x => {
                    ExtensionApi.Commands["Player"]["Ban"].Value.Execute(new Object[] {
                        x,
                        tProps["Length"].Value,
                        tProps["Reason"].Value
                    });
                },
                x => {
                    return x != null &&
                           ExtensionApi.Commands["Player"]["Ban"].Value != null &&
                           ExtensionApi.Commands["Player"]["Ban"].Value.CanExecute(new Object[] {
                               x,
                               tProps["Length"].Value,
                               tProps["Reason"].Value
                           });
                });
            #endregion
            tCmmds["Kick"]["Selection"].Value = new RelayCommand<Object>(
            #region  -- Handles when the "Kick" button is clicked.
                x => {
                    foreach (Player player in tSelectedPlayers)
                        ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                            player,
                            tProps["Reason"].Value
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
                            tProps["Length"].Value,
                            tProps["Reason"].Value
                        });
                    tSelectedPlayers.Clear();
                });
            #endregion

            // Manage the player's collection.
            ObservableCollection<Player>        tBoundPlayers   = null;
            Action<Player>                      tSortPlayer     = null;
            NotifyCollectionChangedEventHandler tPlayersUpdated = null;
            PropertyChangedEventHandler         tPlayerUpdated  = null;
            PropertyChangedEventHandler         tResetPlayers   = null;
            tSortPlayer =
            #region -- Sorts the player to the correct spot in the player list.
                x => {
                    Int32                        tIndex  = 0;
                    ObservableCollection<Player> tSorted = tProps.Value as ObservableCollection<Player>;
                    while (tIndex < tSorted.Count)
                        if (tSorted[tIndex] != x)
                            // Move to the correct team.
                            if (tSorted[tIndex].Team < x.Team)
                                tIndex++;
                            // Move to the correct name placement.
                            else if (tSorted[tIndex].Team == x.Team && tSorted[tIndex].Name.CompareTo(x.Name) < 0)
                                tIndex++;
                            // We're at the correct spot.
                            else {
                                tSorted.Move(tSorted.IndexOf(x), tIndex - (tSorted.IndexOf(x) < tIndex ? 1 : 0));
                                break;
                            }
                        else tIndex++;
                        
                };
            #endregion
            tPlayersUpdated =
            #region -- Manages the player list whenever a player is added or removed.
                (s, e) => {
                    Player                       tPlayer = null;
                    ObservableCollection<Player> tSorted = tProps.Value as ObservableCollection<Player>;
                    // Add and sort the player.
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        tPlayer = e.NewItems[0] as Player;
                        tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Score"));
                        tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Kdr"));
                        tPlayerUpdated(tPlayer, new PropertyChangedEventArgs("Ping"));
                        tSorted.Add(tPlayer);
                        tSortPlayer(tPlayer);
                        tPlayer.PropertyChanged += tPlayerUpdated;
                    }
                    // Remove the player.
                    else if (e.Action == NotifyCollectionChangedAction.Remove) {
                        tPlayer = e.OldItems[0] as Player;
                        tPlayer.PropertyChanged -= tPlayerUpdated;
                        tSorted.Remove(tPlayer);
                    }
                };
            #endregion
            tPlayerUpdated =
            #region -- Sorts the player whenever their team changes.
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
                            tSortPlayer(player);
                            player.PropertyChanged += tPlayerUpdated;
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
