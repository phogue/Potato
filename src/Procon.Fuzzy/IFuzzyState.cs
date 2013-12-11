using System.Xml.Linq;
using Procon.Fuzzy.Tokens.Object;

namespace Procon.Fuzzy {
    /// <summary>
    /// A state passed through to all combination and parse methods
    /// </summary>
    public interface IFuzzyState {
        /// <summary>
        /// Language file, used to lookup values to attach to each object. The xml should be structured like
        /// the namespace of the object.
        /// </summary>
        XElement Document { get; set; }

        /// <summary>
        /// Converts a phrase into a token if the token matches an object
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseThing(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parses the text for any commands it can use.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseMethod(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parse a property, building the property reference.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseProperty(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parses a self reflection token to define who "me" is.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="selfThing"></param>
        /// <returns></returns>
        SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing);
    }
}