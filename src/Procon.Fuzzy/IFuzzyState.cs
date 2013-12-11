using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using Procon.Fuzzy.Utils;
using Procon.Fuzzy.Tokens.Object;

namespace Procon.Fuzzy {
    public interface IFuzzyState {
        /// <summary>
        /// Language file, used to lookup values to attach to each object. The xml should be structured like
        /// the namespace of the object.
        /// </summary>
        XElement Document { get; set; }

        /// <summary>
        /// Dictionary of types and the attached linq mapping parameter used in built expressions
        /// </summary>
        Dictionary<Type, LinqParameterMapping> LinqParameterMappings { get; }

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

        Phrase ParseProperty(IFuzzyState state, Phrase phrase);

        SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing);
    }
}