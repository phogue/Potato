using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections.TextCommands {
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Core.Interfaces.Layer;
    using Procon.Net.Protocols.Objects;
    using Procon.NLP;

    public abstract class TextCommandController : Executable<TextCommandController> {

        public List<TextCommand> TextCommands { get; protected set; }

        [JsonIgnore]
        public Connection Connection {
            get { return mConnection; }
            set {
                if (mConnection != value) {
                    mConnection = value;
                    OnPropertyChanged(this, "Connection");
                }
            }
        }
        private Connection mConnection;

        [JsonIgnore]
        public ILayer Layer {
            get { return mLayer; }
            set {
                if (mLayer != value) {
                    mLayer = value;
                    OnPropertyChanged(this, "Layer");
                }
            }
        }
        private ILayer mLayer;

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

        public abstract void ParseTextCommand(CommandInitiator initiator, String text);

        public abstract void PreviewTextCommand(CommandInitiator initiator, String text);

    }
}
