using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using Procon.Net.Data;

namespace Procon.Core.Connections.TextCommands.Parsers {
    using Procon.Fuzzy;
    using Procon.Fuzzy.Utils;
    using Procon.Fuzzy.Tokens.Object;
    using Procon.Core.Connections;
    using Procon.Core.Security;
    using Procon.Core.Events;

    public abstract class Parser : IFuzzyState {

        public List<TextCommand> TextCommands { get; set; }

        // Elsewhere.
        public XElement Document { get; set; }

        public Player Speaker { get; set; }
        public Account SpeakerAccount { get; set; }

        public Connection Connection { get; set; }

        /// <summary>
        /// Contains a mapping with more information to use on each type.
        /// </summary>
        public Dictionary<Type, LinqParameterMapping> LinqParameterMappings { get; set; }

        public abstract Phrase ParseThing(IFuzzyState state, Phrase phrase);

        public abstract Phrase ParseMethod(IFuzzyState state, Phrase phrase);

        public abstract SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing);

        public abstract PropertyInfo GetPropertyInfo(string propertyName);

        public abstract CommandResultArgs BuildEvent(string prefix, string text, GenericEventType eventType);
    }
}
