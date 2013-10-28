using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core {
    /// <summary>
    /// Command to be executed
    /// </summary>
    /// <remarks><para>Called to execute a command.</para></remarks>
    [Serializable]
    public class CommandAttribute : Attribute, IComparable<CommandAttribute>, IDisposable {

        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        [XmlIgnore, JsonIgnore]
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
        [XmlIgnore, JsonIgnore]
        public CommandAttributeType CommandAttributeType { get; set; }

        /// <summary>
        /// Overridden here simply so we can remove the attribute during serialization
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public override object TypeId {
            get {
                return base.TypeId;
            }
        }

        public CommandAttribute() {
            this.CommandAttributeType = CommandAttributeType.Handler;
        }

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        public CommandAttribute ParseCommandType(String commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                this.CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                this.Name = commandName;
            }

            return this;
        }

        /// <summary>
        /// Totally my first time overloading an operator. Never needed to before.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CommandAttribute a, CommandAttribute b) {
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

        public static bool operator !=(CommandAttribute a, CommandAttribute b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        protected bool Equals(CommandAttribute other) {
            return base.Equals(other) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CommandAttribute)obj);
        }

        public int CompareTo(CommandAttribute other) {
            return String.CompareOrdinal(other.Name, this.Name) == 0 ? 0 : 1;
        }

        public void Dispose() {
            this.CommandType = CommandType.None;
            this.Name = null;
        }
    }
}
