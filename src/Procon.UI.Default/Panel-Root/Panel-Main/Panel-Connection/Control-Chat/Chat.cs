using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Events;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Connection.Chat
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
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
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Chat"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Chat"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");

            // Do what I need to setup my control.
            ChatView     tView = new ChatView();
            GridSplitter tSplt = new GridSplitter();
            Grid.SetRow(tView, 2);
            Grid.SetRow(tSplt, 2);
            tSplt.Height = 10;
            tSplt.Background = Brushes.Transparent;
            tSplt.ResizeBehavior = GridResizeBehavior.PreviousAndCurrent;
            tSplt.VerticalAlignment = VerticalAlignment.Top;
            tSplt.HorizontalAlignment = HorizontalAlignment.Stretch;
            tLayout.Children.Add(tView);
            tLayout.Children.Add(tSplt);
            
            // Commands.
            GridLength tHeight = new GridLength(Double.MaxValue);
            tCmmds["MinMax"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when the chat box title is clicked.
                x => {
                    RowDefinition tRowDef = tLayout.RowDefinitions.Count > 2 ? tLayout.RowDefinitions[2] : null;
                    if (tRowDef != null) {
                        if (tRowDef.Height.Value != tRowDef.MinHeight) {
                            tHeight = tRowDef.Height;
                            tRowDef.Height = new GridLength(tRowDef.MinHeight);
                        }
                        else tRowDef.Height = tHeight;
                    }
                });
            #endregion

            // Player management methods.
            NotifiableCollection<Event>            tEvents  = null;
            NotifiableCollection<Event>            tManaged = new NotifiableCollection<Event>();
            Dictionary<ConnectionViewModel, Int32> tConn = new Dictionary<ConnectionViewModel, Int32>();
            NotifyCollectionChangedEventHandler    tColl = (s, e) => {
                if (!tConn.ContainsKey(ExtensionApi.Connection))
                    tConn.Add(ExtensionApi.Connection, 0);
                Int32 tIndex = tConn[ExtensionApi.Connection];
                tConn[ExtensionApi.Connection] += e.NewItems.Count;
                if (s == tManaged)
                    ((Action<IList>)tProps["List"]["Coll"].Value)(new List<Event>(e.NewItems.OfType<Event>().Skip(tIndex).OfType<ChatEvent>()));
                else {
                    tManaged.Add((Event)e.NewItems[0]);
                    ((Action<IList>)tProps["List"]["Coll"].Value)(new List<Event>(e.NewItems.OfType<ChatEvent>()));
                }
            };

            tProps["List"]["Coll"].Value = new Action<IList>(
            #region -- Updates the managed players list.
                e => {
                    // For each event added.
                    foreach (ChatEvent @event in e) {
                        Visibility tTeam        = Visibility.Collapsed;
                        Visibility tSquad       = Visibility.Collapsed;
                        Visibility tPlayer      = Visibility.Visible;
                        Object     tTeamBrush   = tView.TryFindResource("BrushChatMessage");
                        Object     tSquadBrush  = tView.TryFindResource("BrushChatMessage");
                        Object     tPlayerBrush = tView.TryFindResource("BrushChatPrivate");
                        Object     tNameBrush   = tView.TryFindResource("BrushChatPrivate");

                        // Chat is not to a player.
                        if (@event.Player == null) {
                            tTeam      = Visibility.Visible;
                            tSquad     = Visibility.Visible;
                            tPlayer    = Visibility.Collapsed;
                            tNameBrush = tView.TryFindResource("BrushChatMessage");
                        }
                        // Chat is not to a squad.
                        if (@event.Squad == Squad.None)
                            tSquad = Visibility.Collapsed;
                        // Chat is not to a team.
                        if (@event.Team == Team.None)
                            tTeam = Visibility.Collapsed;
                        // Chat is to a team.
                        else
                            tTeamBrush = tSquadBrush = tNameBrush = tView.TryFindResource("Brush" + @event.Team.ToString() + "Content");

                        // Setup values.
                        @event.DataSet("ui.TeamVs",   tTeam);
                        @event.DataSet("ui.TeamFg",   tTeamBrush);
                        @event.DataSet("ui.SquadVs",  tSquad);
                        @event.DataSet("ui.SquadFg",  tSquadBrush);
                        @event.DataSet("ui.PlayerVs", tPlayer);
                        @event.DataSet("ui.PlayerFg", tPlayerBrush);
                        @event.DataSet("ui.NameFg",   tNameBrush);
                    }
                });
            #endregion
            tProps["List"]["Swap"].Value = new Action<ConnectionViewModel>(
            #region -- Detaches the old list and creates a new list.
                x => {
                    // Cleanup old stuff.
                    if (tEvents != null) {
                        tEvents.CollectionChanged -= tColl;
                        tManaged.Clear();
                        tEvents = null;
                    }
                    // Setup new stuff.
                    if (x != null) {
                        tEvents = x.Events;
                        tManaged.AddRange(tEvents);
                        tEvents.CollectionChanged += tColl;
                        tColl(tManaged, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<Event>(tEvents)));
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
