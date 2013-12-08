using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Procon.Core.Events;
using Procon.Core.Security;
using Procon.Fuzzy;
using Procon.Fuzzy.Tokens.Object;
using Procon.Fuzzy.Utils;
using Procon.Net.Data;

namespace Procon.Core.Connections.TextCommands.Parsers {

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
