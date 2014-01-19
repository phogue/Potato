using System;
using Newtonsoft.Json;

namespace Procon.Core.Shared.Events {
    /// <summary>
    /// Defines a consistent structure that events should be logged or serialized with.
    /// </summary>
    /// <remarks>
    ///     <pre>This is done in a very non-clever way deliberately. Think of it as an Interface that should never change, but instead be added to.</pre>
    /// </remarks>
    [Serializable]
    public sealed class GenericEvent : CommandResult, IGenericEvent {

        public ulong Id { get; set; }

        public String Name { get; set; }

        [JsonIgnore]
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
        public static GenericEvent ConvertToGenericEvent(CommandResult result, GenericEventType eventType) {
            return new GenericEvent() {
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
