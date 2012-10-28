using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
            tCmmds["Send"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when the user presses Enter in the chat box.
                x => {
                    ExtensionApi.Commands["Chat"]["Send"].Value.Execute(new Object[] {
                        tProps["Text"].Value,
                        ChatActionType.Say,
                        new PlayerSubset() {
                            Context = PlayerSubsetContext.All
                        }
                    });
                    tProps["Text"].Value = String.Empty;
                },
                x => {
                    var tEventArgs = (KeyEventArgs)x.Args;
                    return tEventArgs     != null &&
                           tEventArgs.Key == Key.Return &&
                           ExtensionApi.Commands["Chat"]["Send"].Value != null &&
                           ExtensionApi.Commands["Chat"]["Send"].Value.CanExecute(new Object[] {
                               tProps["Text"].Value,
                               ChatActionType.Say,
                               new PlayerSubset() {
                                   Context = PlayerSubsetContext.All
                               }
                           });
                });
            #endregion
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


            // Information management.
            NotifiableCollection<Event> tEvents  = null;
            NotifiableCollection<Event> tManaged = new NotifiableCollection<Event>();

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

            tProps["List"]["Coll"].Value = new Action<IList, NotifyCollectionChangedAction>(
            #region -- Updates the managed events list.
                (i, a) => {
                    switch (a) {
                        case NotifyCollectionChangedAction.Add:
                            foreach (Event @event in i)
                                tManaged.Add(@event);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (Event @event in i)
                                tManaged.Remove(@event);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            tManaged.Clear();
                            break;
                    }

                    // Setup properties for new items being added.
                    if (a == NotifyCollectionChangedAction.Add)
                        foreach (ChatEvent @event in i.OfType<ChatEvent>().Where(e => !e.DataContains("ui.TeamVs"))) {
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

            tProps["Swap"].Value = new Action<ConnectionViewModel>(
            #region -- Changes the lists being viewed.
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
                        tColl(tEvents, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<Event>(tEvents)));
                        tEvents.CollectionChanged += tColl;
                    }
                });
            #endregion

            tProps.Value = tManaged;
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
