using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Main Connection Players"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

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


            // Setup default property values.
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
                    foreach (Player player in (NotifiableCollection<Object>)x.Parameter)
                        ExtensionApi.Commands["Player"]["Kick"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Reason"].Value
                        });
                });
            #endregion
            tCmmds["Ban"]["Selection"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Handles when the "Ban" button is clicked.
                x => {
                    foreach (Player player in (NotifiableCollection<Object>)x.Parameter)
                        ExtensionApi.Commands["Player"]["Ban"].Value.Execute(new Object[] {
                            player,
                            tProps["Action"]["Length"].Value,
                            tProps["Action"]["Reason"].Value
                        });
                });
            #endregion


            // Information management.
            NotifiableCollection<Player> tPlayers = null;
            NotifiableCollection<Player> tManaged = new NotifiableCollection<Player>();

            Action<Player> tSort = p =>
            #region -- Calls tProps["List"]["Sort"]
            {
                ((Action<Player>)tProps["List"]["Sort"].Value)(p);
            };
            #endregion
            ItemChangedEventHandler tItem = (s, e) =>
            #region -- Calls tProps["List"]["Item"]
            {
                ((Action<Player, String>)tProps["List"]["Item"].Value)((Player)e.Item, e.PropertyName);
            };
            #endregion
            NotifyCollectionChangedEventHandler tColl = (s, e) =>
            #region -- Calls tProps["List"]["Coll"]
            {
                var tCollAction = (Action<IList, NotifyCollectionChangedAction>)tProps["List"]["Coll"].Value;
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        tCollAction(e.NewItems, NotifyCollectionChangedAction.Add);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        tCollAction(e.OldItems, NotifyCollectionChangedAction.Remove);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        tCollAction(null, NotifyCollectionChangedAction.Reset);
                        break;
                }
            };
            #endregion

            tProps["List"]["Sort"].Value = new Action<Player>(
            #region -- Moves the player to the correct position in the player list.
                p => {
                    Int32 tMod   = 0;
                    Int32 tIndex = 0;

                    tManaged.Lock();
                    Int32 tIndexOf = tManaged.IndexOf(p);
                    // Threading error check.
                    if (tIndexOf != -1) {
                        while (tIndex < tManaged.Count) {
                            // Still moving the index.
                            if (tManaged[tIndex] == p)
                                tIndex += (tMod = 1);
                            // Move to the correct team.
                            else if (tManaged[tIndex].Team < p.Team)
                                tIndex++;
                            // Move to the correct name placement.
                            else if (tManaged[tIndex].Team == p.Team && tManaged[tIndex].Name.CompareTo(p.Name) < 0)
                                tIndex++;
                            // We're at the correct spot.
                            else
                                break;
                        }
                        tManaged.Move(tIndexOf, tIndex - tMod);
                    }
                    tManaged.Unlock();
                });
            #endregion
            tProps["List"]["Item"].Value = new Action<Player, String>(
            #region -- Updates the player's managed properties.
                (s, n) => {
                    Double tDouble = Double.MaxValue;

                    // Update Team brushes.
                    if (n == null || n == "Team") {
                        Object tHeaderBrush  = tView.TryFindResource("BrushTeam0Header");
                        Object tContentBrush = tView.TryFindResource("BrushTeam0Content");
                        if (s.Team >= Team.Team1) {
                            tHeaderBrush  = tView.TryFindResource("Brush" + s.Team.ToString() + "Header");
                            tContentBrush = tView.TryFindResource("Brush" + s.Team.ToString() + "Content");
                        }
                        s.DataSet("ui.Header",  tHeaderBrush);
                        s.DataSet("ui.Content", tContentBrush);
                        tSort(s);
                    }
                    // Update Country Code image.
                    if (n == null || n == "CountryCode") {
                        if (s.CountryCode != null && ExtensionApi.Properties["Images"]["Countries"].ContainsKey(s.CountryCode))
                            s.DataSet("ui.CountryCode", ExtensionApi.Properties["Images"]["Countries"][s.CountryCode].Value);
                        else 
                            s.DataSet("ui.CountryCode", ExtensionApi.Properties["Images"]["Countries"]["UNK"].Value);
                    }
                    // Update score/kdr/ping brushes.
                    if (n == null || n == "Score") {
                        if (Double.TryParse(tProps["Score"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Score", s.Score >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                    if (n == null || n == "Kdr") {
                        if (Double.TryParse(tProps["Kdr"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Kdr", s.Kdr >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                    if (n == null || n == "Ping") {
                        if (Double.TryParse(tProps["Ping"].Value.ToString(), out tDouble))
                            s.DataSet("ui.Ping", s.Kdr >= tDouble ? tView.TryFindResource("BrushTeam2Header") : tView.TryFindResource("BrushTextDark"));
                    }
                });
            #endregion
            tProps["List"]["Coll"].Value = new Action<IList, NotifyCollectionChangedAction>(
            #region -- Updates the managed players list.
                (i, a) => {
                    switch (a) {
                        case NotifyCollectionChangedAction.Add:
                            foreach (Player player in i) {
                                tManaged.Add(player);
                                tItem(null, new ItemChangedEventArgs(player, -1, null));
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (Player player in i)
                                tManaged.Remove(player);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            tManaged.Clear();
                            break;
                    }
                });
            #endregion

            tProps["Swap"].Value = new Action<ConnectionViewModel>(
            #region -- Changes the lists being viewed.
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
                        tColl(tPlayers, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<Player>(tPlayers)));
                        tPlayers.CollectionChanged += tColl;
                    }
                });
            #endregion

            tManaged.ItemChanged += tItem;
            tProps["List"].Value = tManaged;
            ExtensionApi.Properties["Connection"].PropertyChanged += (s, e) => {
                new Thread(() => {
                    ((Action<ConnectionViewModel>)tProps["Swap"].Value)(ExtensionApi.Connection);
                }).Start();
            };


            // Exit with good status.
            return true;
        }
    }
}
