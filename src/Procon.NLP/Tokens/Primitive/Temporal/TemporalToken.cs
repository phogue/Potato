namespace Procon.Nlp.Tokens.Primitive.Temporal {
    public class TemporalToken : PrimitiveToken {

        /// <summary>
        /// The underlying date/time pattern built up so far by matching tokens.
        /// </summary>
        public DateTimePatternNlp Pattern { get; set; }
    }
}
