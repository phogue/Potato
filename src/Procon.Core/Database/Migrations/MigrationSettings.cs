using System;

namespace Procon.Core.Database.Migrations {
    /// <summary>
    /// Default migration settings
    /// </summary>
    public class MigrationSettings : IMigrationSettings {
        /// <summary>
        /// The type of of migration (core, plugin originated)
        /// </summary>
        public MigrationOrigin Origin { get; set; }

        /// <summary>
        /// The name of the stream to focus on.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The current version of the migration for the specified name.
        /// </summary>
        public int CurrentVersion { get; set; }

    }
}
