using System;

namespace Procon.Core.Shared.Events {
    /// <summary>
    /// Defines a consistent structure that events should be logged or serialized with.
    /// </summary>
    /// <remarks>
    ///     <pre>This is done in a very non-clever way deliberately. Think of it as an Interface that should never change, but instead be added to.</pre>
    /// </remarks>
    public interface IGenericEvent : ICommandResult {
        /// <summary>
        /// The event ID for this execution. These event ids are volatile, only used to track
        /// during the current execution.
        /// </summary>
        ulong Id { get; set; }

        /// <summary>
        /// The event being logged.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        GenericEventType GenericEventType { get; set; }
    }
}
