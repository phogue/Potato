using System;

using Procon.Net;

namespace Procon.UI.API.Events
{
    public class Event
    {
        // Properties.
        public GameEventType Type       { get; private set; }
        public DateTime      Timestamp  { get; private set; }
        public String        Eventstamp { get; private set; }
        public String        Name       { get; private set; }
        public String        Text       { get; private set; }

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
