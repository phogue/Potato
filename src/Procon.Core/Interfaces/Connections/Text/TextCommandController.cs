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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Connections.Text {
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net.Protocols.Objects;
    using Procon.NLP;

    public abstract class TextCommandController : Executable<TextCommandController>, ICoreNLP {

        public List<TextCommand> TextCommands { get; protected set; }

        #region Events

        public delegate void TextCommandEventHandler(TextCommandController sender, TextCommandEventArgs e);
        public virtual event TextCommandEventHandler TextCommandEvent;

        #endregion

        public TextCommandController() {
            this.TextCommands = new List<TextCommand>();
        }

        #region Executable

        /// <summary>
        /// Begins the execution of this text command controller.
        /// Assigns events and loads the config for this file.
        /// </summary>
        public override TextCommandController Execute()
        {
            AssignEvents();
            return this;
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        internal override void WriteConfig(Config config) { }

        #endregion

        protected virtual void AssignEvents() {
            // this.TextCommands.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TextCommands_CollectionChanged);
        }

        private void TextCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (TextCommand item in e.NewItems) {
                    this.ThrowTextCommandEvent(
                        new TextCommandEventArgs() {
                            EventType = TextCommandEventType.Registered,
                            Command = item
                        }
                    );
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (TextCommand item in e.OldItems) {
                    this.ThrowTextCommandEvent(
                        new TextCommandEventArgs() {
                            EventType = TextCommandEventType.Unregistered,
                            Command = item
                        }
                    );
                }
            }
        }
        
        /// <summary>
        /// Just getting around the event being declared in the subclass.
        /// </summary>
        /// <param name="e"></param>
        protected void ThrowTextCommandEvent(TextCommandEventArgs e) {
            if (this.TextCommandEvent != null) {
                this.TextCommandEvent(
                    this,
                    e
                );
            }
        }

        public abstract Sentence Execute(GameState gameState, Player speaker, Account speakerAccount, string prefix, string sentence);


    }
}
