using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Connection.Players
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
        { get { return "Main Connection Players"; } }

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
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");

            // Do what I need to setup my control.
            PlayersView tView = new PlayersView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);

            tProps["Score"].Value = 2000;
            tProps["Kdr"].Value   = 2.0;
            tProps["Ping"].Value  = 150;

            tProps["Action"]["Reason"].Value = "Admin Decision.";
            tProps["Action"]["Length"].Value = new TimeSubset() { Context = TimeSubsetContext.Permanent };

            // Commands.
            tCmmds["Move"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the "Move" context menu is clicked.
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
            tCmmds["Kick"].Value = new RelayCommand<Player>(
            #region -- Handles when the "Kick" context menu is clicked.
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
            #region -- Handles when the "Ban" context menu is clicked.
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

            tCmmds["Kick"]["Selection"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the "Kick" button is clicked.
                x => {
                    foreach (Player player in (ObservableCollection<Object>)x.Parameter)
                        ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Reason"].Value
                        });
                });
            #endregion
            tCmmds["Ban"]["Selection"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the "Ban" button is clicked.
                x => {
                    foreach (Player player in (ObservableCollection<Object>)x.Parameter)
                        ExtensionApi.Commands["Player"]["Ban"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Length"].Value,
                            tProps["Action"]["Reason"].Value
                        });
                });
            #endregion

            // Player management methods.
            NotifiableCollection<Player>        tPlayers = null;
            NotifiableCollection<Player>        tManaged = new NotifiableCollection<Player>();
            Action<Player>                      tSort = (p)    => ((Action<Player>)        tProps["List"]["Sort"].Value)(p);
            ItemChangedEventHandler             tItem = (s, e) => ((Action<Player, String>)tProps["List"]["Item"].Value)((Player)e.Item, e.PropertyName);
            NotifyCollectionChangedEventHandler tColl = (s, e) => ((Action<Player, String>)tProps["List"]["Coll"].Value)(e.NewItems != null ? (Player)e.NewItems[0] : (Player)e.OldItems[0], e.Action.ToString());
            tManaged.ItemChanged += tItem;

            tProps["List"]["Sort"].Value     = new Action<Player>(
            #region -- Moves the player to the correct position in the player list.
                x => {
                    Int32 tMod   = 0;
                    Int32 tIndex = 0;

                    while (tIndex < tManaged.Count) {
                        // Still moving the index.
                        if (tManaged[tIndex] == x)
                            tIndex += (tMod = 1);
                        // Move to the correct team.
                        else if (tManaged[tIndex].Team < x.Team)
                            tIndex++;
                        // Move to the correct name placement.
                        else if (tManaged[tIndex].Team == x.Team && tManaged[tIndex].Name.CompareTo(x.Name) < 0)
                            tIndex++;
                        // We're at the correct spot.
                        else
                            break;
                    }
                    tManaged.Move(tManaged.IndexOf(x), tIndex - tMod);
                });
            #endregion
            tProps["List"]["Item"].Value = new Action<Player, String>(
            #region -- Updates the player's managed properties.
                (s, e) => {
                    Double tDouble = Double.MaxValue;

                    // Update Team brushes.
                    if (e == null || e == "Team") {
                        Object tHeaderBrush  = tView.TryFindResource("BrushTeam0Header");
                        Object tContentBrush = tView.TryFindResource("BrushTeam0Content");
                        if (s.Team >= Team.Team1) {
                            tHeaderBrush  = tView.TryFindResource("Brush" + s.Team.ToString() + "Header");
                            tContentBrush = tView.TryFindResource("Brush" + s.Team.ToString() + "Content");
                        }
                        s.DataSet("ui.Header",  tHeaderBrush);
                        s.DataSet("ui.Content", tContentBrush); 
                        if (Dispatcher.CurrentDispatcher != root.Dispatcher) {
                            root.Dispatcher.Invoke(tSort, s);
                            return;
                        }
                        tSort(s);
                    }
                    // Update Country Code image.
                    if (e == null || e == "CountryCode") {
                        if (s.CountryCode != null && ExtensionApi.Properties["Images"]["Countries"].ContainsKey(s.CountryCode))
                            s.DataSet("ui.CountryCode", ExtensionApi.Properties["Images"]["Countries"][s.CountryCode].Value);
                        else 
                            s.DataSet("ui.CountryCode", ExtensionApi.Properties["Images"]["Countries"]["UNK"].Value);
                    }
                    // Update score/kdr/ping brushes.
                    if (e == null || e == "Score") {
                        if (Double.TryParse(tProps["Score"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Score", s.Score >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                    if (e == null || e == "Kdr") {
                        if (Double.TryParse(tProps["Kdr"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Kdr", s.Kdr >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                    if (e == null || e == "Ping") {
                        if (Double.TryParse(tProps["Ping"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Ping", s.Kdr >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                });
            #endregion
            tProps["List"]["Coll"].Value  = new Action<Player, String>(
            #region -- Updates the managed players list.
                (s, e) => {
                    // Add and sort the player.
                    if (e == "Add") {
                        tManaged.Add(s);
                        tItem(null, new ItemChangedEventArgs(s, -1, null));
                    }

                    // Remove the player.
                    else if (e == "Remove")
                        tManaged.Remove(s);
                });
            #endregion
            tProps["List"]["Swap"].Value = new Action<ConnectionViewModel>(
            #region -- Detaches the old list and creates a new list, sorting the players as they're added.
                x => {
                    // Cleanup old stuff.
                    if (tPlayers != null) {
                        tPlayers.CollectionChanged -= tColl;
                        tManaged.Clear();
                        tPlayers = null;
                    }
                    // Setup new stuff.
                    if (x != null) {
                        tPlayers = x.Players;
                        tManaged.AddRange(tPlayers);
                        tPlayers.CollectionChanged += tColl;
                        foreach (Player player in tPlayers)
                            tItem(null, new ItemChangedEventArgs(player, -1, null));
                    }
                });
            #endregion

            tProps.Value = tManaged;
            ExtensionApi.Properties["Connection"].PropertyChanged += (s, e) => ((Action<ConnectionViewModel>)tProps["List"]["Swap"].Value)(ExtensionApi.Connection);

            // Exit with good status.
            return true;
        }
    }
}
