using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Connections.Plugins;
using Procon.Net;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.API.ViewModels
{
    public class ConnectionViewModel : ViewModel<Connection>
    {
        // Properties.
        public GameType        GameType {
            get { return nModel.GameType; }
        }
        public String          Hostname {
            get { return nModel.Hostname; }
        }
        public UInt16          Port {
            get { return nModel.Port; }
        }
        public String          Additional {
            get { return nModel.Additional; }
        }
        public ConnectionState ConnectionState {
            get { return nModel.GameState.Variables.ConnectionState; }
        }

        public Int32   MaxConsoleLines {
            get { return nModel.GameState.Variables.MaxConsoleLines; }
            set { nModel.GameState.Variables.MaxConsoleLines = value; }
        }
        public Int32   UpTime {
            get { return nModel.GameState.Variables.UpTime; }
        }
        public String  Version {
            get { return nModel.GameState.Variables.Version; }
        }
        public String  ServerName {
            get { return nModel.GameState.Variables.ServerName; }
            set { nModel.GameState.Variables.ServerName = value; }
        }
        public String  ServerDescription {
            get { return nModel.GameState.Variables.ServerDescription; }
            set { nModel.GameState.Variables.ServerDescription = value; }
        }
        public String  BannerUrl {
            get { return nModel.GameState.Variables.BannerUrl; }
            set { nModel.GameState.Variables.BannerUrl = value; }
        }
        public Boolean Ranked {
            get { return nModel.GameState.Variables.Ranked; }
            set { nModel.GameState.Variables.Ranked = value; }
        }
        public Boolean AntiCheat {
            get { return nModel.GameState.Variables.AntiCheat; }
            set { nModel.GameState.Variables.AntiCheat = value; }
        }
        public Boolean AutoBalance {
            get { return nModel.GameState.Variables.AutoBalance; }
            set { nModel.GameState.Variables.AutoBalance = value; }
        }
        public Boolean FriendlyFire {
            get { return nModel.GameState.Variables.FriendlyFire; }
            set { nModel.GameState.Variables.FriendlyFire = value; }
        }
        public Boolean Passworded {
            get { return nModel.GameState.Variables.Passworded; }
            set { nModel.GameState.Variables.Passworded = value; }
        }
        public String  Password {
            get { return nModel.GameState.Variables.Password; }
            set { nModel.GameState.Variables.Password = value; }
        }
        public Int32   RoundTime {
            get { return nModel.GameState.Variables.RoundTime; }
        }
        public Int32   RoundIndex {
            get { return nModel.GameState.Variables.RoundIndex; }
        }
        public Int32   MaxRoundIndex {
            get { return nModel.GameState.Variables.MaxRoundIndex; }
            set { nModel.GameState.Variables.MaxRoundIndex = value; }
        }
        public Int32   PlayerCount {
            get { return nModel.GameState.Variables.PlayerCount; }
        }
        public Int32   MaxPlayerCount {
            get { return nModel.GameState.Variables.MaxPlayerCount; }
            set { nModel.GameState.Variables.MaxPlayerCount = value; }
        }
        public String  MapName {
            get { return nModel.GameState.Variables.MapName; }
        }
        public String  GameModeName {
            get { return nModel.GameState.Variables.GameModeName; }
        }

        // Observable Properties.
        public ObservableCollection<Plugin>         Plugins {
            get { return mPlugins; }
            set {
                if (mPlugins != value) {
                    mPlugins = value;
                    OnPropertyChanged("Plugins");
        } } }
        public ObservableCollection<Player>         Players {
            get { return mPlayers; }
            set {
                if (mPlayers != value) {
                    mPlayers = value;
                    OnPropertyChanged("Players");
        } } }
        public ObservableCollection<Map>            Maps {
            get { return mMaps; }
            set {
                if (mMaps != value) {
                    mMaps = value;
                    OnPropertyChanged("MapList");
        } } }
        public ObservableCollection<Ban>            Bans
        {
            get { return mBans; }
            set {
                if (mBans != value) {
                    mBans = value;
                    OnPropertyChanged("Bans");
        } } }
        public ObservableCollection<GameMode>       GameModePool {
            get { return mGameModePool; }
            set {
                if (mGameModePool != value) {
                    mGameModePool = value;
                    OnPropertyChanged("GameModePool");
        } } }
        public ObservableCollection<Map>            MapPool {
            get { return mMapPool; }
            set {
                if (mMapPool != value) {
                    mMapPool = value;
                    OnPropertyChanged("MapPool");
        } } }
        public ObservableCollection<DataVariable>   Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged("Variables");
        } } }
        public ObservableCollection<ProtocolObject> Events {
            get { return mEvents; }
            set {
                if (mEvents != value) {
                    mEvents = value;
                    OnPropertyChanged("Events");
        } } }
        
        private ObservableCollection<Plugin>         mPlugins;
        private ObservableCollection<Player>         mPlayers;
        private ObservableCollection<Map>            mMaps;
        private ObservableCollection<Ban>            mBans;
        private ObservableCollection<GameMode>       mGameModePool;
        private ObservableCollection<Map>            mMapPool;
        private ObservableCollection<DataVariable>   mVariables;
        private ObservableCollection<ProtocolObject> mEvents;


        // Constructor.
        public ConnectionViewModel(Connection model) : base(model)
        {
            // Listen for changes within the model:
            nModel.Plugins.PluginAdded             += Plugins_Added;
            nModel.Plugins.PluginRemoved           += Plugins_Removed;
            nModel.GameState.Variables.DataAdded   += Variables_DataAdded;
            nModel.GameState.Variables.DataRemoved += Variables_DataRemoved;
            nModel.GameState.Variables.DataChanged += Variables_DataChanged;
            nModel.PropertyChanged                 += Connection_PropertyChanged;
            nModel.GameEvent                       += GameEventOccurred;

            // Expose collections within the model:
            Plugins      = new ObservableCollection<Plugin>(nModel.Plugins.Plugins);
            Players      = new ObservableCollection<Player>(nModel.GameState.PlayerList);
            Maps         = new ObservableCollection<Map>(nModel.GameState.MapList);
            Bans         = new ObservableCollection<Ban>(nModel.GameState.BanList);
            GameModePool = new ObservableCollection<GameMode>(nModel.GameState.GameModePool);
            MapPool      = new ObservableCollection<Map>(nModel.GameState.MapPool);
            Variables    = new ObservableCollection<DataVariable>(nModel.GameState.Variables.Variables.Where(x => !x.IsReadOnly).OrderBy(x => x.Name));
            Events       = new ObservableCollection<ProtocolObject>();
        }


        // View Model Methods.
        public void Action(ProtocolObject action)
        {
            nModel.Action(action);
        }


        // Wraps the Added & Removed events.
        private void Plugins_Added(PluginController parent, Plugin item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Plugins_Added(parent, item)))
                return;

            // Add the new plugin.
            Plugins.Add(item);
        }
        private void Plugins_Removed(PluginController parent, Plugin item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Plugins_Removed(parent, item)))
                return;

            // Remove the old plugin.
            Plugins.Add(item);
        }
        private void Variables_DataAdded(DataController parent, DataVariable item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_DataAdded(parent, item)))
                return;

            // Add the new variable.
            if (!item.IsReadOnly) {
                DataVariable tVariable = Variables.SingleOrDefault(x => item.Name.CompareTo(x.Name) < 0);
                if (tVariable != null) Variables.Insert(Variables.IndexOf(tVariable), item);
                else                   Variables.Add(item);
            }
        }
        private void Variables_DataRemoved(DataController parent, DataVariable item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_DataRemoved(parent, item)))
                return;

            // Remove the old variable.
            Variables.Remove(item);
        }
        private void Variables_DataChanged(DataController parent, DataVariable item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Variables_DataChanged(parent, item)))
                return;

            // Notify that the variable changed.
            if (item.Name.Contains('.') && item.Name.IndexOf('.') + 1 < item.Name.Length)
                OnPropertyChanged(item.Name.Substring(item.Name.IndexOf('.') + 1));
            else 
                OnPropertyChanged(item.Name);
        }
        // Wraps the Connection's property changed events.
        private void Connection_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connection_PropertyChanged(sender, e)))
                return;

            OnPropertyChanged(e.PropertyName);
        }


        // Synchronizes the view model's information with the game.
        private void GameEventOccurred(Game sender, GameEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => GameEventOccurred(sender, e)))
                return;

            switch (e.EventType)
            {
                /* Player Joined Event:
                 * 
                 * [Players Collection]
                 * Updates the player list with the new player, if he is not in 
                 * the list already. */
                case GameEventType.PlayerJoin:
                    if (!Players.Contains(e.Player))
                        Players.Add(e.Player);
                    break;

                /* Player Left Event:
                 * 
                 * [Players Collection]
                 * Removes the player from the player list, if he/she is in the
                 * player list. */
                case GameEventType.PlayerLeave:
                    if (Players.Contains(e.Player))
                        Players.Remove(e.Player);
                    break;

                /* Player Moved Event: */
                case GameEventType.PlayerMoved:
                    break;

                /* Player List Updated Event:
                 * 
                 * [Players Collection]
                 * Removes the entries that no longer exist and adds entries
                 * that are new. This was performed instead of a "Clear" and
                 * "Add" in an attempt to remedy selection issues. */
                case GameEventType.PlayerlistUpdated:
                    // Remove Old Items.
                    for (int i = 0; i < Players.Count; i++)
                        if (!e.GameState.PlayerList.Contains(Players[i]))
                            Players.RemoveAt(i--);
                    // Add New Items.
                    for (int i = 0; i < e.GameState.PlayerList.Count; i++)
                        if (!Players.Contains(e.GameState.PlayerList[i]))
                            Players.Add(e.GameState.PlayerList[i]);
                    break;





                /* Player Kicked Event: */
                case GameEventType.PlayerKicked:
                    break;

                /* Player Banned Event:
                 * 
                 * [Bans Collection]
                 * Updates the ban list with the new ban, if it is not in 
                 * the list already. */
                case GameEventType.PlayerBanned:
                    if (!Bans.Contains(e.Ban))
                        Bans.Add(e.Ban);
                    break;

                /* Player Unbanned Event:
                 * 
                 * [Bans Collection]
                 * Removes the ban from the player list, if it is in the
                 * ban list. */
                case GameEventType.PlayerUnbanned:
                    if (Bans.Contains(e.Ban))
                        Bans.Remove(e.Ban);
                    break;

                /* Ban List Updated Event:
                 * 
                 * [Bans Collection]
                 * Removes the entries that no longer exist and adds entries
                 * that are new. This was performed instead of a "Clear" and
                 * "Add" in an attempt to remedy selection issues. */
                case GameEventType.BanlistUpdated:
                    // Remove Old Items.
                    for (int i = 0; i < Bans.Count; i++)
                        if (!e.GameState.BanList.Contains(Bans[i]))
                            Bans.RemoveAt(i--);
                    // Add New Items.
                    for (int i = 0; i < e.GameState.BanList.Count; i++)
                        if (!Bans.Contains(e.GameState.BanList[i]))
                            Bans.Add(e.GameState.BanList[i]);
                    break;





                /* Player Killed Event: */
                case GameEventType.PlayerKill:
                    break;

                /* Player Spawned Event: */
                case GameEventType.PlayerSpawn:
                    break;

                /* Player Chat Event: */
                case GameEventType.Chat:
                    if (e.Chat.Origin != ChatOrigin.Reflected)
                        Events.Add(e.Chat);
                    break;

                /* Round Changed Event: */
                case GameEventType.RoundChanged:
                    break;

                /* Map Changed Event: */
                case GameEventType.MapChanged:
                    break;

                /* Map List Updated Event:
                 * 
                 * [Maps Collection]
                 * Removes the entries that no longer exist and adds entries
                 * that are new. This was performed instead of a "Clear" and
                 * "Add" in an attempt to remedy selection issues. */
                case GameEventType.MaplistUpdated:
                    // Remove Old Items.
                    for (int i = 0; i < Maps.Count; i++)
                        if (!e.GameState.MapList.Contains(Maps[i]))
                            Maps.RemoveAt(i--);
                    // Add New Items.
                    for (int i = 0; i < e.GameState.MapList.Count; i++)
                        if (!Maps.Contains(e.GameState.MapList[i]))
                            Maps.Add(e.GameState.MapList[i]);
                    break;





                /* Server Info Updated Event: */
                case GameEventType.ServerInfoUpdated:
                    break;

                /* Game Config Executed Event: */
                case GameEventType.GameConfigExecuted:
                    break;
            }
        }
    }
}
