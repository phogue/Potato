// Copyright 2011 Geoffrey 'Phogue' Green
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Net;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;
    using Procon.Core.Scheduler;
    using Procon.Core.Interfaces.Connections.Text;
    using Procon.Core.Interfaces.Security.Objects;

    public class BasicInGameInfo : PluginAPI
    {

        //Ignore this: - Actually, depending on how you wanna save info for plugins, the actually plugin could save it here.
        // For now, it's in PluginAPI
        // 
        // protected override void WriteConfig(System.Xml.Linq.XElement xNamespace) { }
        // public override void Dispose() { }

        private List<TextCommand> Commands { get; set; }

        public BasicInGameInfo() : base() {
            this.Author = "Phogue";
            this.Website = "http://phogue.net";
            this.Description = "herro";

            this.Commands = new List<TextCommand>();
        }

        public override void PluginEvent(PluginEventArgs e) {
            base.PluginEvent(e);

            Console.WriteLine("Basic In Game Info Event: {0}", e.EventType);

            if (e.EventType == PluginEventType.CallbacksRegistered) {
                this.RegisterTextCommand(
                    new Text.TextCommand() {
                        MethodCallback = "TestCommand",
                        DescriptionKey = "TestCommandDescription",
                        Commands = new List<string>() {
                            "Test"
                        },
                    }
                );

                this.RegisterTextCommand(
                    new Text.TextCommand() {
                        MethodCallback = "HelpCommand",
                        Priority = PriorityType.High,
                        DescriptionKey = "HelpCommandDescription",
                        Commands = new List<string>() {
                            "Help"
                        },
                    }
                );
            }
        }

        [Command(CustomName = "MyCustamKommand")]
        public void SetupLocalInterface(CommandInitiator initiator, string ohhai, ushort times) {

            for (int x = 0; x < times; x++) {
                Console.WriteLine(ohhai);
            }

            
        }

        protected List<string> ShortCommandList(TextCommandEventArgs e) {
            return this.Commands.Select(x => x.Commands.FirstOrDefault()).ToList();
        }
        
        protected void HelpCommand(TextCommandEventArgs e) {

            if (e.AlternativeCommands.Count == 0) {
                // List of commands.
                this.Action(
                    new Chat() {
                        Text = this.PlayerLoc(e.Speaker, "HelpCommandHeader", e.Match.Prefix),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );

                foreach (string line in String.Join(", ", this.ShortCommandList(e).ToArray()).WordWrap(this.GameState.Variables.MaxConsoleLines))
                {
                    this.Action(
                        new Chat() {
                            Text = line,
                            Subset = new PlayerSubset() {
                                Context = PlayerSubsetContext.Player,
                                Player = e.Speaker
                            }
                        }
                    );
                }
            }
            else {
                foreach (TextCommand alternate in e.AlternativeCommands) {
                    string description = String.Format("> {0}: {1}", alternate.Commands.FirstOrDefault(),  this.NamespacePlayerLoc(e.Speaker, this.GetType().Namespace + "." + alternate.UidCallback, alternate.MethodCallback));

                    foreach (string line in description.WordWrap(this.GameState.Variables.MaxConsoleLines))
                    {
                        this.Action(
                            new Chat() {
                                Text = line,
                                Subset = new PlayerSubset() {
                                    Context = PlayerSubsetContext.Player,
                                    Player = e.Speaker
                                }
                            }
                        );
                    }
                }
            }
        }

        protected void TestCommand(TextCommandEventArgs e) {
            Console.WriteLine(e.Command.DescriptionKey);

            if (e.Match.Players != null && e.Match.Players.Count > 0) {
                this.Action(
                    new Chat() {
                        Text = "Players: " + String.Join(", ", e.Match.Players.Select(x => x.Name).ToArray()),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Maps != null && e.Match.Maps.Count > 0) {
                this.Action(
                    new Chat() {
                        Text = "Maps: " + String.Join(", ", e.Match.Maps.Select(x => x.Name).ToArray()),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Numeric != null && e.Match.Numeric.Count > 0) {
                this.Action(
                    new Chat() {
                        Text = "Numeric: " + String.Join(", ", e.Match.Numeric.Select(x => String.Format("{0:F2}", x)).ToArray()),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Delay != null) {
                this.Action(
                    new Chat() {
                        Text = String.Format("Delay: {0} (UTC+9:30 Adelaide)", e.Match.Delay),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Interval != null) {
                this.Action(
                    new Chat() {
                        Text = String.Format("Interval: {0}", e.Match.Interval),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Period != null) {
                this.Action(
                    new Chat() {
                        Text = String.Format("Duration: {0}", e.Match.Period),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.AlternativeCommands != null && e.AlternativeCommands.Count > 0) {
                this.Action(
                    new Chat() {
                        Text = "Alternate Commands: " + String.Join(" ", e.AlternativeCommands.Select(x => x.MethodCallback).ToArray()),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }

            if (e.Match.Quotes != null && e.Match.Quotes.Count > 0) {
                this.Action(
                    new Chat() {
                        Text = "Quotes: " + String.Join(", ", e.Match.Quotes.Select(x => String.Format("--{0}--", x)).ToArray()),
                        Subset = new PlayerSubset() {
                            Context = PlayerSubsetContext.Player,
                            Player = e.Speaker
                        }
                    }
                );
            }
        }

        public override void TextCommandEvent(TextCommandEventArgs e) {
            base.TextCommandEvent(e);

            if (e.EventType == TextCommandEventType.Registered) {
                this.Commands.Add(e.Command);
            }
            else if (e.EventType == TextCommandEventType.Unregistered) {
                this.Commands.Remove(e.Command);
            }
            else if (e.EventType == TextCommandEventType.Matched) {

                if (e.Command.MethodCallback == "HelpCommand") {
                    this.HelpCommand(e);
                }
                else if (e.Command.MethodCallback == "TestCommand") {
                    this.TestCommand(e);
                }
            }
        }
    }
}
