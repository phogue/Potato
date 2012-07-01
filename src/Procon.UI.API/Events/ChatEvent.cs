using Procon.Net;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.API.Events
{
    public class ChatEvent : Event
    {
        // Properties.
        public Team   Team   { get; private set; }
        public Squad  Squad  { get; private set; }
        public Player Player { get; private set; }

        // Constructor.
        internal ChatEvent(GameEventArgs e) : base(e)
        {
            Team   = e.Chat.Subset.Team;
            Squad  = e.Chat.Subset.Squad;
            Player = e.Chat.Subset.Player;
        }
    }
}
