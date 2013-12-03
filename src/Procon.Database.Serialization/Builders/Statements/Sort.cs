using System;

namespace Procon.Database.Serialization.Builders.Statements {
    [Serializable]
    public class Sort : Statement {
        /// <summary>
        /// The scope of this field, used when joining on tables.
        /// </summary>
        public new Collection Collection { get; set; }
    }
}
