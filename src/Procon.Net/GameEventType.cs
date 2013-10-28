using System;

namespace Procon.Net {

    [Serializable]
    public enum GameEventType {
        /// <summary>
        /// The game has had its definitions for gamemodes, maps etc. loaded.
        /// </summary>
        GameConfigExecuted,
        /// <summary>
        /// Any server info/server settings have been updated.
        /// </summary>
        GameSettingsUpdated,
        /// <summary>
        /// Playerlist information has been updated (scores/pings etc might have changed)
        /// </summary>
        GamePlayerlistUpdated,
        /// <summary>
        /// The maplist has been updated - synched with server or new maplist, added maps etc.
        /// </summary>
        GameMaplistUpdated,
        /// <summary>
        /// The banlist has been updated - synched with server or new banlist, added/removed bans
        /// </summary>
        GameBanlistUpdated,
        /// <summary>
        /// A player has joined the game
        /// </summary>
        GamePlayerJoin,
        /// <summary>
        /// A player has left the game
        /// </summary>
        GamePlayerLeave,
        /// <summary>
        /// A player has been killed
        /// </summary>
        GamePlayerKill,
        /// <summary>
        /// Chat has occured on the server (by procon, the server or a player)
        /// </summary>
        GameChat,
        /// <summary>
        /// A player has spawned in
        /// </summary>
        GamePlayerSpawn,
        /// <summary>
        /// A player has  been kicked
        /// </summary>
        GamePlayerKicked,
        /// <summary>
        /// A player has moved to another team or has been moved to another team
        /// </summary>
        GamePlayerMoved,
        /// <summary>
        /// A player has been banned
        /// </summary>
        GamePlayerBanned,
        /// <summary>
        /// A player has been unbanned
        /// </summary>
        GamePlayerUnbanned,
        /// <summary>
        /// The map has changed
        /// </summary>
        GameMapChanged,
        /// <summary>
        /// The round has changed
        /// </summary>
        GameRoundChanged,
    }
}
