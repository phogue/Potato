using System;

namespace Procon.Core.Database.Migrations {
    /// <summary>
    /// What actions should be taken in a migration up/down stream.
    /// </summary>
    public interface IMigration {
        /// <summary>
        /// A handler to execute when migrating upwards.
        /// </summary>
        /// <returns>True if the migration was successful, false if an error occured</returns>
        Func<bool> Up { get; set; }

        /// <summary>
        /// A handler to execute when migrating downwards.
        /// </summary>
        /// <returns>True if the migration was successful, false if an error occured</returns>
        Func<bool> Down { get; set; }
    }
}
