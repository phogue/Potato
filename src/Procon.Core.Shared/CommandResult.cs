using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Shared {

    /// <summary>
    /// I'd be tempted to refactor GenericEventArgs so I can seal this class. It's
    /// used as a backbone for xml serialization so any inherited classes could
    /// cause the xml serializer to encounter a type it didn't expect.
    /// </summary>
    [Serializable]
    public class CommandResult : IDisposable {

        public static CommandResult InsufficientPermissions = new CommandResult() {
            Success = false,
            Status = CommandResultType.InsufficientPermissions,
            Message = "You have Insufficient Permissions to execute this command."
        };

        /// <summary>
        /// A general text message used to describe the event in more detail, if required.
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// The limiting scope of the event (the connection, player etc. that this event is limited to)
        /// </summary>
        public CommandData Scope { get; set; }

        /// <summary>
        /// Data that this used to be, like an account being moved from one group to another
        /// this would be the original group.
        /// </summary>
        public CommandData Then { get; set; }

        /// <summary>
        /// Data as it is seen "now"
        /// </summary>
        public CommandData Now { get; set; }

        /// <summary>
        /// Simple flag determining the success of the command being executed.
        /// </summary>
        public Boolean Success { get; set; }

        /// <summary>
        /// A more detailed status describing the command execution.
        /// </summary>
        public CommandResultType Status { get; set; }

        /// <summary>
        /// How the output of the command should be handled if it is a remote request.
        /// Defaults to application/xml where the entire result is output
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String ContentType { get; set; }

        [field: NonSerialized, XmlIgnore, JsonIgnore]
        public event EventHandler Disposed;

        public CommandResult() {
            this.Stamp = DateTime.Now;
            this.Message = String.Empty;

            this.Scope = new CommandData();
            this.Then = new CommandData();
            this.Now = new CommandData();
        }

        protected virtual void OnDisposed() {
            EventHandler handler = Disposed;

            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Note this only releases the items (ignoring the fact the GC will do this anyway)
        /// but does not dispose the items it holds.
        /// </summary>
        public void Dispose() {
            this.Message = null;

            this.Scope.Dispose();
            this.Scope = null;

            this.Then.Dispose();
            this.Then = null;

            this.Now.Dispose();
            this.Now = null;

            this.OnDisposed();
        }
    }
}
