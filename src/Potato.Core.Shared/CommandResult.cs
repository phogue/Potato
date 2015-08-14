#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;

namespace Potato.Core.Shared {
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

        public string Name { get; set; }

        public string Message { get; set; }

        public DateTime Stamp { get; set; }

        public ICommandData Scope { get; set; }

        public ICommandData Then { get; set; }

        public ICommandData Now { get; set; }

        public bool Success { get; set; }

        public CommandResultType CommandResultType {
            get { return _mCommandResultType; }
            set {
                _mCommandResultType = value;

                if (_mCommandResultType != CommandResultType.None) {
                    Name = value.ToString();
                }
            }
        }
        private CommandResultType _mCommandResultType;

        public string ContentType { get; set; }

        /// <summary>
        /// Called when the object is being disposed.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes the command result with the default values.
        /// </summary>
        public CommandResult() {
            Name = string.Empty;
            Stamp = DateTime.Now;
            Message = string.Empty;

            Scope = new CommandData();
            Then = new CommandData();
            Now = new CommandData();
        }

        protected virtual void OnDisposed() {
            var handler = Disposed;

            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Note this only releases the items (ignoring the fact the GC will do this anyway)
        /// but does not dispose the items it holds.
        /// </summary>
        public void Dispose() {
            Name = null;
            Message = null;

            Scope.Dispose();
            Scope = null;

            Then.Dispose();
            Then = null;

            Now.Dispose();
            Now = null;

            OnDisposed();
        }
    }
}
