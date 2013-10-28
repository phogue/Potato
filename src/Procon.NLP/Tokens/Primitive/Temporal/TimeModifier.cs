namespace Procon.Nlp.Tokens.Primitive.Temporal {
    public enum TimeModifier {
        /// <summary>
        /// Nada
        /// </summary>
        None,
        /// <summary>
        /// A delay, should used as an offset for +now e.g "in 20 minutes"
        /// </summary>
        Delay,
        /// <summary>
        /// A period of time, without thought to the current time e.g "for 20 minutes"
        /// </summary>
        Period,
        /// <summary>
        /// An interval of time, without thought to the current time e.g "every 20 minutes"
        /// </summary>
        Interval
    }
}
