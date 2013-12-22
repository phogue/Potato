using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Shared.Events {

    /// <summary>
    /// Defines a consistent structure that events should be logged or serialized with.
    /// </summary>
    /// <remarks>
    ///     <pre>This is done in a very non-clever way deliberately. Think of it as an Interface that should never change, but instead be added to.</pre>
    /// </remarks>
    [Serializable, XmlType(TypeName = "Event")]
    public sealed class GenericEventArgs : CommandResultArgs {

        /// <summary>
        /// The event ID for this execution. These event ids are volatile, only used to track
        /// during the current execution.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The event being logged.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public GenericEventType GenericEventType {
            get { return this._eventType; }
            set {
                this._eventType = value;

                if (this._eventType != GenericEventType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private GenericEventType _eventType;

        /// <summary>
        /// Converts a command result into a new more specialized generic event.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static GenericEventArgs ConvertToGenericEvent(CommandResultArgs result, GenericEventType eventType) {
            return new GenericEventArgs() {
                GenericEventType = eventType,
                Message = result.Message,
                Stamp = result.Stamp,
                Scope = result.Scope,
                Then = result.Then,
                Now = result.Now
            };
        }
    }
}
