using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace Procon.Core.Interfaces.Connections.TextCommands.Parsers {
    using Procon.NLP;
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Object;
    using Procon.Net.Protocols.Objects;
    using Procon.Core.Localization;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Security.Objects;

    public abstract class Parser : IStateNLP {

        public List<TextCommand> TextCommands { get; set; }

        // Elsewhere.
        public XElement Document { get; set; }

        public Player Speaker { get; set; }
        public Account SpeakerAccount { get; set; }

        public Connection Connection { get; set; }

        /// <summary>
        /// All linq commands require the exact same object in order to compile
        /// </summary>
        public Dictionary<Type, ParameterExpression> LinqParameterExpressions { get; set; }

        public abstract Phrase ParseThing(IStateNLP state, Phrase phrase);

        public abstract Phrase ParseMethod(IStateNLP state, Phrase phrase);

        public abstract SelfReflectionThingObjectToken ParseSelfReflectionThing(IStateNLP state, SelfReflectionThingObjectToken selfThing);

        public abstract PropertyInfo GetPropertyInfo(string propertyName);

        public abstract TextCommandEventArgs BuildEvent(string prefix, string text, TextCommandEventType eventType);
    }
}
