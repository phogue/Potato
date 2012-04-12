// Copyright 2011 Cameron 'Imisnew2' Gunnin
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;

using Procon.Net;
using Procon.Net.Protocols.Objects;
using Procon.UI.API.Enums;
using Procon.UI.API.Utils;

namespace Procon.UI.API.Objects
{
    /// <summary>Represents a specific event that has occurred within a game.</summary>
    public class Event
    {
        // Custom Properties
        public String Time        { get; set; }
        public String Type        { get; set; }
        public String Sender      { get; set; }
        public String Recipient   { get; set; }
        public String Information { get; set; }

        /// <summary>Creates an instance of Event and initalizes its properties.</summary>
        private Event()
        {
            // Defaults
            Time        = DateTime.Now.ToShortTimeString();
            Type        = String.Empty;
            Sender      = String.Empty;
            Recipient   = String.Empty;
            Information = String.Empty;
        }

        /// <summary>Allows only specific events to be created via a factory pattern.</summary>
        /// <param name="type">The type of event we want to create.</param>
        /// <param name="args">The arguments for the event to create.</param>
        /// <returns>The event we want to create.</returns>
        public static Event CreateEvent(EventType type, GameEventArgs args)
        {
            if (args != null)
                switch (type)
                {
                    case EventType.Join:
                        return CreateJoinedEvent(args);
                    case EventType.Leave:
                        return CreateLeftEvent(args);
                    case EventType.Move:
                        return CreateMovedEvent(args);
                    case EventType.Kick:
                        return CreateKickedEvent(args);
                    case EventType.Ban:
                        return CreateBannedEvent(args);
                    case EventType.Unban:
                        return CreateUnbannedEvent(args);
                    case EventType.Kill:
                        return CreateKilledEvent(args);
                    case EventType.Spawn:
                        return CreateSpawnedEvent(args);
                    case EventType.Chat:
                        return CreateChatEvent(args);
                    case EventType.Round:
                        return CreateRoundEndEvent(args);
                    case EventType.Map:
                        return CreateMapEndEvent(args);
                }
            return new Event();
        }

        /// <summary>Represents a "Player Joined" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Joined" event.</returns>
        private static Event CreateJoinedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time      = args.Stamp.ToShortTimeString(),
                Type      = Localizer.Loc("Procon.UI.API.Event." + EventType.Join),
                Recipient = args.Player.Name
            };
        }
        /// <summary>Represents a "Player Left" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Left" event.</returns>
        private static Event CreateLeftEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time      = args.Stamp.ToShortTimeString(),
                Type      = Localizer.Loc("Procon.UI.API.Event." + EventType.Leave),
                Recipient = args.Player.Name
            };
        }
        /// <summary>Represents a "Player Moved" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Moved" event.</returns>
        private static Event CreateMovedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time        = args.Stamp.ToShortTimeString(),
                Type        = Localizer.Loc("Procon.UI.API.Event." + EventType.Move),
                Recipient   = args.Player.Name,
                Information = String.Format("{0}: {1} > {2}",
                                  Localizer.Loc("Procon.UI.API.To"),
                                  Localizer.Loc("Procon.UI.API.Team." + args.Player.Team),
                                  Localizer.Loc("Procon.UI.API.Squad." + args.Player.Squad))
            };
        }
        /// <summary>Represents a "Player Kicked" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Kicked" event.</returns>
        private static Event CreateKickedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time        = args.Stamp.ToShortTimeString(),
                Type        = Localizer.Loc("Procon.UI.API.Event." + EventType.Kick),
                Recipient   = args.Kick.Target.Name,
                Information = args.Kick.Reason
            };
        }
        /// <summary>Represents a "Player Banned" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Banned" event.</returns>
        private static Event CreateBannedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time        = args.Stamp.ToShortTimeString(),
                Type        = Localizer.Loc("Procon.UI.API.Event." + EventType.Ban),
                Recipient   = (!String.IsNullOrEmpty(args.Ban.Target.Name)) ? 
                                  String.Format("{0}",
                                      args.Ban.Target.Name)
                              : (!String.IsNullOrEmpty(args.Ban.Target.GUID)) ? 
                                  String.Format("[GUID] {0}",
                                      args.Ban.Target.GUID)
                              : String.Empty,
                Information = String.Format("[{0}] {1}",
                                  Localizer.Loc("Procon.UI.API.Subset.Time." + args.Ban.Time.Context),
                                  args.Ban.Reason)
            };
        }
        /// <summary>Represents a "Player Unbanned" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Unbanned" event.</returns>
        private static Event CreateUnbannedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time      = args.Stamp.ToShortTimeString(),
                Type      = Localizer.Loc("Procon.UI.API.Event." + EventType.Unban),
                Recipient = (!String.IsNullOrEmpty(args.Ban.Target.Name)) ? 
                                  String.Format("{0}",
                                      args.Ban.Target.Name)
                              : (!String.IsNullOrEmpty(args.Ban.Target.GUID)) ? 
                                  String.Format("[GUID] {0}",
                                      args.Ban.Target.GUID)
                              : String.Empty
            };
        }
        /// <summary>Represents a "Player Killed" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Killed" event.</returns>
        private static Event CreateKilledEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time        = args.Stamp.ToShortTimeString(),
                Type        = Localizer.Loc("Procon.UI.API.Event." + EventType.Kill),
                Sender      = args.Kill.Killer != null ? args.Kill.Killer.Name : String.Empty,
                Recipient   = args.Kill.Target.Name,
                Information = args.Kill.Reason != null ? args.Kill.Reason : String.Empty
            };
        }
        /// <summary>Represents a "Player Spawned" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Spawned" event.</returns>
        private static Event CreateSpawnedEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time        = args.Stamp.ToShortTimeString(),
                Type        = Localizer.Loc("Procon.UI.API.Event." + EventType.Spawn),
                Recipient   = args.Spawn.Player.Name,
                Information = Localizer.Loc("Procon.UI.API.Role." + args.Spawn.Role.Name)
            };
        }
        /// <summary>Represents a "Player Chat" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Player Chat" event.</returns>
        private static Event CreateChatEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time      = args.Stamp.ToShortTimeString(),
                Type      = Localizer.Loc("Procon.UI.API.Event." + EventType.Chat),
                Sender    = args.Chat.Author.Name,
                Recipient = (args.Chat.Subset.Context == PlayerSubsetContext.All) ?
                                String.Format("{0}",
                                    Localizer.Loc("Procon.UI.API.Subset.Player.All"))
                            : (args.Chat.Subset.Context == PlayerSubsetContext.Team) ?
                                String.Format("{0} : {1}",
                                    Localizer.Loc("Procon.UI.API.Subset.Player.Team"),
                                    Localizer.Loc("Procon.UI.API.Teams." + args.Chat.Subset.Team))
                            : (args.Chat.Subset.Context == PlayerSubsetContext.Squad) ?
                                String.Format("{0} : {1} > {2}",
                                    Localizer.Loc("Procon.UI.API.Subset.Player.Squad"),
                                    Localizer.Loc("Procon.UI.API.Teams." + args.Chat.Subset.Team),
                                    Localizer.Loc("Procon.UI.API.Squads." + args.Chat.Subset.Squad))
                            : (args.Chat.Subset.Context == PlayerSubsetContext.Player) ?
                                String.Format("{0} : {1}",
                                    Localizer.Loc("Procon.UI.API.Subset.Player.Player"),
                                    args.Chat.Subset.Player != null ? args.Chat.Subset.Player.Name : String.Empty)
                            : String.Empty,
                Information = args.Chat.Text
            };
        }
        /// <summary>Represents a "Round Ended" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Round Ended" event.</returns>
        private static Event CreateRoundEndEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time = args.Stamp.ToShortTimeString(),
                Type = Localizer.Loc("Procon.UI.API.Event." + EventType.Round)
            };
        }
        /// <summary>Represents a "Map Ended" event.</summary>
        /// <param name="args">The arguments used to populate the event's information.</param>
        /// <returns>A "Map Ended" event.</returns>
        private static Event CreateMapEndEvent(GameEventArgs args)
        {
            return new Event()
            {
                Time = args.Stamp.ToShortTimeString(),
                Type = Localizer.Loc("Procon.UI.API.Event." + EventType.Map)
            };
        }
    }
}
