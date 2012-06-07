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
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Procon.Core.Interfaces.Connections;
using Procon.Net;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.UI.API.Enums;

namespace Procon.UI.API.ViewModels
{
    /// <summary>Wraps a Connection of Procon so that it can be used in the UI.</summary>
    public class ConnectionViewModel : ViewModelBase<Connection>
    {
        // Standard Model Properties
        public GameType GameType
        {
            get { return Model.GameType; }
        }
        public String Hostname
        {
            get { return Model.Hostname; }
        }
        public UInt16 Port
        {
            get { return Model.Port; }
        }
        public String Additional
        {
            get { return Model.Additional; }
        }

        // Standard Variables
        public Int32 MaxConsoleLines
        {
            get { return Model.GameState.Variables.MaxConsoleLines;  }
            set { Model.GameState.Variables.MaxConsoleLines = value; }
        }
        public ConnectionState ConnectionState
        {
            get { return Model.GameState.Variables.ConnectionState; }
        }
        public Int32 UpTime
        {
            get { return Model.GameState.Variables.UpTime; }
        }
        public String Version
        {
            get { return Model.GameState.Variables.Version; }
        }
        public String ServerName
        {
            get { return Model.GameState.Variables.ServerName; }
            set { Model.GameState.Variables.ServerName = value; }
        }
        public String ServerDescription
        {
            get { return Model.GameState.Variables.ServerDescription; }
            set { Model.GameState.Variables.ServerDescription = value; }
        }
        public String BannerUrl
        {
            get { return Model.GameState.Variables.BannerUrl; }
            set { Model.GameState.Variables.BannerUrl = value; }
        }
        public Boolean Ranked
        {
            get { return Model.GameState.Variables.Ranked; }
            set { Model.GameState.Variables.Ranked = value; }
        }
        public Boolean AntiCheat
        {
            get { return Model.GameState.Variables.AntiCheat; }
            set { Model.GameState.Variables.AntiCheat = value; }
        }
        public Boolean AutoBalance
        {
            get { return Model.GameState.Variables.AutoBalance; }
            set { Model.GameState.Variables.AutoBalance = value; }
        }
        public Boolean FriendlyFire
        {
            get { return Model.GameState.Variables.FriendlyFire; }
            set { Model.GameState.Variables.FriendlyFire = value; }
        }
        public Boolean Passworded
        {
            get { return Model.GameState.Variables.Passworded; }
            set { Model.GameState.Variables.Passworded = value; }
        }
        public String Password
        {
            get { return Model.GameState.Variables.Password; }
            set { Model.GameState.Variables.Password = value; }
        }
        public Int32 RoundTime
        {
            get { return Model.GameState.Variables.RoundTime; }
        }
        public Int32 RoundIndex
        {
            get { return Model.GameState.Variables.RoundIndex; }
        }
        public Int32 MaxRoundIndex
        {
            get { return Model.GameState.Variables.MaxRoundIndex; }
            set { Model.GameState.Variables.MaxRoundIndex = value; }
        }
        public Int32 PlayerCount
        {
            get { return Model.GameState.Variables.PlayerCount; }
        }
        public Int32 MaxPlayerCount
        {
            get { return Model.GameState.Variables.MaxPlayerCount; }
            set { Model.GameState.Variables.MaxPlayerCount = value; }
        }
        public String MapName
        {
            get { return Model.GameState.Variables.MapName; }
        }
        public String GameModeName
        {
            get { return Model.GameState.Variables.GameModeName; }
        }

        // Custom Properties
        public BitmapImage GameTypeIcon
        {
            get
            {
                switch (GameType)
                {
                    case GameType.BF_BC2:
                        return InstanceViewModel.PublicProperties["Images"]["Games"]["BF_BC2"].Value as BitmapImage;
                    case GameType.BF_3:
                        return InstanceViewModel.PublicProperties["Images"]["Games"]["BF_3"].Value as BitmapImage;
                    case GameType.COD_BO:
                        return InstanceViewModel.PublicProperties["Images"]["Games"]["COD_BO"].Value as BitmapImage;
                    case GameType.Homefront:
                        return InstanceViewModel.PublicProperties["Images"]["Games"]["Homefront"].Value as BitmapImage;
                    case GameType.MOH_2010:
                        return InstanceViewModel.PublicProperties["Images"]["Games"]["MOH_2010"].Value as BitmapImage;
                    default:
                        return new BitmapImage();

                }
            }
        }
        public BitmapImage ConnectionStateIcon
        {
            get
            {
                switch (ConnectionState)
                {
                    case ConnectionState.LoggedIn:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Good"].Value as BitmapImage;
                    case ConnectionState.Connecting:
                    case ConnectionState.Connected:
                    case ConnectionState.Ready:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Flux"].Value as BitmapImage;
                    case ConnectionState.Disconnecting:
                    case ConnectionState.Disconnected:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Bad"].Value as BitmapImage;
                    default:
                        return new BitmapImage();
                }
            }
        }
        public BitmapImage RankedIcon
        {
            get {
                if (Ranked)
                    return InstanceViewModel.PublicProperties["Images"]["Info"]["Ranked"].Value as BitmapImage;
                return InstanceViewModel.PublicProperties["Images"]["Info"]["NotRanked"].Value as BitmapImage;
            }
        }
        public BitmapImage AntiCheatIcon
        {
            get {
                if (AntiCheat)
                    return InstanceViewModel.PublicProperties["Images"]["Info"]["Secure"].Value as BitmapImage;
                return InstanceViewModel.PublicProperties["Images"]["Info"]["NotSecure"].Value as BitmapImage;
            }
        }
        public BitmapImage PasswordedIcon
        {
            get {
                if (Passworded)
                    return InstanceViewModel.PublicProperties["Images"]["Info"]["Passworded"].Value as BitmapImage;
                return InstanceViewModel.PublicProperties["Images"]["Info"]["NotPassworded"].Value as BitmapImage;
            }
        }
        public BitmapImage AutoBalanceIcon
        {
            get {
                if (AutoBalance)
                    return InstanceViewModel.PublicProperties["Images"]["Info"]["AutoBalanced"].Value as BitmapImage;
                return InstanceViewModel.PublicProperties["Images"]["Info"]["NotAutoBalanced"].Value as BitmapImage;
            }
        }

        // View Model Properties
        public  ObservableCollection<PlayerViewModel> Players
        {
            get { return mPlayers; }
            set
            {
                mPlayers = value;
                OnPropertyChanged(this, "Players");
            }
        }
        private ObservableCollection<PlayerViewModel> mPlayers;

        public  ObservableCollection<MapViewModel> Maps
        {
            get { return mMapList; }
            set
            {
                mMapList = value;
                OnPropertyChanged(this, "MapList");
            }
        }
        private ObservableCollection<MapViewModel> mMapList;

        public  ObservableCollection<BanViewModel> Bans
        {
            get { return mBans; }
            set
            {
                mBans = value;
                OnPropertyChanged(this, "Bans");
            }
        }
        private ObservableCollection<BanViewModel> mBans;

        // TODO: This should use GameModeViewModel?
        public ObservableCollection<String> GameModePool
        {
            get { return mGameModePool; }
            set
            {
                mGameModePool = value;
                OnPropertyChanged(this, "GameModePool");
            }
        }
        private ObservableCollection<String> mGameModePool;

        public  ObservableCollection<MapViewModel> MapPool
        {
            get { return mMapPool; }
            set
            {
                mMapPool = value;
                OnPropertyChanged(this, "MapPool");
            }
        }
        private ObservableCollection<MapViewModel> mMapPool;

        public  ObservableCollection<DataVariable> Variables
        {
            get { return mVariables; }
            set
            {
                mVariables = value;
                OnPropertyChanged(this, "Variables");
            }
        }
        private ObservableCollection<DataVariable> mVariables;

        //public  ObservableCollection<Event> Events
        //{
        //    get { return mEvents; }
        //    set
        //    {
        //        mEvents = value;
        //        OnPropertyChanged(this, "Events");
        //    }
        //}
        //private ObservableCollection<Event> mEvents;

        /// <summary>Creates an instance of ConnectionViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to an instance of a connection in procon.</param>
        public ConnectionViewModel(Connection model) : base(model)
        {
            // Listen for changes within the model:
            Model.PropertyChanged                     += Connection_PropertyChanged;
            Model.GameState.Variables.DataAdded       += Variables_DataAdded;
            Model.GameState.Variables.DataRemoved     += Variables_DataRemoved;
            Model.GameState.Variables.PropertyChanged += Variables_PropertyChanged;
            Model.GameEvent                           += GameEventOccurred;

            // Expose collections within the model:
            Players      = new ObservableCollection<PlayerViewModel>(Model.GameState.PlayerList.Select(x => new PlayerViewModel(x)));
            Maps         = new ObservableCollection<MapViewModel>(Model.GameState.MapList.Select(x => new MapViewModel(x)));
            Bans         = new ObservableCollection<BanViewModel>(Model.GameState.BanList.Select(x => new BanViewModel(x)));
            GameModePool = new ObservableCollection<String>(Model.GameState.GameModePool.Select(x => x.FriendlyName));
            MapPool      = new ObservableCollection<MapViewModel>(Model.GameState.MapPool.Select(x => new MapViewModel(x)));
            Variables    = new ObservableCollection<DataVariable>(Model.GameState.Variables.Variables.Where(x => !x.IsReadOnly).OrderBy(x => x.Name));

            // Manages a logging of events within the model:
            //Events = new ObservableCollection<Event>();
        }



        /// <summary>
        /// Attempts to perform the specified action for the connection.
        /// </summary>
        public void Action(ProtocolObject action)
        {
            Model.Action(action);
        }



        /// <summary>
        /// Is notified when a change in the game is made.  We monitor the notification to alter
        /// visible collections so the UI knows the collections have been changed.
        /// </summary>
        private void GameEventOccurred(Game sender, GameEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => GameEventOccurred(sender, e)))
                return;

            Boolean add;
            Boolean remove;
            Boolean existing;
            //Event   newEvent;
            switch (e.EventType)
            {
                /* Player Joined Event:
                 * 
                 * [Players Collection]
                 * Updates the player list with the new player, if he is not in 
                 * the list already.  Adds the new player to the list without
                 * affecting the user's selection.
                 * 
                 * [Events Collection]
                 * Adds a PlayerJoined event to the chat and events log. */
                #region PlayerJoin
                case GameEventType.PlayerJoin:
                    // -- [Players] --
                    existing = false;
                    foreach (PlayerViewModel pvm in Players)
                        if (pvm.Uid == e.Player.UID)
                        {
                            existing = true;
                            break;
                        }
                    if (!existing)
                        Players.Add(new PlayerViewModel(e.Player));


                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Join, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player Left Event:
                 * 
                 * [Players Collection]
                 * Removes the player from the player list, if he/she is in the
                 * player list.  Does not affect the user's selection, unless the
                 * player removed is selected.
                 * 
                 * [Events Collection]
                 * Adds a PlayerLeft event to the chat and events log. */
                #region PlayerLeave
                case GameEventType.PlayerLeave:
                    // -- [Players] --
                    foreach (PlayerViewModel pvm in Players)
                        if (pvm.Uid == e.Player.UID)
                        {
                            Players.Remove(pvm);
                            break;
                        }

                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Leave, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player Moved Event:
                 * 
                 * [Events Collection]
                 * Adds a PlayerMoved event to the chat and events log. */
                #region PlayerMoved
                case GameEventType.PlayerMoved:
                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Move, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player List Updated Event:
                 * 
                 * [Players Collection]
                 * This is a combination of Remove Old and Add New entries.
                 * So instead of doing a "Clear" and "Add New" method, which
                 * gets rid of the user's current selection, I took a little
                 * more expensive route and manually updated all the properties
                 * in each view model, removed all the old entries, and added
                 * all the new entries. */
                #region PlayerlistUpdated
                case GameEventType.PlayerlistUpdated:
                    // -- [Players] --
                    // Remove Old Entries:
                    for (int i = 0; i < Players.Count; i++)
                    {
                        remove = true;
                        foreach (Player p in Model.GameState.PlayerList)
                            if (Players[i].Uid == p.UID) {
                                remove = false;
                                break; }
                        if (remove)
                            Players.RemoveAt(i--);
                    }
                    // Add New Entries:
                    for (int i = 0; i < Model.GameState.PlayerList.Count; i++)
                    {
                        add = true;
                        foreach (PlayerViewModel pvm in Players)
                            if (Model.GameState.PlayerList[i].UID == pvm.Uid) {
                                add = false;
                                break; }
                        if (add)
                            Players.Add(new PlayerViewModel(Model.GameState.PlayerList[i]));
                    }
                    break;
                #endregion





                /* Player Kicked Event:
                 * 
                 * [Events Collection]
                 * Adds a PlayerKicked event to the chat and events log. */
                #region PlayerKicked
                case GameEventType.PlayerKicked:
                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Kick, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player Banned Event:
                 * 
                 * [Bans Collection]
                 * Updates the ban list with the new ban, if it is not in 
                 * the list already.  Adds the new ban to the list without
                 * affecting the user's selection.
                 * 
                 * [Events Collection]
                 * Adds a PlayerBanned event to the chat and events log. */
                #region PlayerBanned
                case GameEventType.PlayerBanned:
                    // -- [Bans] --
                    existing = false;
                    foreach (BanViewModel bvm in Bans)
                        if (bvm.Target.GUID == e.Ban.Target.GUID)
                        {
                            existing = true;
                            break;
                        }
                    if (!existing)
                        Bans.Add(new BanViewModel(e.Ban));

                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Ban, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;

                #endregion

                /* Player Unbanned Event:
                 * 
                 * [Bans Collection]
                 * Removes the ban from the player list, if it is in the
                 * ban list.  Does not affect the user's selection, unless the
                 * ban removed is selected.
                 * 
                 * [Events Collection]
                 * Adds a PlayerUnbanned event to the chat and events log. */
                #region PlayerUnbanned
                case GameEventType.PlayerUnbanned:
                    // -- [Bans] --
                    foreach (BanViewModel bvm in Bans)
                        if (bvm.Target.GUID == e.Ban.Target.GUID)
                        {
                            Bans.Remove(bvm);
                            break;
                        }

                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Unban, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Ban List Updated Event:
                 * 
                 * [Bans Collection]
                 * This is a combination of Remove Old and Add New entries.
                 * So instead of doing a "Clear" and "Add New" method, which
                 * gets rid of the user's current selection, I took a little
                 * more expensive route and manually updated all the properties
                 * in each view model, removed all the old entries, and added
                 * all the new entries. */
                #region BanlistUpdated
                case GameEventType.BanlistUpdated:
                    // -- [Bans] --
                    // Remove Old Entries:
                    for (int i = 0; i < Bans.Count; i++)
                    {
                        remove = true;
                        foreach (Ban b in Model.GameState.BanList)
                            if (Bans[i].Target.GUID == b.Target.GUID) {
                                remove = false;
                                break; }
                        if (remove)
                            Bans.RemoveAt(i--);
                    }
                    // Update/Add New Entries:
                    for (int i = 0; i < Model.GameState.BanList.Count; i++)
                    {
                        add = true;
                        foreach (BanViewModel bvm in Bans)
                            if (Model.GameState.BanList[i].Target.GUID == bvm.Target.GUID) {
                                bvm.Length = Model.GameState.BanList[i].Time.Length;
                                add = false;
                                break; }
                        if (add)
                            Bans.Add(new BanViewModel(Model.GameState.BanList[i]));
                    }
                    break;
                #endregion





                /* Player Killed Event:
                 * 
                 * [Events Collection]
                 * Adds a PlayerKilled event to the chat and events log. */
                #region PlayerKill
                case GameEventType.PlayerKill:
                    // -- [Events] --
                    //if (e.Kill != null && e.Kill.Target != null)
                    //    if ((newEvent = Event.CreateEvent(EventType.Kill, e)).Type != String.Empty)
                    //        Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player Spawned Event:
                 * 
                 * [Events Collection]
                 * Adds a PlayerSpawned event to the chat and events log. */
                #region PlayerSpawn
                case GameEventType.PlayerSpawn:
                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Spawn, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Player Chat Event:
                 * 
                 * [Events Collection]
                 * Adds a Chat event to the chat and events log. */
                #region Chat
                case GameEventType.Chat:
                    // -- [Events] --
                    //if (e.Chat != null && e.Chat.Origin != ChatOrigin.Reflected)
                    //    if ((newEvent = Event.CreateEvent(EventType.Chat, e)).Type != String.Empty)
                    //        Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Round Changed Event:
                 * 
                 * [Events Collection]
                 * Adds a RoundChanged event to the chat and events log. */
                #region RoundChanged
                case GameEventType.RoundChanged:
                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Round, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Map Changed Event:
                 * 
                 * [Events Collection]
                 * Adds a MapChanged event to the chat and events log. */
                #region MapChanged
                case GameEventType.MapChanged:
                    // -- [Events] --
                    //if ((newEvent = Event.CreateEvent(EventType.Map, e)).Type != String.Empty)
                    //    Events.Insert(0, newEvent);
                    break;
                #endregion

                /* Map List Updated Event:
                 * 
                 * [Maps Collection]
                 * This is a combination of Remove Old and Add New entries.
                 * So instead of doing a "Clear" and "Add New" method, which
                 * gets rid of the user's current selection, I took a little
                 * more expensive route and manually updated all the properties
                 * in each view model, removed all the old entries, and added
                 * all the new entries. */
                #region MaplistUpdated
                case GameEventType.MaplistUpdated:
                    // -- [Maps] --
                    // Remove Old Entries:
                    for (int i = 0; i < Maps.Count; i++)
                    {
                        remove = true;
                        foreach (Map m in Model.GameState.MapList)
                            if (Maps[i].Name == m.Name && Maps[i].Index == m.Index) {
                                remove = false;
                                break; }
                        if (remove)
                            Maps.RemoveAt(i--);
                    }
                    // Update/Add New Entries:
                    for (int i = 0; i < Model.GameState.MapList.Count; i++)
                    {
                        add = true;
                        foreach (MapViewModel mvm in Maps)
                            if (Model.GameState.MapList[i].Name == mvm.Name && Model.GameState.MapList[i].Index == mvm.Index) {
                                add = false;
                                break; }
                        if (add)
                            Maps.Add(new MapViewModel(Model.GameState.MapList[i]));
                    }
                    break;
                #endregion

                case GameEventType.ServerInfoUpdated:
                    break;
                case GameEventType.GameConfigExecuted:
                    break;
            }
        }
        /// <summary>
        /// Is notified when a new data variable is added to the class.
        /// </summary>
        private void Variables_DataAdded(DataController parent, DataVariable item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_DataAdded(parent, item)))
                return;

            if (!item.IsReadOnly)
                for (int i = 0; i < Variables.Count; i++)
                    if (item.Name.CompareTo(Variables[i].Name) < 0) {
                        Variables.Insert(i, item);
                        break;
                    }
        }
        /// <summary>
        /// Is notified when a data variable is removed from the class all-together.
        /// </summary>
        private void Variables_DataRemoved(DataController parent, DataVariable item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_DataRemoved(parent, item)))
                return;

            for (int i = 0; i < Variables.Count; i++)
                if (item.Name == Variables[i].Name) {
                    Variables.RemoveAt(i);
                    break;
                }
        }
        /// <summary>
        /// Is notified when a data variable is changed in the class.
        /// </summary>
        private void Variables_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_PropertyChanged(sender, e)))
                return;
            
            // Connection State was updated.
            if (e.PropertyName == "ConnectionState") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            if (e.PropertyName == "Ranked") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            if (e.PropertyName == "AntiCheat") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            if (e.PropertyName == "Passworded") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            if (e.PropertyName == "AutoBalance") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            else OnPropertyChanged(this, e.PropertyName);
        }
        /// <summary>
        /// Lets the UI's view model know a property in the model changed.
        /// </summary>
        private void Connection_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connection_PropertyChanged(sender, e)))
                return;

            // PlayerList collection was re-set?
            if (e.PropertyName == "PlayerList") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < Players.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.GameState.PlayerList.Count; j++)
                        if (exists = Players[i].ModelEquals(Model.GameState.PlayerList[j]))
                            break;
                    if (!exists) Players.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.GameState.PlayerList.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Players.Count; j++)
                        if (exists = Players[j].ModelEquals(Model.GameState.PlayerList[i]))
                            break;
                    if (!exists) Players.Add(new PlayerViewModel(Model.GameState.PlayerList[i]));
                }
            }
            // MapList collection was re-set?
            else if (e.PropertyName == "MapList") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < Maps.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.GameState.MapList.Count; j++)
                        if (exists = Maps[i].ModelEquals(Model.GameState.MapList[j]))
                            break;
                    if (!exists) Maps.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.GameState.MapList.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Maps.Count; j++)
                        if (exists = Maps[j].ModelEquals(Model.GameState.MapList[i]))
                            break;
                    if (!exists) Maps.Add(new MapViewModel(Model.GameState.MapList[i]));
                }
            }
            // BanList collection was re-set?
            else if (e.PropertyName == "BanList") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < Bans.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.GameState.BanList.Count; j++)
                        if (exists = Bans[i].ModelEquals(Model.GameState.BanList[j]))
                            break;
                    if (!exists) Bans.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.GameState.BanList.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Bans.Count; j++)
                        if (exists = Bans[j].ModelEquals(Model.GameState.BanList[i]))
                            break;
                    if (!exists) Bans.Add(new BanViewModel(Model.GameState.BanList[i]));
                }
            }
            // GameModePool collection was re-set?
            else if (e.PropertyName == "GameModePool") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < GameModePool.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.GameState.GameModePool.Count; j++)
                        if (exists = GameModePool[i] == Model.GameState.GameModePool[j].FriendlyName)
                            break;
                    if (!exists) GameModePool.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.GameState.GameModePool.Count; i++) {
                    exists = false;
                    for (int j = 0; j < GameModePool.Count; j++)
                        if (exists = GameModePool[j] == Model.GameState.GameModePool[i].FriendlyName)
                            break;
                    if (!exists) GameModePool.Add(Model.GameState.GameModePool[i].FriendlyName);
                }
            }
            // MapPool collection was re-set?
            else if (e.PropertyName == "MapPool") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < MapPool.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.GameState.MapPool.Count; j++)
                        if (exists = MapPool[i].ModelEquals(Model.GameState.MapPool[j]))
                            break;
                    if (!exists) MapPool.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.GameState.MapPool.Count; i++) {
                    exists = false;
                    for (int j = 0; j < MapPool.Count; j++)
                        if (exists = MapPool[j].ModelEquals(Model.GameState.MapPool[i]))
                            break;
                    if (!exists) MapPool.Add(new MapViewModel(Model.GameState.MapPool[i]));
                }
            }
            // Other.
            else OnPropertyChanged(this, e.PropertyName);
        }
    }
}
