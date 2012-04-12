// Copyright 2011 Cameron 'Imisnew2' Gunnin
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Procon.Core.Interfaces.Connections;
using Procon.Net;
using Procon.Net.Protocols.Objects;
using Procon.UI.Old.Classes;
using Procon.UI.Old.Enumerations;
using Procon.Net.Protocols;

namespace Procon.UI.Old.Windows
{
    public partial class GameWindow : Window
    {
        /// <summary>
        /// Controls whether the window should close or just be hidden.
        /// </summary>
        private Boolean mShouldClose;

        /// <summary>
        /// The current teams that this game (mode) supports.
        /// </summary>
        public ObservableCollection<Team> Teams { get; set; }
        /// <summary>
        /// The current squads that this game (mode) supports.
        /// </summary>
        public ObservableCollection<Squad> Squads { get; set; }

        /// <summary>
        /// The context menu that should be used when the user right
        /// clicks on the columns of the player list.
        /// </summary>
        public ContextMenu PlayerListColumnMenu { get; set; }
        /// <summary>
        /// The context menu that should be used when the user right
        /// clicks on the rows of the player list.
        /// </summary>
        public ContextMenu PlayerListRowMenu { get; set; }

        /// <summary>
        /// Contains the chat messages that have been collected since
        /// the window was first created.
        /// </summary>
        public ObservableCollection<Chat> ChatMessages { get; set; }
        /// <summary>
        /// The portion of the chat box to check when filtering.
        /// </summary>
        public  ChatFilterType ChatFilterType
        {
            get { return mChatFilterType; }
            set
            {
                mChatFilterType = value;
                CollectionViewSource.GetDefaultView(listbox_ChatBox.ItemsSource).Refresh();
            }
        }
        private ChatFilterType mChatFilterType = ChatFilterType.Message;
        /// <summary>
        /// How to treat the filter text when filtering the chat box.
        /// </summary>
        public  ChatFilterMethod ChatFilterMethod
        {
            get { return mChatFilterMethod; }
            set
            {
                mChatFilterMethod = value;
                CollectionViewSource.GetDefaultView(listbox_ChatBox.ItemsSource).Refresh();
            }
        }
        private ChatFilterMethod mChatFilterMethod = ChatFilterMethod.Contains;
        /// <summary>
        /// The text to filter the chat box with.
        /// </summary>
        public  String ChatFilterText
        {
            get { return mChatFilterText; }
            set
            {
                mChatFilterText = value.Trim();
                CollectionViewSource.GetDefaultView(listbox_ChatBox.ItemsSource).Refresh();
            }
        }
        private String mChatFilterText = String.Empty;

        /// <summary>
        /// The type of maps to look at when viewing the map pool.
        /// </summary>
        public  GameMode GameModeFilter
        {
            get { return mGameModeFilter; }
            set
            {
                mGameModeFilter = value;
                CollectionViewSource.GetDefaultView(listbox_MapPool.ItemsSource).Refresh();
                GameMode_Selected(GameModeFilter);
            }
        }
        private GameMode mGameModeFilter = null;

        /// <summary>
        /// The portion of the ban list to check when filtering.
        /// </summary>
        public BanFilterType BanFilterType
        {
            get { return mBanFilterType; }
            set
            {
                mBanFilterType = value;
                CollectionViewSource.GetDefaultView(datagrid_BanList.ItemsSource).Refresh();
            }
        }
        private BanFilterType mBanFilterType = BanFilterType.Uid;
        /// <summary>
        /// The text to filter the ban list with.
        /// </summary>
        public  String BanFilterText
        {
            get { return mBanFilterText; }
            set
            {
                mBanFilterText = value.Trim();
                CollectionViewSource.GetDefaultView(datagrid_BanList.ItemsSource).Refresh();
            }
        }
        private String mBanFilterText = String.Empty;

        public String ServerName
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.ServerName, "Unnamed Server"); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.ServerName, value); }
        }

        public String ServerDescription
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.Description, "None"); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.Description, value); }
        }

        public String ServerBannerUrl
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.BannerUrl, "None"); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.BannerUrl, value); }
        }

        public Boolean ServerPunkbuster
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.AntiCheat, false); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.AntiCheat, value); }
        }

        public Boolean ServerRanked
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.Ranked, false); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.Ranked, value); }
        }

        public Boolean ServerProfanityFilter
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.ProfanityFilter, false); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.ProfanityFilter, value); }
        }

        public Boolean ServerIdleKick
        {
            get { return true; }//return (DataContext as Connection).GameState.Get(ServerVariableKey.IdleTimeout, false); }
            set { }//(DataContext as Connection).GameState.Set(ServerVariableKey.IdleTimeout, value); }
        }

        public Int32 ServerIdleKickTimeout
        {
            get { return (DataContext as Connection).GameState.Get(ServerVariableKey.IdleTimeout, 300); }
            set { (DataContext as Connection).GameState.Set(ServerVariableKey.IdleTimeout, value); }
        }


        /// <summary>
        /// Create a UI used to display a single game connection's information.
        /// Also allows the user to easily manage the game server the conneciton
        /// represents, allowing them to chat, kick/ban, manage the ban list,
        /// manage the map list, along with other server functions.
        /// </summary>
        /// <param name="connection">The connection that we want to manage.</param>
        public GameWindow(Connection connection)
        {
            // Initialize Code-behind values.
            Title = String.Format("{0} <{1}:{2}>",
                connection.GameState.Get<String>(ServerVariableKey.ServerName),
                connection.Hostname,
                connection.Port);
            DataContext  = connection;
            mShouldClose = false;
            Teams        = new ObservableCollection<Team>();
            Squads       = new ObservableCollection<Squad>();
            ChatMessages = new ObservableCollection<Chat>();

            InitializeComponent();

            #region Code Behind Initialization

            // Setting up default Selections
            if (combobox_GameMode.Items.Count > 0)
                combobox_GameMode.SelectedIndex = 0;
            if (connection.GameType == GameType.BF_BC2)
                button_GameMode.Visibility = Visibility.Visible;
            textbox_pBanDur.Text = "60";

            #region Collection View Source Sort/Group/Filter

            (Resources["cvs_Players"] as CollectionViewSource).SortDescriptions.Add(new SortDescription("Team",  ListSortDirection.Ascending));
            (Resources["cvs_Players"] as CollectionViewSource).SortDescriptions.Add(new SortDescription("Squad", ListSortDirection.Ascending));
            (Resources["cvs_Players"] as CollectionViewSource).GroupDescriptions.Add(new PropertyGroupDescription("Team"));
            (Resources["cvs_MapList"] as CollectionViewSource).SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            (Resources["cvs_MapPool"] as CollectionViewSource).Filter += MapPool_Filter;
            (Resources["cvs_BanList"] as CollectionViewSource).Filter += BanList_Filter;
            (Resources["cvs_ChatMessages"] as CollectionViewSource).Filter += ChatBox_Filter;

            #endregion
            #region Player List Columns and Context Menus

            DataGridTextColumn dgtc;
            MenuItem mi;
            Binding b;

            #region Column-Based

            PlayerListColumnMenu = new ContextMenu();
            // Represents properties we want to display from the Player class.
            List<Tuple<String, String, Int32, Visibility>> playerListIds = new List<Tuple<String, String, Int32, Visibility>>()
            {
                new Tuple<String, String, Int32, Visibility>("Uid",          "Uid",          75, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("GUID",         "GUID",         75, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("SlotID",       "SlotID",       35, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("ClanTag",      "ClanTag",      50, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Name",         "Name",         50, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("NameStripped", "NameStripped", 50, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("Role",         "Role.Name",    70, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Team",         "Team",         40, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Squad",        "Squad",        40, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Score",        "Score",        35, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Kills",        "Kills",        35, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("Deaths",       "Deaths",       35, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("Kdr",          "Kdr",          35, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Ping",         "Ping",         35, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("Country",      "Country",      50, Visibility.Collapsed),
                new Tuple<String, String, Int32, Visibility>("CountryCode",  "CountryCode",  35, Visibility.Visible),
                new Tuple<String, String, Int32, Visibility>("IP",           "IP",           70, Visibility.Collapsed)
            };

            // Populate the Player List Data Grid's Columns and Column Context Menu.
            foreach (Tuple<String, String, Int32, Visibility> playerListId in playerListIds)
            {
                dgtc            = new DataGridTextColumn();
                dgtc.Header     = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", playerListId.Item1);
                dgtc.Binding    = new Binding(playerListId.Item2);
                if (playerListId.Item1 == "Kdr")
                    dgtc.Binding = new Binding(playerListId.Item2) { StringFormat = "{0:0.##}" };
                if (playerListId.Item1 == "Team" || playerListId.Item1 == "Squad")
                    dgtc.Binding = new Binding(playerListId.Item2) { Converter = (Procon.UI.Old.Classes.EnumConverter)App.Current.Resources["enumConv"] };
                dgtc.MinWidth   = playerListId.Item3;
                dgtc.Visibility = playerListId.Item4;

                b                     = new Binding("Visibility");
                b.Source              = dgtc;
                b.Converter           = (IsVisibleConverter)App.Current.Resources["isVisible"];
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                mi             = new MenuItem();
                mi.Header      = dgtc.Header;
                mi.IsCheckable = true;
                mi.SetBinding(MenuItem.IsCheckedProperty, b);

                datagrid_PlayerList.Columns.Add(dgtc);
                PlayerListColumnMenu.Items.Add(mi);
            }

            #endregion
            #region Row-Based

            PlayerListRowMenu = new ContextMenu();
            // Represents the actions we want to allow the user to perform.
            List<Tuple<String, String>> playerListActions = new List<Tuple<String, String>>()
            {
                new Tuple<String, String>("View",      "ViewPlayer"),
                new Tuple<String, String>("Search",    "SearchPlayer"),
                new Tuple<String, String>("Move",      "MovePlayer"),
                new Tuple<String, String>("Separator", null),
                new Tuple<String, String>("Yell",      "YellPlayer"),
                new Tuple<String, String>("Kill",      "KillPlayer"),
                new Tuple<String, String>("Kick",      "KickPlayer"),
                new Tuple<String, String>("Ban",       "BanPlayer")
            };

            // Populate the Player List Data Grid's Row Context Menu.
            foreach (Tuple<String, String> playerListAction in playerListActions)
            {
                if (playerListAction.Item1 == "Separator")
                    PlayerListRowMenu.Items.Add(new Separator());
                else
                {
                    mi = new MenuItem();
                    mi.Header = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", playerListAction.Item1);
                    mi.Icon = App.Current.Resources[playerListAction.Item2];
                    if (playerListAction.Item1 == "Move")
                    {
                        #region Teams Sub-Menu

                        MenuItem teams     = new MenuItem();
                        teams.Name         = playerListAction.Item1;
                        teams.Header       = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "Teams");
                        teams.ItemTemplate = (DataTemplate)App.Current.Resources["EnumTemplate"];
                        teams.PreviewMouseLeftButtonDown += teams_PreviewMouseLeftButtonDown;
                        b                     = new Binding("Teams");
                        b.Source              = this;
                        b.Converter           = (Procon.UI.Old.Classes.EnumConverter)App.Current.Resources["enumConv"];
                        b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        teams.SetBinding(MenuItem.ItemsSourceProperty, b);
                        mi.Items.Add(teams);

                        #endregion
                        #region Squads Sub-Menu

                        MenuItem squads     = new MenuItem();
                        squads.Name         = playerListAction.Item1;
                        squads.Header       = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "Squads");
                        squads.ItemTemplate = (DataTemplate)App.Current.Resources["EnumTemplate"];
                        squads.PreviewMouseLeftButtonDown += teams_PreviewMouseLeftButtonDown;
                        b                      = new Binding("Squads");
                        b.Source               = this;
                        b.Converter            = (Procon.UI.Old.Classes.EnumConverter)App.Current.Resources["enumConv"];
                        b.UpdateSourceTrigger  = UpdateSourceTrigger.PropertyChanged;
                        squads.SetBinding(MenuItem.ItemsSourceProperty, b);
                        mi.Items.Add(squads);

                        #endregion
                    }
                    else
                        mi.Click += PlayerList_ContextMenuClicked;
                    PlayerListRowMenu.Items.Add(mi);
                }
            }

            #endregion

            #endregion

            // Updating our managed Team/Squad lists.
            updateTeams();
            combobox_pMoveToTeam.SelectedItem = Team.Team1;
            updateSquads();
            combobox_pMoveToSquad.SelectedItem = Squad.None;

            #endregion
            #region Combo Box Selections

            // Ban Types
            List<Enum> mEnumerations = new List<Enum>();
            foreach (TimeSubsetContext timeSubsetContext in Enum.GetValues(typeof(TimeSubsetContext)))
                if (timeSubsetContext != TimeSubsetContext.None && timeSubsetContext != TimeSubsetContext.Round)
                    mEnumerations.Add(timeSubsetContext);
            combobox_pBanDurType.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_pBanDurType.SelectedItem = TimeSubsetContext.Permanent;

            // Ban Filter Type
            mEnumerations.Clear();
            foreach (BanFilterType banFilterType in Enum.GetValues(typeof(BanFilterType)))
                mEnumerations.Add(banFilterType);
            combobox_BanListFilter.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_BanListFilter.SelectedItem = BanFilterType.Uid;

            // Chat Filter Method
            mEnumerations.Clear();
            foreach (ChatFilterMethod chatFilterMethod in Enum.GetValues(typeof(ChatFilterMethod)))
                mEnumerations.Add(chatFilterMethod);
            combobox_ChatBoxFilterMethod.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_ChatBoxFilterMethod.SelectedItem = ChatFilterMethod.Contains;

            // Chat Filter Type
            mEnumerations.Clear();
            foreach (ChatFilterType chatFilterType in Enum.GetValues(typeof(ChatFilterType)))
                mEnumerations.Add(chatFilterType);
            combobox_ChatBoxFilterType.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_ChatBoxFilterType.SelectedItem = ChatFilterType.Message;

            // Chat Message Type
            mEnumerations.Clear();
            foreach (ChatActionType chatActionType in Enum.GetValues(typeof(ChatActionType)))
                mEnumerations.Add(chatActionType);
            combobox_ChatBoxSendType.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_ChatBoxSendType.SelectedItem = ChatActionType.Say;

            // Chat Message Subset
            mEnumerations.Clear();
            foreach (PlayerSubsetContext playerSubsetContext in Enum.GetValues(typeof(PlayerSubsetContext)))
                if (playerSubsetContext != PlayerSubsetContext.Server)
                    mEnumerations.Add(playerSubsetContext);
            combobox_ChatBoxSendTo.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_ChatBoxSendTo.SelectedItem = PlayerSubsetContext.All;

            #endregion

            #region Localization

            #region Tab Items

            tabitem_PlayerList.Header = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "Title");
            tabitem_MapList.Header    = Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList",    "Title");
            tabitem_BanList.Header    = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList",    "Title");
            tabitem_ServerInfo.Header = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "Title");

            #endregion
            #region Player List
            
            #region Move Popup

            label_pMoveTitle.Content    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveTitle");
            textblock_pMoveMessage.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveMessage");
            label_pMoveToTeam.Content   = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveTeam");
            label_pMoveToSquad.Content  = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveSquad");
            button_pMoveOk.Content      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveOK");
            button_pMoveCancel.Content  = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Move", "MoveCancel");

            #endregion
            #region Warn Popup

            label_pWarnTitle.Content      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnTitle");
            textblock_pWarnMessage.Text   = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnMessage");
            button_pWarnOk.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnOK");
            button_pWarnCancel.Content    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnCancel");
            textbox_pWarnReason.Text      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnDefaultReason");
            textbox_pWarnReasonPopup.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Warn", "WarnErrorReason");

            #endregion
            #region Kick Popup

            label_pKickTitle.Content      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickTitle");
            textblock_pKickMessage.Text   = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickMessage");
            button_pKickOk.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickOK");
            button_pKickCancel.Content    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickCancel");
            textbox_pKickReason.Text      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickDefaultReason");
            textbox_pKickReasonPopup.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Kick", "KickErrorReason");

            #endregion
            #region Ban Popup

            label_pBanTitle.Content      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanTitle");
            textblock_pBanMessage.Text   = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanMessage");
            label_pBanDurType.Content    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanDurType");
            label_pBanDur.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanDur");
            label_pBanReason.Content     = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanReason");
            button_pBanOk.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanOK");
            button_pBanCancel.Content    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanCancel");
            textbox_pBanReason.Text      = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanDefaultReason");
            textbox_pBanDurPopup.Text    = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanErrorDur");
            textbox_pBanReasonPopup.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList.Ban", "BanErrorReason");

            #endregion

            #endregion
            #region Map List

            label_GameMode.Content         = Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList", "GameMode");
            label_MapListFunctions.Content = Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList", "MapListFunc");
            label_MapPool.Content          = Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList", "MapPool");
            label_MapList.Content          = Local.Loc.Loc(null, "Procon.UI.GameWindow.MapList", "MapList");

            #endregion
            #region Ban List

            label_BanListFilter.Content         = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Filter");
            datagrid_BanListUidColumn.Header    = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Uid");
            datagrid_BanListNameColumn.Header   = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Name");
            datagrid_BanListTypeColumn.Header   = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Type");
            datagrid_BanListTimeColumn.Header   = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Time");
            datagrid_BanListReasonColumn.Header = Local.Loc.Loc(null, "Procon.UI.GameWindow.BanList", "Reason");

            #endregion
            #region Server Information

            label_ServerName.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "Name");
            label_ServerDescription.Content = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "Description");
            label_ServerBannerUrl.Content   = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "BannerUrl");

            checkbox_ServerPunkbuster.Content      = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "Punkbuster");
            checkbox_ServerRanked.Content          = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "Ranked");
            checkbox_ServerProfanityFilter.Content = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "ProfanityFilter");
            checkbox_ServerIdleKick.Content        = Local.Loc.Loc(null, "Procon.UI.GameWindow.ServerInfo", "IdleKick");

            #endregion
            #region Chat Box

            label_ChatBoxTitle.Content   = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "Title");
            label_ChatBoxFilter.Content  = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "Filter");
            button_ChatBoxFilter.Content = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "FilterMore");
            label_ChatBoxSendMsg.Content = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "SendMsg");
            label_ChatBoxSendTo.Content  = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "SendTo");
            button_ChatBoxSend.Content   = Local.Loc.Loc(null, "Procon.UI.GameWindow.ChatBox", "Send");

            #endregion

            #endregion

            // Watch for changes in the connection.
            connection.GameEvent += new Game.GameEventHandler(Game_ChangeOccurred);
        }

        void teams_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }


        /// <summary>
        /// Handles game events such as when a player joins or leaves.
        /// This is used to update the collections that the controls for this
        /// window are bound to.
        /// </summary>
        /// <param name="sender">The game that was changed.</param>
        /// <param name="e">Information about the event (such as a chat message).</param>
        private void Game_ChangeOccurred(Game sender, GameEventArgs e)
        {
            // Remember to make all calls to delegates within here so we
            // don't break the UI - The UI has to be managed on it's own thread.
            switch (e.EventType)
            {
                #region Player Join/Left/Swapped/List Updated

                case GameEventType.PlayerJoin:
                    break;
                case GameEventType.PlayerLeave:
                    break;
                case GameEventType.PlayerMoved:
                    // Refresh the player's list, because grouping get's all out of whack when
                    // a player's team changes after he has been added to the list.
                    // Save the currently selected players, refreshes the list, reselects players.
                    Dispatcher.BeginInvoke((System.Action)(() => { updatePlayers(); }));
                    break;
                case GameEventType.PlayerlistUpdated:
                    break;

                #endregion
                #region Player Kicked/Banned/Unbanned

                case GameEventType.PlayerKicked:
                    break;
                case GameEventType.PlayerBanned:
                    break;
                case GameEventType.PlayerUnbanned:
                    break;

                #endregion
                #region Player Killed/Spawned

                case GameEventType.PlayerKill:
                    break;
                case GameEventType.PlayerSpawn:
                    break;

                #endregion
                #region Chat/Round Change

                case GameEventType.Chat:
                    // Add the chat message to the chat/event box.  Filter out messages that get reflected
                    // back from the procon server.  These are simply duplicate messages from when you
                    // send a chat message to the server.
                    if (e.Chat.Origin != ChatOrigin.Reflected)
                        Dispatcher.BeginInvoke((System.Action)(() => { ChatMessages.Insert(0, e.Chat); }));
                    break;
                case GameEventType.RoundChanged:
                    // When the round changes, update the selections allowed when specifing a team or
                    // squad to perform some action (such as chatting or moving players).
                    Dispatcher.BeginInvoke((System.Action)(() =>
                    {
                        updateTeams();
                        updateSquads();
                        GameMode_Selected(GameModeFilter);
                    }));
                    break;

                #endregion
                #region ServerInfo/Banlist Updated, Config Executed

                case GameEventType.ServerInfoUpdated:
                    break;
                case GameEventType.BanlistUpdated:
                    break;
                case GameEventType.GameConfigExecuted:
                    break;

                #endregion
            }
        }


        /// <summary>
        /// Occurs whenever a button is clicked either on the main form
        /// or any of the tabs.
        /// </summary>
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            #region Move Players

            // Attempt to move the players.  Closes popup if successful.
            if (sender == button_pMoveOk)
                movePlayers();
            // Close the popup.
            else if (sender == button_pMoveCancel)
                closeMovePlayers();

            #endregion
            #region Warn Players

            // Attempt to warn the players.  Closes popup if successful.
            if (sender == button_pWarnOk)
                warnPlayers();
            // Close the popup.
            else if (sender == button_pWarnCancel)
                closeWarnPlayers();

            #endregion
            #region Kick Players

            // Attempt to kick the players.  Closes popup if successful.
            if (sender == button_pKickOk)
                kickPlayers();
            // Close the popup.
            else if (sender == button_pKickCancel)
                closeKickPlayers();

            #endregion
            #region Ban Players

            // Attempt to ban the players.  Closes popup if successful.
            if (sender == button_pBanOk)
                banPlayers();
            // Close the popup.
            else if (sender == button_pBanCancel)
                closeBanPlayers();


            #endregion

            #region Map List

            // Advance to the next round.
            if (sender == button_NextRound)
                (DataContext as Connection).Action(new Map() { MapActionType = MapActionType.NextRound });
            // Restart the current round.
            else if (sender == button_RestartRound)
                (DataContext as Connection).Action(new Map() { MapActionType = MapActionType.RestartRound });
            // Advance to the next map.
            else if (sender == button_NextMap)
                (DataContext as Connection).Action(new Map() { MapActionType = MapActionType.NextMap });
            // Restart the current map.
            else if (sender == button_RestartMap)
                (DataContext as Connection).Action(new Map() { MapActionType = MapActionType.RestartMap });
            // Change the game mode.
            else if (sender == button_GameMode)
                changeGameMode();
            // Add all the selected maps to the map list.
            else if (sender == button_AddToMapList)
                addToMapList();
            // Remove all the selected maps from the map list.
            else if (sender == button_RemoveFromMapList)
                removeFromMapList();
            // Move the selected items up in the map list.
            else if (sender == button_MapListUp)
                moveMapsUp();
            // Move the selected items down in the map list.
            else if (sender == button_MapListDown)
                moveMapsDown();

            #endregion

            #region Additional Filters

            // View the more filters popup.
            if (sender == button_ChatBoxFilter)
            {
            }

            #endregion
            #region Send Chat Message

            // Sends a chat message based on the options the user picked.
            if (sender == button_ChatBoxSend)
            {
                Chat chat = new Chat();
                chat.Text = textbox_ChatBoxSendMsg.Text.Trim();
                chat.ChatActionType = (ChatActionType)combobox_ChatBoxSendType.SelectedItem;
                switch ((PlayerSubsetContext)combobox_ChatBoxSendTo.SelectedItem)
                {
                    case PlayerSubsetContext.All:
                        chat.Subset = new PlayerSubset() { Context = PlayerSubsetContext.All };
                        break;
                    case PlayerSubsetContext.Team:
                        chat.Subset = new PlayerSubset() { Context = PlayerSubsetContext.Team, Team = (Team)combobox_ChatBoxSendToSubset.SelectedItem };
                        break;
                    case PlayerSubsetContext.Squad:
                        chat.Subset = new PlayerSubset() { Context = PlayerSubsetContext.Squad, Squad = (Squad)combobox_ChatBoxSendToSubset.SelectedItem };
                        break;
                    case PlayerSubsetContext.Player:
                        chat.Subset = new PlayerSubset() { Context = PlayerSubsetContext.Player, Player = (Player)combobox_ChatBoxSendToSubset.SelectedItem };
                        break;
                }
                Connection c = (DataContext as Connection);
                c.Action(chat);
                textbox_ChatBoxSendMsg.Text = String.Empty;
            }

            #endregion
        }
        /// <summary>
        /// Allows actions to be performed by simply pressing enter/esc on a textbox.
        /// An alternative to hitting the OK or Cancel button in many cases.
        /// </summary>
        private void TextBox_KeyPressed(object sender, KeyEventArgs e)
        {
            // Warn Players Popup.
            if (sender == textbox_pWarnReason)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pWarnOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pWarnCancel, null);
            }
            // Kick Players Popup.
            if (sender == textbox_pKickReason)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pKickOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pKickCancel, null);
            }
            // Ban Players Popup.
            if (sender == textbox_pBanReason)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pBanOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pBanCancel, null);
            }
            // Chat Box.
            if (sender == textbox_ChatBoxSendMsg)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_ChatBoxSend, null);
                else if (e.Key == Key.Escape)
                    textbox_ChatBoxSendMsg.Text = String.Empty;
            }
        }

        #region Events

        #region Context Menus

        /// <summary>
        /// Handles deciding which context menu to open when the user right clicks somewhere
        /// on the player list.  If the user clicks on one of the headers, it displays a list
        /// of headers that (either checked or unchecked) to manage their visibility.  If the
        /// user clicks on a row, it will open up player-specific actions about them.
        /// </summary>
        private void PlayerList_ContextMenuOpened(object sender, ContextMenuEventArgs e)
        {
            // User right clicked on a row.
            if (((FrameworkElement)e.OriginalSource).DataContext is Player)
            {
                datagrid_PlayerList.ContextMenu = PlayerListRowMenu;
                datagrid_PlayerList.ContextMenu.IsOpen = true;
            }
            // User right clicked on a column header.
            else
            {
                datagrid_PlayerList.ContextMenu = PlayerListColumnMenu;
                datagrid_PlayerList.ContextMenu.IsOpen = true;
            }
        }
        /// <summary>
        /// Handles actions to perform whenever the user clicks on a menu item
        /// after right clicking on a row in the player list.
        /// </summary>
        private void PlayerList_ContextMenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (sender as MenuItem);
            if (mi != null)
                switch (mi.Name)
                {
                    case "View":
                        // TODO - Create Soldier Popup
                        break;
                    case "Search":
                        combobox_ChatBoxFilterType.SelectedItem = ChatFilterType.Sender;
                        textbox_ChatBoxFilter.Text              = (datagrid_PlayerList.SelectedItem as Player).Name;
                        textbox_ChatBoxFilter.SelectionStart    = (datagrid_PlayerList.SelectedItem as Player).Name.Length;
                        textbox_ChatBoxFilter.Focus();
                        break;
                    case "Move":
                        popup_pMove.IsOpen = true;
                        combobox_pMoveToTeam.Focus();
                        break;
                    case "Warn":
                        popup_pWarn.IsOpen = true;
                        textbox_pWarnReason.Focus();
                        break;
                    case "Kill":
                        foreach (Player player in datagrid_PlayerList.SelectedItems)
                            (DataContext as Connection).Action(new Kill() { Target = player });
                        break;
                    case "Kick":
                        popup_pKick.IsOpen = true;
                        textbox_pKickReason.Focus();
                        break;
                    case "Ban":
                        popup_pBan.IsOpen = true;
                        combobox_pBanDurType.Focus();
                        break;
            }
        }

        /// <summary>
        /// Handles actions to perform whenever the user clicks on a menu item
        /// after right clicking on a row in the ban list.
        /// </summary>
        private void BanList_ContextMenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (sender as MenuItem);
            if (mi != null)
                switch (mi.Name)
                {
                    case "menuitem_UnBan":
                        Connection c = (DataContext as Connection);
                        foreach (Ban ban in datagrid_BanList.SelectedItems)
                            c.Action(new Ban()
                            {
                                BanActionType = BanActionType.Unban,
                                Target = ban.Target
                            });
                        break;
                    case "menuitem_ConvToPerm":
                        // TODO - Convert to permanent ban.
                        break;
                    case "menuitem_ConvToTemp":
                        // TODO - Convert to temporary ban.
                        break;
                }
        }

        #endregion
        #region Filters

        /// <summary>
        /// Filters items out of the map pool based on the filter specified by the user.
        /// </summary>
        private void MapPool_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            Map item = (Map)e.Item;
            if (GameModeFilter == null || item.GameMode.Name == GameModeFilter.Name)
                e.Accepted = true;
        }
        /// <summary>
        /// Filters items out of the ban list based on the filter specified by the user.
        /// </summary>
        private void BanList_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            Ban item = (Ban)e.Item;
            if (BanFilterText != String.Empty)
            {
                switch (BanFilterType)
                {
                    case BanFilterType.Uid:
                        if (item.Target.Uid.ToLower().Contains(BanFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case BanFilterType.Name:
                        if (item.Target.Name.ToLower().Contains(BanFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case BanFilterType.Type:
                        if (Local.Loc.Loc(null, "Procon.UI.TimeSubsetContext", item.Time.Context.ToString()).ToLower().Contains(BanFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case BanFilterType.Time:
                        if (item.Time.Length.ToString().ToLower().Contains(BanFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                }
            }
            else
                e.Accepted = true;
        }
        /// <summary>
        /// Filters items out of the chatbox based on the filter specified by the user.
        /// </summary>
        private void ChatBox_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            Chat item = (Chat)e.Item;
            if (ChatFilterText != String.Empty)
            {
                switch (ChatFilterType)
                {
                    case ChatFilterType.Time:
                        if (item.Created.ToShortTimeString().ToLower().Contains(ChatFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case ChatFilterType.Type:
                        if (Local.Loc.Loc(null, "Procon.UI.ChatActionType", item.ChatActionType.ToString()).ToLower().Contains(ChatFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case ChatFilterType.Sender:
                        if (item.Author.Name.ToLower().Contains(ChatFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case ChatFilterType.ReceivingSubset:
                        if (Local.Loc.Loc(null, "Procon.UI.PlayerSubsetContext", item.Subset.Context.ToString()).ToLower().Contains(ChatFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                    case ChatFilterType.ReceivingEntity:
                        if (item.Subset.Context == PlayerSubsetContext.All)
                            if (Local.Loc.Loc(null, "Procon.UI.PlayerSubsetContext", PlayerSubsetContext.All.ToString()).ToLower().Contains(ChatFilterText.ToLower()))
                                e.Accepted = true;
                        if (item.Subset.Context == PlayerSubsetContext.Team)
                            if (Local.Loc.Loc(null, "Procon.UI.Team", item.Subset.Team.ToString()).ToLower().Contains(ChatFilterText.ToLower()))
                                e.Accepted = true;
                        if (item.Subset.Context == PlayerSubsetContext.Squad)
                            if (Local.Loc.Loc(null, "Procon.UI.Squad", item.Subset.Squad.ToString()).ToLower().Contains(ChatFilterText.ToLower()))
                                e.Accepted = true;
                        if (item.Subset.Context == PlayerSubsetContext.Player)
                            if (item.Subset.Player.Name.ToString().ToLower().Contains(ChatFilterText.ToLower()))
                                e.Accepted = true;
                        break;
                    case ChatFilterType.Message:
                        if (item.Text.ToString().ToLower().Contains(ChatFilterText.ToLower()))
                            e.Accepted = true;
                        break;
                }
            }
            else
                e.Accepted = true;
        }


        #endregion
        #region Selections

        /// <summary>
        /// Handles special cases whenever a game mode is selected.
        /// </summary>
        private void GameMode_Selected(GameMode gameMode)
        {
            // If the game is Battlefield Bad Company 2, the map list can only
            // contain maps from a single game mode.  Make everything disabled
            // until either the user selects the original game mode or applies
            // the current game mode.
            if ((DataContext as Connection).GameType == GameType.BF_BC2)
                if (gameMode.Name != (DataContext as Connection).GameState.Get<String>(ServerVariableKey.GameMode))
                {
                    listbox_MapPool.IsEnabled          = false;
                    button_AddToMapList.IsEnabled      = false;
                    button_RemoveFromMapList.IsEnabled = false;
                    button_GameMode.IsEnabled          = true;
                }
                else
                {
                    listbox_MapPool.IsEnabled          = true;
                    button_AddToMapList.IsEnabled      = true;
                    button_RemoveFromMapList.IsEnabled = true;
                    button_GameMode.IsEnabled          = false;
                }
        }

        /// <summary>
        /// Populates the "ChatBoxSendToSubset" combo box with the appropriate subsets
        /// depending on what was selected as the "ChatBoxSendTo" group.
        /// </summary>
        private void Subset_Selected(object sender, SelectionChangedEventArgs e)
        {
            // All
            if ((PlayerSubsetContext)combobox_ChatBoxSendTo.SelectedItem == PlayerSubsetContext.All)
            {
                combobox_ChatBoxSendToSubset.ItemsSource = new List<Enum>() { PlayerSubsetContext.All };
                combobox_ChatBoxSendToSubset.SelectedItem = PlayerSubsetContext.All;
            }
            // Team
            else if ((PlayerSubsetContext)combobox_ChatBoxSendTo.SelectedItem == PlayerSubsetContext.Team)
            {
                combobox_ChatBoxSendToSubset.ItemsSource = Teams;
                if (combobox_ChatBoxSendToSubset.Items.Count > 0)
                    combobox_ChatBoxSendToSubset.SelectedIndex = 0;
            }
            // Squad
            else if ((PlayerSubsetContext)combobox_ChatBoxSendTo.SelectedItem == PlayerSubsetContext.Squad)
            {
                combobox_ChatBoxSendToSubset.ItemsSource = Squads;
                if (combobox_ChatBoxSendToSubset.Items.Count > 0)
                    combobox_ChatBoxSendToSubset.SelectedIndex = 0;
            }
            // Player
            else if ((PlayerSubsetContext)combobox_ChatBoxSendTo.SelectedItem == PlayerSubsetContext.Player)
            {
                combobox_ChatBoxSendToSubset.ItemsSource = (Resources["cvs_Players"] as CollectionViewSource).View;
                if (combobox_ChatBoxSendToSubset.Items.Count > 0)
                    combobox_ChatBoxSendToSubset.SelectedIndex = 0;
            }
        }

        #endregion

        #endregion
        #region Manual UI Update Functions

        /// <summary>
        /// Updates the current teams that this game (mode) supports.
        /// </summary>
        private void updateTeams()
        {
            Connection c = (DataContext as Connection);
            GameMode gm  = c.GameState.GameModePool.Find(x => x.Name == c.GameState.Get<String>(ServerVariableKey.GameMode));
            if (gm != null)
            {
                // Populate only Neutral + number of teams.
                Int32 numTeams = 0;
                Teams.Clear();
                foreach (Team team in Enum.GetValues(typeof(Team)))
                    if (team != Team.None && numTeams++ <= gm.TeamCount)
                        Teams.Add(team);
            }
        }

        /// <summary>
        /// Updates the current squads that this game (mode) supports.
        /// </summary>
        private void updateSquads()
        {
            Squads.Clear();
            foreach (Squad squad in Enum.GetValues(typeof(Squad)))
                Squads.Add(squad);
        }

        /// <summary>
        /// Refreshes the player list and makes sure to keep the selections made
        /// before the player list was refreshed.
        /// </summary>
        private void updatePlayers()
        {
            List<Player> selectedItems = new List<Player>();
            foreach (Player p in datagrid_PlayerList.SelectedItems)
                selectedItems.Add(p);
            (Resources["cvs_Players"] as CollectionViewSource).View.Refresh();
            foreach (Player p in selectedItems)
                datagrid_PlayerList.SelectedItems.Add(p);
        }

        #endregion

        #region Map List Functions

        /// <summary>
        /// Destroys the current map list and changes the game mode to what
        /// the user specified.  Then, advances the list one round to force
        /// a map change to the new game mode.
        /// </summary>
        private void changeGameMode()
        {
            Connection c = (DataContext as Connection);

            // Tell server to change the game mode on map change.
            c.Action(new Map()
            {
                MapActionType = MapActionType.ChangeMode,
                GameMode = GameModeFilter
            });
            // Populate the default play list of maps.
            foreach (Map map in listbox_MapPool.Items)
                if (map.GameMode.Name == GameModeFilter.Name)
                    c.Action(new Map()
                    {
                        MapActionType = MapActionType.Append,
                        Name          = map.Name
                    });
            // End the current map and move to next map.
            c.Action(new Map()
            {
                MapActionType = MapActionType.NextRound
            });
        }
        /// <summary>
        /// Adds the selected from the map pool to the map list.
        /// </summary>
        private void addToMapList()
        {
            Connection c = (DataContext as Connection);

            // Save selected maps into separate list.
            List<Map> selectedMaps = new List<Map>();
            foreach (Map map in listbox_MapPool.SelectedItems)
                selectedMaps.Add(map);
            selectedMaps.Sort((x, y) => String.Compare(x.Name, y.Name));
            // Add the selected items to the list.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Name          = map.Name,
                    MapActionType = MapActionType.Append
                });
        }
        /// <summary>
        /// Removes the selected maps from the map list.
        /// </summary>
        private void removeFromMapList()
        {
            Connection c = (DataContext as Connection);

            // Save selected maps into separate list.
            List<Map> selectedMaps = new List<Map>();
            foreach (Map map in listbox_MapList.SelectedItems)
                selectedMaps.Add(map);
            selectedMaps.Sort((x, y) => y.Index - x.Index);
            // Remove the selected items from the list.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Index         = map.Index,
                    MapActionType = MapActionType.RemoveIndex
                });
        }
        /// <summary>
        /// Moves the selected maps up.
        /// </summary>
        private void moveMapsUp()
        {
            Connection c = (DataContext as Connection);

            // Save selected maps into separate list.
            List<Map> selectedMaps = new List<Map>();
            foreach (Map map in listbox_MapList.SelectedItems)
                selectedMaps.Add(map);
            selectedMaps.Sort((x, y) => y.Index - x.Index);
            // Remove the selected items from the list.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Index         = map.Index,
                    MapActionType = MapActionType.RemoveIndex
                });
            selectedMaps.Sort((x, y) => x.Index - y.Index);
            // Add the selected items back 1 index up.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Name          = map.Name,
                    Index         = map.Index - 1,
                    MapActionType = MapActionType.Insert
                });
        }
        /// <summary>
        /// Moves the selected maps down.
        /// </summary>
        private void moveMapsDown()
        {
            Connection c = (DataContext as Connection);

            // Save selected maps into separate list.
            List<Map> selectedMaps = new List<Map>();
            foreach (Map map in listbox_MapList.SelectedItems)
                selectedMaps.Add(map);
            selectedMaps.Sort((x, y) => y.Index - x.Index);
            // Remove the selected items from the list.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Index         = map.Index,
                    MapActionType = MapActionType.RemoveIndex
                });
            selectedMaps.Sort((x, y) => x.Index - y.Index);
            // Add the selected items back 1 index down.
            foreach (Map map in selectedMaps)
                c.Action(new Map()
                {
                    Name          = map.Name,
                    Index         = map.Index + 1,
                    MapActionType = MapActionType.Insert
                });
        }

        #endregion
        #region Move/Warn/Kick/Ban Functions

        /// <summary>
        /// Moves the selected players to the specified team/squad and closes the popup.
        /// </summary>
        private void movePlayers()
        {
            // Get info from UI.
            Team  team  = (Team)combobox_pMoveToTeam.SelectedItem;
            Squad squad = (Squad)combobox_pMoveToSquad.SelectedItem;

            // Move each player to the specified destination.
            foreach (Player player in datagrid_PlayerList.SelectedItems)
                (DataContext as Connection).Action(new Move()
                {
                    MoveActionType = MoveActionType.ForceMove,
                    Destination = new PlayerSubset()
                    {
                        Team = team,
                        Squad = squad
                    },
                    Target = player
                });
            closeMovePlayers();
        }
        /// <summary>
        /// Closes the Move Players popup window.
        /// </summary>
        private void closeMovePlayers()
        {
            combobox_pMoveToTeam.SelectedItem  = Team.Team1;
            combobox_pMoveToSquad.SelectedItem = Squad.None;
            popup_pMove.IsOpen = false;
            datagrid_PlayerList.Focus();
        }

        /// <summary>
        /// Error checks the user's input to see if they entered a valid reason (it
        /// isn't blank).  Displays popup box containing an error message if there
        /// is an error, otherwise it yells at the players and closes the popup.
        /// </summary>
        private void warnPlayers()
        {
            // Clear old errors.
            popup_pWarnReason.IsOpen = false;

            // Get info from UI.
            String warnReason = textbox_pWarnReason.Text.Trim();

            // Error check info.
            if (warnReason == String.Empty)
                popup_pWarnReason.IsOpen = true;

            // Yell at players if there are no errors.
            if (!popup_pWarnReason.IsOpen)
            {
                foreach (Player player in datagrid_PlayerList.SelectedItems)
                    (DataContext as Connection).Action(new Chat()
                    {
                        ChatActionType = ChatActionType.Yell,
                        Subset = new PlayerSubset()
                        {
                            Context = PlayerSubsetContext.Player,
                            Player = player
                        },
                        Text = textbox_pWarnReason.Text
                    });
                closeWarnPlayers();
            }
        }
        /// <summary>
        /// Closes the Warn Players popup window.
        /// </summary>
        private void closeWarnPlayers()
        {
            textbox_pWarnReason.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "WarnDefaultReason");
            popup_pWarn.IsOpen        = false;
            datagrid_PlayerList.Focus();
        }

        /// <summary>
        /// Error checks the user's input to see if they entered a valid reason (it
        /// isn't blank).  Displays popup box containing an error message if there
        /// is an error, otherwise it kicks the players and closes the popup.
        /// </summary>
        private void kickPlayers()
        {
            // Clear old errors.
            popup_pKickReason.IsOpen = false;

            // Get info from UI.
            String kickReason = textbox_pKickReason.Text.Trim();

            // Error check info.
            if (kickReason == String.Empty)
                popup_pKickReason.IsOpen = true;

            // Kick players if there are no errors.
            if (!popup_pKickReason.IsOpen)
            {
                foreach (Player player in datagrid_PlayerList.SelectedItems)
                    (DataContext as Connection).Action(new Kick()
                    {
                        Target = player,
                        Reason = kickReason
                    });
                closeKickPlayers();
            }
        }
        /// <summary>
        /// Closes the Kick Players popup window.
        /// </summary>
        private void closeKickPlayers()
        {
            textbox_pKickReason.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "KickDefaultReason");
            popup_pKick.IsOpen       = false;
            datagrid_PlayerList.Focus();
        }

        /// <summary>
        /// Error checks the user's input to see if they entered a valid duration (> 0)
        /// and reason (it isn't blank).  Displays popup box containing an error
        /// message if there is an error, otherwise it bans the players and closes
        /// the popup.
        /// </summary>
        private void banPlayers()
        {
            // Clear old errors.
            popup_pBanDur.IsOpen    = false;
            popup_pBanReason.IsOpen = false;

            // Get info from UI.
            String banDuration = textbox_pBanDur.Text.Trim();
            String banReason   = textbox_pKickReason.Text.Trim();

            // Error check info.
            UInt32 tempDuration = 0;
            if ((banDuration == String.Empty || !UInt32.TryParse(banDuration, out tempDuration)) && label_pBanDur.IsVisible)
                popup_pBanDur.IsOpen = true;
            if (banReason == String.Empty)
                popup_pBanReason.IsOpen = true;

            // Ban players if there are no errors.
            if (!popup_pBanDur.IsOpen && !popup_pBanReason.IsOpen)
            {
                foreach (Player player in datagrid_PlayerList.SelectedItems)
                    (DataContext as Connection).Action(new Ban()
                    {
                        BanActionType = BanActionType.Ban,
                        Target = player,
                        Reason = banReason,
                        Time = new TimeSubset()
                        {
                            Context = (TimeSubsetContext)combobox_pBanDurType.SelectedItem,
                            Length  = TimeSpan.FromMinutes(tempDuration)
                        }
                    });
                closeBanPlayers();
            }
        }
        /// <summary>
        /// Closes the Kick Players popup window.
        /// </summary>
        private void closeBanPlayers()
        {
            textbox_pBanDur.Text    = "60";
            textbox_pBanReason.Text = Local.Loc.Loc(null, "Procon.UI.GameWindow.PlayerList", "BanDefaultReason");
            popup_pBan.IsOpen = false;
            datagrid_PlayerList.Focus();
        }

        #endregion

        /// <summary>
        /// Allows the window to actually close instead of being hidden.
        /// This replaces the normal close call for anything outside of this window.
        /// This means, users who attempt to close the window via the "x" button will
        /// only hide the window, yet it will seem like it closed for them.
        /// </summary>
        new public void Close()
        {
            mShouldClose = true;
            base.Close();
        }
        /// <summary>
        /// This is fired whenever the user tries to close the window in some way.
        /// Instead of closing the window and destroying all the controls, this method
        /// redirects the close so that the window is hidden in the background. If
        /// "mShouldClose" is set, this method operates as it normally would.
        /// </summary>
        private void Exiting_Game(object sender, CancelEventArgs e)
        {
            if (!mShouldClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

    }
}
