using System;

namespace Procon.Core.Shared {
    /// <summary>
    /// I'd be tempted to refactor GenericEventArgs so I can seal this class. It's
    /// used as a backbone for xml serialization so any inherited classes could
    /// cause the xml serializer to encounter a type it didn't expect.
    /// </summary>
    [Serializable]
    public class CommandResult : IDisposable, ICommandResult {
        /// <summary>
        /// A static result describing insufficient permissions
        /// </summary>
        /// <remarks>May be moved to a "CommandResultBuilder" class at some point.</remarks>
        public static ICommandResult InsufficientPermissions = new CommandResult() {
            Success = false,
            CommandResultType = CommandResultType.InsufficientPermissions,
            Message = "You have Insufficient Permissions to execute this command."
        };

        public String Name { get; set; }

        public String Message { get; set; }

        public DateTime Stamp { get; set; }

        public ICommandData Scope { get; set; }

        public ICommandData Then { get; set; }

        public ICommandData Now { get; set; }

        public Boolean Success { get; set; }

        public CommandResultType CommandResultType {
            get { return this._mCommandResultType; }
            set {
                this._mCommandResultType = value;

                if (this._mCommandResultType != CommandResultType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private CommandResultType _mCommandResultType;

        public String ContentType { get; set; }

        /// <summary>
        /// Called when the object is being disposed.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes the command result with the default values.
        /// </summary>
        public CommandResult() {
            this.Name = String.Empty;
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
            this.Name = null;
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
