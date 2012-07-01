using System;
using System.Collections.Generic;
using Procon.Net;

namespace Procon.UI.API.Events
{
    public static class EventBuilder
    {
        // Supported event types.
        public static List<GameEventType> SUPPORTED_TYPES = new List<GameEventType>() {
            GameEventType.PlayerJoin,   GameEventType.PlayerLeave,  GameEventType.PlayerMoved,
            GameEventType.PlayerKicked, GameEventType.PlayerBanned, GameEventType.PlayerUnbanned,
            GameEventType.PlayerKill,   GameEventType.PlayerSpawn,
            GameEventType.MapChanged,   GameEventType.RoundChanged,
            GameEventType.Chat
        };

        // Builds events based on their event type.
        public static Event CreateEvent(GameEventArgs e)
        {
            if (!SUPPORTED_TYPES.Contains(e.EventType))
                throw new NotSupportedException(String.Format("\"{0}\" Event Type is not supported.", e.EventType));

            if (e.EventType == GameEventType.Chat)
                return new ChatEvent(e);
            return new Event(e);
        }
    }
}
