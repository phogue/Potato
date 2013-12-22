using System;

namespace Procon.Core.Shared.Database.Migrations {
    /// <summary>
    /// A migration entry
    /// </summary>
    public class Migration : IMigration {
        public Func<bool> Up { get; set; }

        public Func<bool> Down { get; set; }
    }
}
