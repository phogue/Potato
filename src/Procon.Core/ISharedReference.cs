namespace Procon.Core {
    /// <summary>
    /// Enforces variable to access static references of common controllers.
    /// </summary>
    public interface ISharedReferenceAccess {

        /// <summary>
        /// A reference to the shared reference of static variables.
        /// </summary>
        SharedReferences Shared { get; }
    }
}
