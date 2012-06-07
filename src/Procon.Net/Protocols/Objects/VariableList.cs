using System;
using System.ComponentModel;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class VariableList : DataController
    {
        #region Default (Normalized) Keys used to access common game server information.

        protected static readonly string C_MAX_CONSOLE_LINES  = "procon.MaxConsoleLines";
        protected static readonly string C_CONNECTION_STATE   = "procon.ConnectionState";
        protected static readonly string C_UP_TIME            = "procon.UpTime";
        protected static readonly string C_VERSION            = "procon.Version";
        protected static readonly string C_SERVER_NAME        = "procon.ServerName";
        protected static readonly string C_SERVER_DESCRIPTION = "procon.ServerDescription";
        protected static readonly string C_BANNER_URL         = "procon.BannerUrl";
        protected static readonly string C_RANKED             = "procon.Ranked";
        protected static readonly string C_ANTI_CHEAT         = "procon.AntiCheat";
        protected static readonly string C_AUTO_BALANCE       = "procon.AutoBalance";
        protected static readonly string C_FRIENDLY_FIRE      = "procon.FriendlyFire";
        protected static readonly string C_PASSWORDED         = "procon.Passworded";
        protected static readonly string C_PASSWORD           = "procon.Password";
        protected static readonly string C_ROUND_TIME         = "procon.RoundTime";
        protected static readonly string C_ROUND_INDEX        = "procon.RoundIndex";
        protected static readonly string C_MAX_ROUND_INDEX    = "procon.MaxRoundIndex";
        protected static readonly string C_PLAYER_COUNT       = "procon.PlayerCount";
        protected static readonly string C_MAX_PLAYER_COUNT   = "procon.MaxPlayerCount";
        protected static readonly string C_MAP_NAME           = "procon.MapName";
        protected static readonly string C_GAME_MODE_NAME     = "procon.GameModeName";

        #endregion

        public VariableList() {
            this[C_MAX_CONSOLE_LINES]  = new DataVariable(C_MAX_CONSOLE_LINES,  null, false, "Variables.MAX_CONSOLE_LINES",  "Variables.MAX_CONSOLE_LINES_Description");
            this[C_CONNECTION_STATE]   = new DataVariable(C_CONNECTION_STATE,   null, true,  "Variables.CONNECTION_STATE",   "Variables.CONNECTION_STATE_Description");
            this[C_UP_TIME]            = new DataVariable(C_UP_TIME,            null, true,  "Variables.UP_TIME",            "Variables.UP_TIME_Description");
            this[C_VERSION]            = new DataVariable(C_VERSION,            null, true,  "Variables.VERSION",            "Variables.VERSION_Description");
            this[C_SERVER_NAME]        = new DataVariable(C_SERVER_NAME,        null, false, "Variables.SERVER_NAME",        "Variables.SERVER_NAME_Description");
            this[C_SERVER_DESCRIPTION] = new DataVariable(C_SERVER_DESCRIPTION, null, false, "Variables.SERVER_DESCRIPTION", "Variables.SERVER_DESCRIPTION_Description");
            this[C_BANNER_URL]         = new DataVariable(C_BANNER_URL,         null, false, "Variables.BANNER_URL",         "Variables.BANNER_URL_Description");
            this[C_RANKED]             = new DataVariable(C_RANKED,             null, false, "Variables.RANKED",             "Variables.RANKED_Description");
            this[C_ANTI_CHEAT]         = new DataVariable(C_ANTI_CHEAT,         null, false, "Variables.ANTI_CHEAT",         "Variables.ANTI_CHEAT_Description");
            this[C_AUTO_BALANCE]       = new DataVariable(C_AUTO_BALANCE,       null, false, "Variables.AUTO_BALANCE",       "Variables.AUTO_BALANCE_Description");
            this[C_FRIENDLY_FIRE]      = new DataVariable(C_FRIENDLY_FIRE,      null, false, "Variables.FRIENDLY_FIRE",      "Variables.FRIENDLY_FIRE_Description");
            this[C_PASSWORDED]         = new DataVariable(C_PASSWORDED,         null, false, "Variables.PASSWORDED",         "Variables.PASSWORDED_Description");
            this[C_PASSWORD]           = new DataVariable(C_PASSWORD,           null, false, "Variables.PASSWORD",           "Variables.PASSWORD_Description");
            this[C_ROUND_TIME]         = new DataVariable(C_ROUND_TIME,         null, true,  "Variables.ROUND_TIME",         "Variables.ROUND_TIME_Description");
            this[C_ROUND_INDEX]        = new DataVariable(C_ROUND_INDEX,        null, true,  "Variables.ROUND_INDEX",        "Variables.ROUND_INDEX_Description");
            this[C_MAX_ROUND_INDEX]    = new DataVariable(C_MAX_ROUND_INDEX,    null, true,  "Variables.MAX_ROUND_INDEX",    "Variables.MAX_ROUND_INDEX_Description");
            this[C_PLAYER_COUNT]       = new DataVariable(C_PLAYER_COUNT,       null, true,  "Variables.PLAYER_COUNT",       "Variables.PLAYER_COUNT_Description");
            this[C_MAX_PLAYER_COUNT]   = new DataVariable(C_MAX_PLAYER_COUNT,   null, false, "Variables.MAX_PLAYER_COUNT",   "Variables.MAX_PLAYER_COUNT_Description");
            this[C_MAP_NAME]           = new DataVariable(C_MAP_NAME,           null, true,  "Variables.MAP_NAME",           "Variables.MAP_NAME_Description");
            this[C_GAME_MODE_NAME]     = new DataVariable(C_GAME_MODE_NAME,     null, true,  "Variables.GAME_MODE_NAME",     "Variables.GAME_MODE_NAME_Description");
        }

        /// <summary>The maxmimum amount of lines able to be entered into the console</summary>
        public virtual int MaxConsoleLines
        {
            get { return TryGetVariable<int>(C_MAX_CONSOLE_LINES, 0); }
            set {
                DataAddSet(C_MAX_CONSOLE_LINES, value);
                OnPropertyChanged("MaxConsoleLines");
        } }

        /// <summary>The current state of the connection between the game server and rcon port</summary>
        public virtual ConnectionState ConnectionState
        {
            get { return TryGetVariable<ConnectionState>(C_CONNECTION_STATE, 0); }
            set {
                DataAddSet(C_CONNECTION_STATE, value);
                OnPropertyChanged("ConnectionState");
        } }
        /// <summary>The amount of time the server has been running since it's last restart</summary>
        public virtual int UpTime
        {
            get { return TryGetVariable<int>(C_UP_TIME, 0); }
            set {
                DataAddSet(C_UP_TIME, value);
                OnPropertyChanged("UpTime");
        } }
        /// <summary>The version of the server</summary>
        public virtual string Version
        {
            get { return TryGetVariable<string>(C_VERSION, null); }
            set {
                DataAddSet(C_VERSION, value);
                OnPropertyChanged("Version");
        } }

        /// <summary>The visible name of the server</summary>
        public virtual string ServerName
        {
            get { return TryGetVariable<string>(C_SERVER_NAME, null); }
            set {
                DataAddSet(C_SERVER_NAME, value);
                OnPropertyChanged("ServerName");
        } }
        /// <summary>The description of the server</summary>
        public virtual string ServerDescription
        {
            get { return TryGetVariable<string>(C_SERVER_DESCRIPTION, null); }
            set {
                DataAddSet(C_SERVER_DESCRIPTION, value);
                OnPropertyChanged("ServerDescription");
        } }
        /// <summary>The url for the banner of the server</summary>
        public virtual string BannerUrl
        {
            get { return TryGetVariable<string>(C_BANNER_URL, null); }
            set {
                DataAddSet(C_BANNER_URL, value);
                OnPropertyChanged("BannerUrl");
        } }
        
        /// <summary>Whether the server is ranked</summary>
        public virtual bool Ranked
        {
            get { return TryGetVariable<bool>(C_RANKED, false); }
            set {
                DataAddSet(C_RANKED, value);
                OnPropertyChanged("Ranked");
        } }
        /// <summary>Whether the server is protected by anti-cheat software</summary>
        public virtual bool AntiCheat
        {
            get { return TryGetVariable<bool>(C_ANTI_CHEAT, false); }
            set {
                DataAddSet(C_ANTI_CHEAT, value);
                OnPropertyChanged("AntiCheat");
        } }
        /// <summary>Whether the server is auto-balanced</summary>
        public virtual bool AutoBalance
        {
            get { return TryGetVariable<bool>(C_AUTO_BALANCE, false); }
            set {
                DataAddSet(C_AUTO_BALANCE, value);
                OnPropertyChanged("AutoBalance");
        } }
        /// <summary>Whether the server has friendly-fire enabled</summary>
        public virtual bool FriendlyFire
        {
            get { return TryGetVariable<bool>(C_FRIENDLY_FIRE, false); }
            set {
                DataAddSet(C_FRIENDLY_FIRE, value);
                OnPropertyChanged("FriendlyFire");
        } }
        /// <summary>Whether the server is password protected</summary>
        public virtual bool Passworded
        {
            get { return TryGetVariable<bool>(C_PASSWORDED, false); }
            set {
                DataAddSet(C_PASSWORDED, value);
                OnPropertyChanged("Passworded");
        } }
        /// <summary>The password of the server, if password protected</summary>
        public virtual string Password
        {
            get { return TryGetVariable<string>(C_PASSWORD, null); }
            set {
                DataAddSet(C_PASSWORD, value);
                OnPropertyChanged("Password");
        } }

        /// <summary>How long the current round has been played</summary>
        public virtual int RoundTime
        {
            get { return TryGetVariable<int>(C_ROUND_TIME, 0); }
            set {
                DataAddSet(C_ROUND_TIME, value);
                OnPropertyChanged("RoundTime");
        } }
        /// <summary>The current round index being played</summary>
        public virtual int RoundIndex
        {
            get { return TryGetVariable<int>(C_ROUND_INDEX, 0); }
            set {
                DataAddSet(C_ROUND_INDEX, value);
                OnPropertyChanged("RoundIndex");
        } }
        /// <summary>The maximum number of rounds to be played</summary>
        public virtual int MaxRoundIndex
        {
            get { return TryGetVariable<int>(C_MAX_ROUND_INDEX, 0); }
            set {
                DataAddSet(C_MAX_ROUND_INDEX, value);
                OnPropertyChanged("MaxRoundIndex");
        } }
        
        /// <summary>How many players are currently playing in the server</summary>
        public virtual int PlayerCount
        {
            get { return TryGetVariable<int>(C_PLAYER_COUNT, 0); }
            set
            {
                DataAddSet(C_PLAYER_COUNT, value);
                OnPropertyChanged("PlayerCount");
        } }
        /// <summary>How many players are supported by the server</summary>
        public virtual int MaxPlayerCount
        {
            get { return TryGetVariable<int>(C_MAX_PLAYER_COUNT, 0); }
            set {
                DataAddSet(C_MAX_PLAYER_COUNT, value);
                OnPropertyChanged("MaxPlayerCount");
        } }

        /// <summary>The name of the current map</summary>
        public virtual string MapName
        {
            get { return TryGetVariable<string>(C_MAP_NAME, null); }
            set {
                DataAddSet(C_MAP_NAME, value);
                OnPropertyChanged("MapName");
        } }
        /// <summary>The name of the current game mode</summary>
        public virtual string GameModeName
        {
            get { return TryGetVariable<string>(C_GAME_MODE_NAME, null); }
            set {
                DataAddSet(C_GAME_MODE_NAME, value);
                OnPropertyChanged("GameModeName");
        } }
    }
}