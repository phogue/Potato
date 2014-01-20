using System;
using System.Collections.Generic;

namespace Procon.Core.Shared {
    /// <summary>
    /// Command to be executed
    /// </summary>
    /// <remarks><para>Called to execute a command.</para></remarks>
    [Serializable]
    public class CommandDispatch : IComparable<CommandDispatch>, IDisposable, IEquatable<CommandDispatch> {

        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        public CommandType CommandType {
            get { return this._mCommandType; }
            set {
                this._mCommandType = value;

                if (this._mCommandType != CommandType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        /// <summary>
        /// When in the execution we want to capture the command (before, as the handler or after)
        /// </summary>
        public CommandAttributeType CommandAttributeType { get; set; }

        /// <summary>
        /// A list of parameter names with the type of parameter expected.
        /// </summary>
        public List<CommandParameterType> ParameterTypes { get; set; }

        /// <summary>
        /// Initializes the dispatch with default values.
        /// </summary>
        public CommandDispatch() {
            this.CommandAttributeType = CommandAttributeType.Handler;
        }

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        public CommandDispatch ParseCommandType(String commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                this.CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                this.Name = commandName;
            }

            return this;
        }
        
        /// <summary>
        /// Checks if this dspatcher can handle the command
        /// </summary>
        /// <returns></returns>
        public bool CanDispatch(CommandAttributeType attributeType, ICommand command) {
            return this.CommandAttributeType == attributeType && this.Name == command.Name;
        }

        /// <summary>
        /// Totally my first time overloading an operator. Never needed to before.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CommandDispatch a, CommandDispatch b) {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.CompareTo(b) == 0;
        }

        /// <summary>
        /// Second time overloading an operator. It's worn off now.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(CommandDispatch a, CommandDispatch b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (ParameterTypes != null ? ParameterTypes.GetHashCode() : 0);
            }
        }

        public bool Equals(CommandDispatch other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name) && Equals(ParameterTypes, other.ParameterTypes);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CommandDispatch) obj);
        }

        public int CompareTo(CommandDispatch other) {
            return String.CompareOrdinal(other.Name, this.Name) == 0 ? 0 : 1;
        }
        
        public void Dispose() {
            this.CommandType = CommandType.None;
            this.Name = null;
        }
    }
}
