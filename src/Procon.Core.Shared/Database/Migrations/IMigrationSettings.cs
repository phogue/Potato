using System;

namespace Procon.Core.Shared.Database.Migrations {
    /// <summary>
    /// Holds settings to describe how migrations should be handled and what stream to look into.
    /// </summary>
    public interface IMigrationSettings {
        /// <summary>
        /// The name of the stream to focus on.
        /// </summary>
        MigrationOrigin Origin { get; set; }

        /// <summary>
        /// The name of the stream to focus on.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The current version of the migration for the specified name.
        /// </summary>
        int CurrentVersion { get; set; }
    }
}
