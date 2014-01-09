using System;

namespace Procon.Net.Shared {

    [Serializable]
    public enum ProtocolEventType {
        /// <summary>
        /// The game has had its definitions for gamemodes, maps etc. loaded.
        /// </summary>
        ProtocolConfigExecuted,
        /// <summary>
        /// Any server info/server settings have been updated.
        /// </summary>
        ProtocolSettingsUpdated,
        /// <summary>
        /// Playerlist information has been updated (scores/pings etc might have changed)
        /// </summary>
        ProtocolPlayerlistUpdated,
        /// <summary>
        /// The maplist has been updated - synched with server or new maplist, added maps etc.
        /// </summary>
        ProtocolMaplistUpdated,
        /// <summary>
        /// The banlist has been updated - synched with server or new banlist, added/removed bans
        /// </summary>
        ProtocolBanlistUpdated,
        /// <summary>
        /// A player has joined the game
        /// </summary>
        ProtocolPlayerJoin,
        /// <summary>
        /// A player has left the game
        /// </summary>
        ProtocolPlayerLeave,
        /// <summary>
        /// A player has been killed
        /// </summary>
        ProtocolPlayerKill,
        /// <summary>
        /// Chat has occured on the server (by procon, the server or a player)
        /// </summary>
        ProtocolChat,
        /// <summary>
        /// A player has spawned in
        /// </summary>
        ProtocolPlayerSpawn,
        /// <summary>
        /// A player has  been kicked
        /// </summary>
        ProtocolPlayerKicked,
        /// <summary>
        /// A player has moved to another team or has been moved to another team
        /// </summary>
        ProtocolPlayerMoved,
        /// <summary>
        /// A player has been banned
        /// </summary>
        ProtocolPlayerBanned,
        /// <summary>
        /// A player has been unbanned
        /// </summary>
        ProtocolPlayerUnbanned,
        /// <summary>
        /// The map has changed
        /// </summary>
        ProtocolMapChanged,
        /// <summary>
        /// The round has changed
        /// </summary>
        ProtocolRoundChanged,
    }
}
