using System;

using Procon.Net;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.API.Events
{
    public class Event : DataController
    {
        #region Default (Normalized) keys used to access common event information.

        protected static readonly string C_TYPE       = "procon.Type";
        protected static readonly string C_TIMESTAMP  = "procon.Timestamp";
        protected static readonly string C_EVENTSTAMP = "procon.Eventstamp";
        protected static readonly string C_NAME       = "procon.Name";
        protected static readonly string C_TEXT       = "procon.Text";

        #endregion

        // Properties.
        public GameEventType Type {
            get         { return TryGetVariable<GameEventType>(C_TYPE, GameEventType.Chat); }
            private set {
                if (Type != value) {
                    DataSet(C_TYPE, value);
                    OnPropertyChanged("Type");
        } } }
        public DateTime Timestamp {
            get         { return TryGetVariable<DateTime>(C_TIMESTAMP, DateTime.MinValue); }
            private set {
                if (Timestamp != value) {
                    DataSet(C_TIMESTAMP, value);
                    OnPropertyChanged("Timestamp");
        } } }
        public String Eventstamp {
            get         { return TryGetVariable<String>(C_EVENTSTAMP, null); }
            private set {
                if (Eventstamp != value) {
                    DataSet(C_EVENTSTAMP, value);
                    OnPropertyChanged("Eventstamp");
        } } }
        public String Name {
            get         { return TryGetVariable<String>(C_NAME, null); }
            private set {
                if (Name != value) {
                    DataSet(C_NAME, value);
                    OnPropertyChanged("Name");
        } } }
        public String Text {
            get         { return TryGetVariable<String>(C_TEXT, null); }
            private set {
                if (Text != value) {
                    DataSet(C_TEXT, value);
                    OnPropertyChanged("Text");
        } } }

        // Constructor.
        internal Event(GameEventArgs e)
        {
            Type       = e.EventType;
            Timestamp  = e.Stamp;
            Eventstamp = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.{0}",        Type));
            Text       = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Events.{0}", Type));
            switch (Type) {
                case GameEventType.PlayerJoin:
                case GameEventType.PlayerLeave:
                    Name = e.Player.Name;
                    break;

                case GameEventType.PlayerMoved:
                    Name = e.Player.Name;
                    Text = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Events.{0}", Type),
                        ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Team.{0}",  e.Player.Team)),
                        ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Squad.{0}", e.Player.Squad)));
                    break;

                case GameEventType.PlayerKicked:
                    Name = e.Kick.Target.Name;
                    break;

                case GameEventType.PlayerBanned:
                case GameEventType.PlayerUnbanned:
                    Name = e.Ban.Target.Name;
                    break;

                case GameEventType.PlayerKill:
                    Name = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Weapons.{0}", e.Kill.DamageType.Name));
                    Text = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Events.{0}",  Type),
                        (e.Kill.Killer != null) ? e.Kill.Killer.Name : ExtensionApi.Localize("Procon.UI.Root.Main.Connection.Chat.World"),
                        (e.Kill.Target != null) ? e.Kill.Target.Name : ExtensionApi.Localize("Procon.UI.Root.Main.Connection.Chat.Unknown"));
                    break;

                case GameEventType.PlayerSpawn:
                    Name = e.Spawn.Player.Name;
                    Text = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Events.{0}", Type),
                        e.Spawn.Role.Name);
                    break;

                case GameEventType.MapChanged:
                    Name = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Maps.{0}.{1}", e.GameType, e.GameState.Variables.MapName));
                    break;
                case GameEventType.RoundChanged:
                    Name = ExtensionApi.Localize(String.Format("Procon.UI.Root.Main.Connection.Chat.Round"),
                        e.GameState.Variables.RoundIndex);
                    break;

                case GameEventType.Chat:
                    Name = e.Chat.Author.Name;
                    Text = e.Chat.Text;
                    break;
            }
        }
    }
}
