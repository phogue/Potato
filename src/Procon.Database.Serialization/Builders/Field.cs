namespace Procon.Database.Serialization.Builders {
    public class Field : Statement {

        /// <summary>
        /// The scope of this field, used when joining on tables.
        /// </summary>
        public new Collection Collection { get; set; }

    }
}
