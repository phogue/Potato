using Procon.Net;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.API.Events
{
    public class ChatEvent : Event
    {
        #region Default (Normalized) keys used to access common event information.

        protected static readonly string C_TEAM   = "procon.Team";
        protected static readonly string C_SQUAD  = "procon.Squad";
        protected static readonly string C_PLAYER = "procon.Player";

        #endregion

        // Properties.
        public Team Team {
            get         { return TryGetVariable<Team>(C_TEAM, Team.None); }
            private set {
                if (Team != value) {
                    DataSet(C_TEAM, value);
                    OnPropertyChanged("Team");
        } } }
        public Squad Squad {
            get         { return TryGetVariable<Squad>(C_SQUAD, Squad.None); }
            private set {
                if (Squad != value) {
                    DataSet(C_SQUAD, value);
                    OnPropertyChanged("Squad");
        } } }
        public Player Player {
            get         { return TryGetVariable<Player>(C_PLAYER, null); }
            private set {
                if (Player != value) {
                    DataSet(C_PLAYER, value);
                    OnPropertyChanged("Player");
        } } }


        // Constructor.
        internal ChatEvent(GameEventArgs e) : base(e)
        {
            Team   = e.Chat.Subset.Team;
            Squad  = e.Chat.Subset.Squad;
            Player = e.Chat.Subset.Player;
        }
    }
}
