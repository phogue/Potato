using System;

namespace Procon.Core.Database.Migrations {
    /// <summary>
    /// An entry into the migration table
    /// </summary>
    public class MigrationModel : DatabaseModel<MigrationModel> {
        /// <summary>
        /// The origin of the migration (Core, Plugin)
        /// </summary>
        public String Origin { get; set; }

        /// <summary>
        /// The name of the stream.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The version of the stream on this entry.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// When this entry was added to the migration table.
        /// </summary>
        public DateTime Stamp { get; set; }
    }
}
