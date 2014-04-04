using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Predefined traits to attach to a permission.
    /// </summary>
    public static class PermissionTraitsType {
        /// <summary>
        /// The authority level should be trated as a boolean, authority == (0 || null) = false and
        /// authority > 0 = true.
        /// </summary>
        public const String Boolean = "Boolean";
    }
}
