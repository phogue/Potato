namespace Procon.Core.Database.Migrations {
    /// <summary>
    /// Who owns the migration stream, if it is attached a plugin or just part of Procon itself.
    /// </summary>
    public enum MigrationOrigin {
        /// <summary>
        /// The migration(s) are part of a plugin
        /// </summary>
        Plugin,
        /// <summary>
        /// The migration(s) are part of procon core
        /// </summary>
        /// <remarks>This pretty much covers the migration table itself really.</remarks>
        Core
    }
}
