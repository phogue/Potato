using System;

namespace Procon.Net.Models {
    /// <summary>
    /// Note that the property names will more than likely lengthen to very specific for their task.
    /// 
    /// We originally had a very good implementation by Cameron to handle generic additions by protocols to
    /// this list of settings, however they kind of broke the abstraction because we would then need to know
    /// what game was running and what additional variables were in this game. It could also mean clashes 
    /// between implementations of games that have identical variable names, but subtle or drastic differences
    /// in the value.
    /// 
    /// This class will more than likely blow out with very specific variable names - this is okay, this is almost
    /// perfect in fact. It means that a variable named "GameServerUpTimeSeconds" will always store the up time of
    /// the game server in a unit of seconds. A plugin seeking this information would always know where to look
    /// and various protocol implementations can reuse this variable along with any unit conversions required.
    /// 
    /// In all, I expect this class will have several hundred property names and if it means that conversion needs
    /// to be done in one location (protocol implementation) rather than within a plugin then it'll all be worth it.
    /// </summary>
    [Serializable]
    public sealed class SettingsData {

        /// <summary>
        /// The maxmimum amount of lines able to be entered into the chat (4 in bf4 for instance)
        /// </summary>
        public int? ChatLinesCount { get; set; }

        /// <summary>
        /// The maximum number of characters that can appear in the chat console before being wrapped or cut off.
        /// </summary>
        public int? ChatColumnCount { get; set; }

        /// <summary>
        /// The current state of the connection between the game server and rcon port
        /// </summary>
        public ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// The amount of time the server has been running since it's last restart
        /// </summary>
        public int? UpTimeMilliseconds { get; set; }

        /// <summary>
        /// The version of the server
        /// </summary>
        public String ServerVersionText { get; set; }

        /// <summary>
        /// The visible name of the server
        /// </summary>
        public String ServerNameText { get; set; }

        /// <summary>The description of the server
        /// </summary>
        public String ServerDescriptionText { get; set; }

        /// <summary>
        /// The url for the banner of the server
        /// </summary>
        public String ServerBannerUrlText { get; set; }

        /// <summary>
        /// The maximum rank a player can be.
        /// </summary>
        public int? PlayerRank { get; set; }

        /// <summary>
        /// Whether the server is hard core
        /// </summary>
        public bool? HardcoreEnabled { get; set; }

        /// <summary>
        /// Whether the server is protected by anti-cheat software
        /// </summary>
        public bool? AntiCheatEnabled { get; set; }

        /// <summary>
        /// Whether the server is auto-balanced
        /// </summary>
        public bool? AutoBalanceEnabled { get; set; }

        /// <summary>
        /// Whether the server has a profanity filter enabled or not.
        /// </summary>
        public bool? ProfanityFilterEnabled { get; set; }

        /// <summary>
        /// Whether the player has a cross hair or not.
        /// </summary>
        public bool? CrossHairEnabled { get; set; }

        /// <summary>
        /// Whether the mini map is enabled for players.
        /// </summary>
        public bool? MiniMapEnabled { get; set; }

        /// <summary>
        /// Whether the kill camera is enabled
        /// </summary>
        public bool? KillCameraEnabled { get; set; }

        /// <summary>
        /// Whether the server has friendly-fire enabled
        /// </summary>
        public bool? FriendlyFireEnabled { get; set; }

        /// <summary>
        /// Whether the server is password protected
        /// </summary>
        public bool? PasswordProtectionEnabled { get; set; }

        /// <summary>
        /// The password of the server, if password protected
        /// </summary>
        public String PasswordText { get; set; }

        /// <summary>
        /// How long the current round has been played
        /// </summary>
        public int? RoundTimeMilliseconds { get; set; }

        /// <summary>
        /// if the idle timeout is enabled or not.
        /// </summary>
        public bool? IdleTimeoutEnabled { get; set; }

        /// <summary>
        /// How long the player can idle before they are kicked out.
        /// </summary>
        public int? IdleTimeoutMilliseconds { get; set; }

        /// <summary>
        /// The current round index being played
        /// </summary>
        public int? RoundIndex { get; set; }

        /// <summary>
        /// How many players are currently playing in the server
        /// </summary>
        public int? PlayerCount { get; set; }

        /// <summary>
        /// The name of the current map
        /// </summary>
        public String MapNameText { get; set; }

        /// <summary>
        /// The name of the current game mode
        /// </summary>
        public String GameModeNameText { get; set; }

        /// <summary>
        /// The name of any mod being run
        /// </summary>
        public String ModNameText { get; set; }
    }
}
