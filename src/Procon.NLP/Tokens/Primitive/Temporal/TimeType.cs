namespace Procon.Nlp.Tokens.Primitive.Temporal {
    public enum TimeType {
        /// <summary>
        /// No specifics given for this time
        /// </summary>
        None,
        /// <summary>
        /// The date/time should be considered a single moment regardless of the current
        /// date/time.
        /// </summary>
        Definitive,
        /// <summary>
        /// The date/time should be used as an offset +/- for now
        /// </summary>
        Relative
    }
}
