using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;

namespace Procon.Nlp {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Object;
    public interface IStateNlp {

        /// <summary>
        /// Language file, used to lookup values to attach to each object. The xml should be structured like
        /// the namespace of the object.
        /// </summary>
        XElement Document { get; set; }

        /// <summary>
        /// Dictionary of types and the attached linq mapping parameter used in built expressions
        /// </summary>
        Dictionary<Type, LinqParameterMapping> LinqParameterMappings { get; }

        Phrase ParseThing(IStateNlp state, Phrase phrase);
        Phrase ParseMethod(IStateNlp state, Phrase phrase);
        
        SelfReflectionThingObjectToken ParseSelfReflectionThing(IStateNlp state, SelfReflectionThingObjectToken selfThing);

        PropertyInfo GetPropertyInfo(string propertyName);
    }
}
